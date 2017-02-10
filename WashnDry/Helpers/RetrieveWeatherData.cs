using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Android.Locations;

namespace WashnDry
{
	public class RetrieveWeatherData
	{
		public static async Task<JsonValue> FetchWeatherAsync(string latitude, string longitude)
		{
			string url = "http://api.openweathermap.org/data/2.5/weather?appid=f30fd8bd2d1f9f1bbdfbd627f9faa54b&lat=" + latitude + "&lon=" + longitude + "&units=metric";
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
			request.ContentType = "application/json";
			request.Method = "GET";

			// Send the request to the server and wait for the response:
			using (WebResponse response = await request.GetResponseAsync())
			{
				// Get a stream representation of the HTTP web response:
				using (Stream stream = response.GetResponseStream())
				{
					// Use this stream to build a JSON document object:
					JsonValue jsonDoc = await Task.Run(() => JsonValue.Load(stream));
					//Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
					// Return the JSON document:
					return jsonDoc;
				}
			}
		}

		public static async Task<JsonValue> FetchFiveDayWeatherForecastAsync(string latitude, string longitude)
		{
			Console.Out.WriteLine("latitude: " + latitude + " longitude: " + longitude);
			string url = "http://api.openweathermap.org/data/2.5/forecast?appid=f30fd8bd2d1f9f1bbdfbd627f9faa54b&lat=" + latitude + "&lon=" + longitude + "&units=metric";
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
			request.ContentType = "application/json";
			request.Method = "GET";

			// Send the request to the server and wait for the response:
			using (WebResponse response = await request.GetResponseAsync())
			{
				// Get a stream representation of the HTTP web response:
				using (Stream stream = response.GetResponseStream())
				{
					// Use this stream to build a JSON document object:
					JsonValue jsonDoc = await Task.Run(() => JsonValue.Load(stream));
					//Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
					// Return the JSON document:
					return jsonDoc;
				}
			}
		}



		public static async void updateFiveDayWashDatesAndThreeBestTimings()
		{
			Console.Out.WriteLine("Inside getFiveDayWeatherData()");
			Context mContext = Android.App.Application.Context;
			AppPreferences ap = new AppPreferences(mContext);
			string _latitude = ap.getCurrentLatitude();
			string _longitude = ap.getCurrentLongitude();
			string[] all_event_dates = ap.getCalendarEventsDates().Split(',');

			if (_latitude == "" || _longitude == "")
			{
				return;
			}
			JsonValue weatherData5Day = await RetrieveWeatherData.FetchFiveDayWeatherForecastAsync(_latitude, _longitude);
			if (weatherData5Day != null)
			{
				string[,] dataString = DataTransformers.parseFiveDayWeatherData(weatherData5Day);
				JsonValue response = await RetrieveServerData.InvokeRequestResponseService(dataString);
				if (response != null)
				{
					//parse and store data 
					float veryGoodThreshold = 2500;
					float goodThreshold = 2800;
					float okThreshold = 3200;
					string veryGoodList = "";
					string goodList = "";
					string okList = "";
					string threeBestTimingsDates = "";
					string threeBestTimingsDurations = "";
					int hoursOffset = DateTime.Now.Hour;

					var data = response["Results"]["output1"]["value"]["Values"];
					List<string[]> dataWithOrder = new List<string[]>();
					Console.Out.WriteLine("dataCount: " + data.Count);

					for (int i = 0; i < data.Count; i++)
					{
						dataWithOrder.Add(new string[] { i.ToString(), data[i][5] });
					}

					var dataList = dataWithOrder.OrderBy(arr => float.Parse(arr[1])).ToList();

					int numBestTimingDates = 0;

					for (int i = 0; i < dataList.Count; i++)
					{
						var timing = dataList[i][1].ToString();
						var hoursFromStartOfDay = int.Parse(dataList[i][0]) * 3 + hoursOffset;
						Console.Out.WriteLine(dataList[i][0] + " " + dataList[i][1]);
						if (numBestTimingDates < 3)
						{
							string candidateBestTimingDate = DataTransformers.roundOffToPastHour(DateTime.Now.AddHours(hoursFromStartOfDay - DateTime.Now.Hour)).ToString();
							if (!all_event_dates.Contains(candidateBestTimingDate))
							{
								if (numBestTimingDates == 0)
								{
									threeBestTimingsDates = candidateBestTimingDate;
									threeBestTimingsDurations = timing;
								}
								else {
									threeBestTimingsDates = threeBestTimingsDates + "," + candidateBestTimingDate;
									threeBestTimingsDurations = threeBestTimingsDurations + ", " + timing;
								}
								numBestTimingDates += 1;
							}
						}
						if (float.Parse(timing) <= veryGoodThreshold)
						{
							if (veryGoodList == "")
							{
								veryGoodList = hoursFromStartOfDay.ToString();
							}
							else {
								veryGoodList = veryGoodList + "," + hoursFromStartOfDay.ToString();
							}
						}
						else if (float.Parse(timing) <= goodThreshold)
						{
							if (goodList == "")
							{
								goodList = hoursFromStartOfDay.ToString();
							}
							else {
								goodList = goodList + "," + hoursFromStartOfDay.ToString();
							}
						}
						else if (float.Parse(timing) <= okThreshold)
						{
							if (okList == "")
							{
								okList = hoursFromStartOfDay.ToString();
							}
							else {
								okList = okList + "," + hoursFromStartOfDay.ToString();
							}
						}

					}
					string latestScheduleDate = DateTime.Now.ToString("dd MMM");
					//Console.Out.WriteLine(veryGoodList);
					//Console.Out.WriteLine(goodList);
					//Console.Out.WriteLine(okList);
					Console.Out.WriteLine(threeBestTimingsDates);
					ap.saveLatestVeryGoodPositions(veryGoodList);
					ap.saveLatestGoodPositions(goodList);
					ap.saveLatestOkPositions(okList);
					ap.saveThreeBestTimings(threeBestTimingsDates + "_" + threeBestTimingsDurations);
					ap.saveLatestScheduleDate(latestScheduleDate);
				}
			}
		}



	}
}
