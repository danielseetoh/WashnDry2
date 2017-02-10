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
	public class WeatherFragment : Fragment
	{
		public System.Threading.Timer toNextDryTimer;
		public System.Threading.Timer dryingTimer;
		//static int timeLeftInSeconds;
		//int initialTimeInSeconds;

		//enum State { ready, notReady, dryingInProgress, laundryFinished };
		//static State state = State.notReady;

		//TextView nextLaundryTV, timeToNextLaundryTV;
		//TextView timerTextView, estTextView, timeTakenTextView;
		//Button startDryingButton, restartDryingButton, stopDryingButton;

		//DateTime nextLaundryDate;
		//DateTime currentDate;

		//DateCalculator dc = new DateCalculator();
		//DateCalculator nextLaundryDc = new DateCalculator();


		//TextView descriptionText;

		//TextView _addressText;
		//TextView _locationText;
		TextView _temperatureText;
		TextView _windText;
		TextView _humidityText;
		TextView _weatherText;
		TextView _estDryingTimeText;
		JsonValue weatherData;
		static readonly int TimerWait = 2000;
		Timer timer;
		DateTime startTime;

		static string _longitude;
		static string _latitude;
		//static string _address;
		static string _currentWeather;
		static string _currentTemperature;
		static string _currentWind;
		static string _currentHumidity;
		static string _estimatedDryingTime;

		View rootView;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}



		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			rootView = inflater.Inflate(Resource.Layout.Weather, container, false);

			//estTextView = rootView.FindViewById<TextView>(Resource.Id.estTime);
			getUIElements();

			return rootView;
		}



		public override void OnPause()
		{
			base.OnPause();
			if (dryingTimer != null)
			{
				Console.WriteLine(" dryertimer destroyed onpause event");
				dryingTimer.Dispose();
				dryingTimer = null;
			}
			if (toNextDryTimer != null)
			{
				Console.WriteLine("to next dry timer destroyed onpause event");
				toNextDryTimer.Dispose();
				toNextDryTimer = null;
			}

		}

		public override void OnResume()
		{
			base.OnResume();

			updateDisplays();
			startTime = DateTime.UtcNow;
			timer = new Timer(HandleTimerCallback, startTime, 0, TimerWait);

		}


		[BroadcastReceiver]
		public class HomeBroadcastReceiver : BroadcastReceiver
		{
			public static readonly string sendCountDownTimerData = "SendCountDownTimerData";
			public static readonly string sendLocationBroadcast = "SendLocationData";

			public override void OnReceive(Context context, Intent intent)
			{
				if (intent.Action == sendCountDownTimerData) // There can be multipe broadcast received. This is the general listener. check whether the event received is the one which starts the countdown
				{
					Bundle extras = intent.Extras;
					//timeLeftInSeconds = extras.GetInt("timeLeftInSeconds");
					//if (extras.GetBoolean("isLaundryDone")) { state = State.laundryFinished; }
				}

				else if (intent.Action == sendLocationBroadcast)
				{
					// get location data here
				}

			}
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
			_weatherText.Text = _currentWeather;
			_temperatureText.Text = _currentTemperature;
			_windText.Text = _currentWind;
			_humidityText.Text = _currentHumidity;
			if (_estimatedDryingTime != null)
			{
				_estDryingTimeText.Text = _estimatedDryingTime;
			}
		}

		async void getWeatherData()
		{
			weatherData = await RetrieveWeatherData.FetchWeatherAsync(_latitude, _longitude);
			if (weatherData != null)
			{
				updateWeatherData(weatherData);
				Console.Out.WriteLine("Just before DataTransformers.parseWeatherData");
				string[,] parsedWeatherData = DataTransformers.parseWeatherData(weatherData);
				Console.Out.WriteLine("parsedWeatherData: " + parsedWeatherData);
				RetrieveServerData.getCurrentDryingTime(parsedWeatherData);
			}
		}

		private void updateWeatherData(JsonValue json)
		{
			_currentWeather = json["weather"][0]["main"].ToString().Replace("\"","");
			_currentWeather = _currentWeather.First().ToString().ToUpper() + _currentWeather.Substring(1);
			_currentTemperature = json["main"]["temp"].ToString() + " Celsius";
			_currentWind = json["wind"]["speed"].ToString() + " m/s";
			_currentHumidity = json["main"]["humidity"].ToString() + "%";
			Context mContext = Android.App.Application.Context;
			AppPreferences ap = new AppPreferences(mContext);
			_estimatedDryingTime = ap.getCurrentDryingTime();
		}

		public void getUIElements()
		{
			//_locationText = rootView.FindViewById<TextView>(Resource.Id.CurrentLocationText);
			//_addressText = rootView.FindViewById<TextView>(Resource.Id.AddressText);
			_weatherText = rootView.FindViewById<TextView>(Resource.Id.WeatherText);
			_temperatureText = rootView.FindViewById<TextView>(Resource.Id.TemperatureText);
			_windText = rootView.FindViewById<TextView>(Resource.Id.WindText);
			_humidityText = rootView.FindViewById<TextView>(Resource.Id.HumidityText);
			_estDryingTimeText = rootView.FindViewById<TextView>(Resource.Id.EstDryingText);
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
					//_address = extras.GetString("Address");
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
