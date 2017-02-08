using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Locations;

namespace WashnDry
{

	[Service]
	public class RetrieveLocationService : Service, ILocationListener
	{
		static readonly string TAG = "X:" + typeof(RetrieveLocationService).Name;
		static readonly int TimerWait = 4000;
		Timer timer;
		DateTime startTime;
		//bool isStarted = false;
		int count = 0;
		Location _currentLocation;
		LocationManager _locationManager;
		string _locationProvider;
		Context context;
		string _addressText;
		string _locationText;
		string _latitude;
		string _longitude;

		public override void OnCreate()
		{
			base.OnCreate();
			context = this;
			InitializeLocationManager();
			getLocation();
			_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			startTime = DateTime.UtcNow;
			timer = new Timer(HandleTimerCallback, startTime, 0, TimerWait);
			return StartCommandResult.NotSticky;
		}

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
		}

		void HandleTimerCallback(object state)
		{
			BroadcastStarted();
			//getFiveDayWeatherData();
			count = count + 1;
		}

		private void BroadcastStarted()
		{
			InitializeLocationManager();
			getLocation();
			Intent BroadcastIntent = new Intent(this, typeof(WeatherFragment.LocationBroadcastReceiver));
			string action = "Send Location Data";
			BroadcastIntent.SetAction(action);
			BroadcastIntent.AddCategory(Intent.CategoryDefault);
			if (_currentLocation == null)
			{
				_latitude = "none";
				_longitude = "none";
			}
			else {
				_latitude = _currentLocation.Latitude.ToString();
				_longitude = _currentLocation.Longitude.ToString();
			}
			BroadcastIntent.PutExtra("Latitude", _latitude);
			BroadcastIntent.PutExtra("Longitude", _longitude);
			BroadcastIntent.PutExtra("Address", _addressText);
			SendBroadcast(BroadcastIntent);
		}


		void InitializeLocationManager()
		{

			_locationManager = (LocationManager)context.GetSystemService(Context.LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				_locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				_locationProvider = string.Empty;
			}
		}

		async void getLocation()
		{
			if (_currentLocation == null)
			{
				_addressText = "Can't determine the current address. Try again in a few minutes.";
				return;
			}

			Address address = await ReverseGeocodeCurrentLocation();
			getAddress(address);
		}

		void getAddress(Address address)
		{
			if (address != null)
			{
				StringBuilder deviceAddress = new StringBuilder();
				for (int i = 0; i < address.MaxAddressLineIndex; i++)
				{
					deviceAddress.AppendLine(address.GetAddressLine(i));
				}
				// Remove the last comma from the end of the address.
				_addressText = deviceAddress.ToString();
			}
			else
			{
				_addressText = "Unable to determine the address. Try again in a few minutes.";
			}
		}

		async Task<Address> ReverseGeocodeCurrentLocation()
		{
			Geocoder geocoder = new Geocoder(context);
			IList<Address> addressList =
				await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

			Address address = addressList.FirstOrDefault();
			return address;
		}

		public async void OnLocationChanged(Location location)
		{
			_currentLocation = location;
			if (_currentLocation == null)
			{
				_locationText = "Unable to determine your location. Try again in a short while.";
			}
			else
			{
				_locationText = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
				Address address = await ReverseGeocodeCurrentLocation();
				getAddress(address);
				// store location
				Context mContext = Application.Context;
				AppPreferences ap = new AppPreferences(mContext);
				ap.saveCurrentLatitude(_currentLocation.Latitude.ToString());
				ap.saveCurrentLongitude(_currentLocation.Longitude.ToString());
			}
		}

		public void OnProviderDisabled(string provider) { }

		public void OnProviderEnabled(string provider) { }

		public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

		public static async void getCurrentDryingTime(string[,] parsedWeatherData)
		{
			JsonValue serverData = await RetrieveServerData.InvokeRequestResponseService(parsedWeatherData);
			//Console.Out.WriteLine("serverData: " + serverData.ToString());
			double dryingTimeInSeconds = double.Parse(serverData["Results"]["output1"]["value"]["Values"][0][5]);
			//Console.Out.WriteLine("dryingTimeInSeconds: " + dryingTimeInSeconds.ToString());
			TimeSpan time = TimeSpan.FromSeconds(dryingTimeInSeconds);
			//Console.Out.WriteLine("time.ToString(): " + time.ToString());
			Context mContext = Android.App.Application.Context;
			AppPreferences ap = new AppPreferences(mContext);
			string currentDryingTime = string.Format("{0}hr {1}min", time.Hours, time.Minutes);
			if (time.Hours == 0){
				currentDryingTime = string.Format("{0}min", time.Minutes);
			}
			//Console.Out.WriteLine("Current Drying Time: " + currentDryingTime);
			ap.saveCurrentDryingtime(currentDryingTime);
		}

		public static async void updateFiveDayWashDatesAndThreeBestTimings()
		{
			Console.Out.WriteLine("Inside getFiveDayWeatherData()");
			Context mContext = Android.App.Application.Context;
			AppPreferences ap = new AppPreferences(mContext);
			string _latitude = ap.getCurrentLatitude();
			string _longitude = ap.getCurrentLongitude();
			JsonValue weatherData5Day = await RetrieveWeatherData.FetchFiveDayWeatherForecastAsync(_latitude, _longitude);
			if (weatherData5Day != null)
			{
				string[,] dataString = DataTransformers.parseFiveDayWeatherData(weatherData5Day);
				JsonValue response = await RetrieveServerData.InvokeRequestResponseService(dataString);
				if (response != null)
				{
					//parse and store data 
					float veryGoodThreshold = 2500;
					float goodThreshold = 2750;
					float okThreshold = 3000;
					string veryGoodList = "";
					string goodList = "";
					string okList = "";
					string threeBestTimingsDates = "";
					string threeBestTimingsTimes = "";
					int hoursOffset = DateTime.Now.Hour - 3;
					var data = response["Results"]["output1"]["value"]["Values"];
					List<string[]> dataWithOrder = new List<string[]>();
					Console.Out.WriteLine("dataCount: " + data.Count);
					for (int i = 0; i < data.Count; i++)
					{
						dataWithOrder.Add(new string[] { i.ToString(), data[i][5] });
					}
					var dataList = dataWithOrder.OrderBy(arr => arr[1]).ToList();

					for (int i = 0; i < dataList.Count; i++)
					{
						var timing = dataList[i][1];
						var hoursLater = int.Parse(dataList[i][0]) * 3 + hoursOffset;
						if (i < 3)
						{
							if (i == 0)
							{
								threeBestTimingsDates = DateTime.Now.AddHours(hoursLater).ToString();
								threeBestTimingsTimes = timing;
							}
							else {
								threeBestTimingsDates = threeBestTimingsDates + "," + DateTime.Now.AddHours(hoursLater);
								threeBestTimingsTimes = threeBestTimingsTimes + ", " + timing;
							}
						}
						if (float.Parse(timing) <= veryGoodThreshold)
						{
							if (veryGoodList == "")
							{
								veryGoodList = hoursLater.ToString();
							}
							else {
								veryGoodList = veryGoodList + "," + hoursLater.ToString();
							}
						}
						else if (float.Parse(timing) <= goodThreshold)
						{
							if (goodList == "")
							{
								goodList = hoursLater.ToString();
							}
							else {
								goodList = goodList + "," + hoursLater.ToString();
							}
						}
						else if (float.Parse(timing) <= okThreshold)
						{
							if (okList == "")
							{
								okList = hoursLater.ToString();
							}
							else {
								okList = okList + "," + hoursLater.ToString();
							}
						}

					}
					string latestScheduleDate = DateTime.Now.ToString("dd MMM");
					//Console.Out.WriteLine(veryGoodList);
					//Console.Out.WriteLine(goodList);
					//Console.Out.WriteLine(okList);
					//Console.Out.WriteLine(threeBestTimings);
					ap.saveLatestVeryGoodPositions(veryGoodList);
					ap.saveLatestGoodPositions(goodList);
					ap.saveLatestOkPositions(okList);
					ap.saveThreeBestTimings(threeBestTimingsDates+"_"+threeBestTimingsTimes);
					ap.saveLatestScheduleDate(latestScheduleDate);
				}
			}
		}
	}
}
