using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Locations;
using System.Threading.Tasks;
using Plugin.Geolocator;
using System.Net;
using System.Json;
using System.IO;
using System.Threading;


namespace WashnDry
{
	public class HomeFragment : Fragment
	{

		TextView _addressText;
		TextView _locationText;
		TextView _temperatureText;
		TextView _windText;
		TextView _humidityText;
		TextView _weatherText;
		JsonValue weatherData;
		static readonly int TimerWait = 2000;
		Timer timer;
		DateTime startTime;

		static string _longitude;
		static string _latitude;
		static string _address;
		static string _currentWeather;
		static string _currentTemperature;
		static string _currentWind;
		static string _currentHumidity;
		//RetrieveWeatherData retrieveWeatherData = new RetrieveWeatherData();

		View rootView;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			//return base.OnCreateView(inflater, container, savedInstanceState);

			rootView = inflater.Inflate(Resource.Layout.Home, container, false);
			getUIElements();
			return rootView;
		}

		public override void OnResume()
		{
			base.OnResume();
			updateDisplays();
			startTime = DateTime.UtcNow;
			timer = new Timer(HandleTimerCallback, startTime, 0, TimerWait);
		}

		public override void OnPause()
		{
			base.OnPause();
		}

		void HandleTimerCallback(object state)
		{
			if (_latitude != "none" && _latitude != null)
			{
				getWeatherData();
				Activity.RunOnUiThread(updateDisplays);
			}
			else {
				Activity.RunOnUiThread(displayGettingLocation);
			}
		}

		void displayGettingLocation()
		{
			Toast.MakeText(this.Activity, "Getting Location", ToastLength.Short).Show();
		}

		public override void OnStop()
		{
			base.OnStop();
			timer.Dispose();
		}

		void updateDisplays()
		{
			_locationText.Text = _latitude + " " + _longitude;
			_addressText.Text = _address;
			_weatherText.Text = "Weather: " + _currentWeather;
			_temperatureText.Text = "Temperature: " + _currentTemperature;
			_windText.Text = "Wind Speed: " + _currentWind;
			_humidityText.Text = "Humidity: " + _currentHumidity;
		}

		async void getWeatherData()
		{
			weatherData = await RetrieveWeatherData.FetchWeatherAsync(_latitude, _longitude);
			if (weatherData != null)
				parseWeatherData(weatherData);
		}

		private void parseWeatherData(JsonValue json)
		{
			_currentWeather = json["weather"][0]["description"].ToString();
			_currentTemperature = json["main"]["temp"].ToString();
			_currentWind = json["wind"]["speed"].ToString();
			_currentHumidity = json["main"]["humidity"].ToString();
		}

		public void getUIElements()
		{
			_locationText = rootView.FindViewById<TextView>(Resource.Id.CurrentLocationText);
			_addressText = rootView.FindViewById<TextView>(Resource.Id.AddressText);
			_weatherText = rootView.FindViewById<TextView>(Resource.Id.WeatherText);
			_temperatureText = rootView.FindViewById<TextView>(Resource.Id.TemperatureText);
			_windText = rootView.FindViewById<TextView>(Resource.Id.WindText);
			_humidityText = rootView.FindViewById<TextView>(Resource.Id.HumidityText);
		}

		[BroadcastReceiver]
		public class LocationBroadcastReceiver : BroadcastReceiver
		{
			public static readonly string GRID_STARTED = "GRID_STARTED";
			public override void OnReceive(Context context, Intent intent)
			{
				if (intent.Action == GRID_STARTED)
				{
					Toast.MakeText(context, "Grid Started", ToastLength.Short).Show();
				}
				else {
					
					Bundle extras = intent.Extras;
					_longitude = extras.GetString("Longitude");
					_latitude = extras.GetString("Latitude");
					_address = extras.GetString("Address");
					//Toast.MakeText(context, _latitude + " " + _longitude + " " + _address, ToastLength.Short).Show();
				}
			}
		}

		private LocationBroadcastReceiver _receiver;

		private void RegisterBroadcastReceiver()
		{
			IntentFilter filter = new IntentFilter(LocationBroadcastReceiver.GRID_STARTED);
			filter.AddCategory(Intent.CategoryDefault);
			_receiver = new LocationBroadcastReceiver();
			this.Activity.RegisterReceiver(_receiver, filter);
		}

		private void UnRegisterBroadcastReceiver()
		{
			this.Activity.UnregisterReceiver(_receiver);
		}

	}
}
