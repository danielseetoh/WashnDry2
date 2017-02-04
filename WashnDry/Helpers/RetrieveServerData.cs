// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client

using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Net;
//using System.Net.Http;
//using System.Net.Http.Formatting;
//using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WashnDry
{

	public class StringTable
	{
		public string[] ColumnNames { get; set; }
		public string[,] Values { get; set; }
	}

	public class RetrieveServerData
	{

		public static async Task<JsonValue> fetchFiveDayWashDates(List<string[]> weatherData)
		{
			string url = "https://ussouthcentral.services.azureml.net/workspaces/24adfa57a81e45f880ce53d63175dbbd/services/a61c0cd4db904bef8fb0be3ec99e4fd8/execute?api-version=2.0&details=true";
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
			request.ContentType = "application/json";
			request.Method = "POST";
			var scoreRequest = new
			{

				Inputs = new Dictionary<string, StringTable>() {
						{
							"input1",
							new StringTable()
							{
								ColumnNames = new string[] {"Profile", "Temperature", "Humidity", "Precipitation(%)", "Irradiation", "Windspeed", "Indoors/outdoors", "Drying-time"},
								Values = new string[,] {  { "value", "0", "0", "0", "0", "0", "0", "0" },  { "value", "0", "0", "0", "0", "0", "0", "0" },  }
							}
						},
					},
				GlobalParameters = new Dictionary<string, string>()
				{
				}
			};
			const string apiKey = "Jj/xaQ1uR7Msnef4WChG08zBdVw4mJhyTcBIXcQLW1WLE7HWvlMQt7Xu5ZJCiP1JtqIluq0wRnQ4S5bfbRV++g==";
			request.PreAuthenticate = true;
			request.Headers.Add("Authorization", "Bearer " + apiKey);
			request.Accept = "application/json";

			using (var streamWriter = new StreamWriter(request.GetRequestStream()))
			{
				//var json = scoreRequest;

				streamWriter.Write(scoreRequest);
				streamWriter.Flush();
			}

			Console.Out.WriteLine("Inside RetrieveServerData.fetchFiveDayWashDates");
			// Send the request to the server and wait for the response:
			using (WebResponse response = await request.GetResponseAsync())
			{
				Console.WriteLine(((HttpWebResponse)response).StatusDescription);
				// Get a stream representation of the HTTP web response:
				using (Stream stream = response.GetResponseStream())
				{
					StreamReader reader = new StreamReader(stream);
					// Read the content.  
					string responseFromServer = reader.ReadToEnd();
					// Display the content.  
					Console.WriteLine(responseFromServer);

					// Use this stream to build a JSON document object:
					JsonValue jsonDoc = await Task.Run(() => JsonValue.Load(stream));
					//Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
					// Return the JSON document:
					return jsonDoc;

				}
			}
		}

	}
	//class Program
	//{
	//	static void Main(string[] args)
	//	{
	//		InvokeRequestResponseService().Wait();
	//	}

	//	static async Task InvokeRequestResponseService()
	//	{
	//		using (var client = new HttpClient())
	//		{
	//			var scoreRequest = new
	//			{

	//				Inputs = new Dictionary<string, StringTable>() {
	//					{
	//						"input1",
	//						new StringTable()
	//						{
	//							ColumnNames = new string[] {"Profile", "Temperature", "Humidity", "Precipitation(%)", "Irradiation", "Windspeed", "Indoors/outdoors", "Drying-time"},
	//							Values = new string[,] {  { "value", "0", "0", "0", "0", "0", "0", "0" },  { "value", "0", "0", "0", "0", "0", "0", "0" },  }
	//						}
	//					},
	//				},
	//				GlobalParameters = new Dictionary<string, string>()
	//				{
	//				}
	//			};
	//			const string apiKey = "Jj/xaQ1uR7Msnef4WChG08zBdVw4mJhyTcBIXcQLW1WLE7HWvlMQt7Xu5ZJCiP1JtqIluq0wRnQ4S5bfbRV++g=="; // Replace this with the API key for the web service
	//			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

	//			client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/24adfa57a81e45f880ce53d63175dbbd/services/a61c0cd4db904bef8fb0be3ec99e4fd8/execute?api-version=2.0&details=true");

	//			// WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
	//			// One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
	//			// For instance, replace code such as:
	//			//      result = await DoSomeTask()
	//			// with the following:
	//			//      result = await DoSomeTask().ConfigureAwait(false)

	//			HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

	//			if (response.IsSuccessStatusCode)
	//			{
	//				string result = await response.Content.ReadAsStringAsync();
	//				Console.WriteLine("Result: {0}", result);
	//			}
	//			else
	//			{
	//				Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

	//				// Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
	//				Console.WriteLine(response.Headers.ToString());

	//				string responseContent = await response.Content.ReadAsStringAsync();
	//				Console.WriteLine(responseContent);
	//			}
	//		}
	//	}
	//}
}