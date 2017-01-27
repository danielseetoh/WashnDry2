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
using System.Timers;


namespace WashnDry
{
	public class HomeFragment : Fragment, ILocationListener
	{
		public System.Threading.Timer cdtimer;
		public System.Timers.Timer mytimer;
		static int timeLeftInSeconds;
		int initialTimeInSeconds;
		LinearLayout timerWrapper;
		TextView timerTextView, timeTakenTextView;
		Button startDryingButton;
		static bool isLaundryDone;
		TextView laundryDoneAlert;

		TextView _addressText;
		TextView _locationText;
		TextView _temperatureText;
		TextView _windText;
		TextView _humidityText;
		TextView _weatherText;
		Location _currentLocation;
		LocationManager _locationManager;
		string _locationProvider;

		Context context;
		View rootView;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			rootView = inflater.Inflate(Resource.Layout.Home, container, false);

			startDryingButton = rootView.FindViewById<Button>(Resource.Id.startDryingButton);
			laundryDoneAlert = rootView.FindViewById<TextView>(Resource.Id.laundryDoneAlert);
			timerWrapper = rootView.FindViewById<LinearLayout>(Resource.Id.timerWrapper);
			timerTextView = rootView.FindViewById<TextView>(Resource.Id.timerTextView);
			timeTakenTextView = rootView.FindViewById<TextView>(Resource.Id.timeTaken);
			initialTimeInSeconds = 7; // should retrieve this value from the app's calculations

			startDryingButton.Click += startDryingHandler;
			//RegisterBroadcastReceiver();

			mytimer = new System.Timers.Timer();
			mytimer.Interval = 100;
			mytimer.Enabled = true;
			mytimer.Elapsed += TimerElapsedHandler; // Handler method to call when timer elapses every 100ms

			locationTrigger();
			return rootView;
		}

		void TimerElapsedHandler(object sender, ElapsedEventArgs e)
		{
			if (isLaundryDone) { Activity.RunOnUiThread(stopDryingHandler);  }
			else { 
				Activity.RunOnUiThread(() => { 
					timerTextView.Text = DataTransformers.formatSecondsToTime(timeLeftInSeconds, DataTransformers.TimeFormat.digital); 
				}); 
			}
		}

		void startDryingHandler(object sender, EventArgs e)
		{
			mytimer.Start(); // starts timer which then periodically upates the data by listening to the broadcast
			timerWrapper.Visibility = ViewStates.Visible;
			laundryDoneAlert.Visibility = ViewStates.Gone;

			Intent BroadcastIntent = new Intent(Activity, typeof(TimerService.TimerBroadcastReceiver));
			string action = "sendInitialTimeInSeconds";
			BroadcastIntent.SetAction(action);
			BroadcastIntent.AddCategory(Intent.CategoryDefault);
			BroadcastIntent.PutExtra("initialTimeInSeconds", initialTimeInSeconds);
			Activity.SendBroadcast(BroadcastIntent); //when this broadcast is received, it triggers the start of the TimerService
			//Activity.StartService(new Intent(Activity, typeof(TimerService)));

			Toast.MakeText(Activity, "Started Drying", ToastLength.Short).Show();
		}

		void stopDryingHandler()
		{
			Toast.MakeText(Activity, "Finished Drying", ToastLength.Short).Show();
			mytimer.Stop();
			laundryDoneAlert.Visibility = ViewStates.Visible;
			timerWrapper.Visibility = ViewStates.Gone;
			timeTakenTextView.Text = "Time Taken: \n" + DataTransformers.formatSecondsToTime(initialTimeInSeconds, DataTransformers.TimeFormat.verbose);
			timeTakenTextView.Visibility = ViewStates.Visible;
			Activity.StopService(new Intent(Activity, typeof(TimerService)));
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
					timeLeftInSeconds = extras.GetInt("timeLeftInSeconds");
					isLaundryDone = extras.GetBoolean("isLaundryDone");
				}

				else if (intent.Action == sendLocationBroadcast)
				{
					// get location data here
				}

			}
		}


/* ===================================================================================
============= I somehow think this is redundant to register the receiver =============
*/

		//private HomeBroadcastReceiver homeReceiver;

		//private void RegisterBroadcastReceiver()
		//{
		//	IntentFilter filter = new IntentFilter(HomeBroadcastReceiver.sendCountDownTimerData);
		//	filter.AddCategory(Intent.CategoryDefault);
		//	homeReceiver = new HomeBroadcastReceiver();
		//	this.Activity.RegisterReceiver(homeReceiver, filter);
		//}

		//private void UnRegisterBroadcastReceiver()
		//{
		//	this.Activity.UnregisterReceiver(homeReceiver);
		//}

		// Other Code that is not mine

		public void locationTrigger()
		{
			_locationText = rootView.FindViewById<TextView>(Resource.Id.CurrentLocationText);
			_addressText = rootView.FindViewById<TextView>(Resource.Id.AddressText);
			_weatherText = rootView.FindViewById<TextView>(Resource.Id.WeatherText);
			_temperatureText = rootView.FindViewById<TextView>(Resource.Id.TemperatureText);
			_windText = rootView.FindViewById<TextView>(Resource.Id.WindText);
			_humidityText = rootView.FindViewById<TextView>(Resource.Id.HumidityText);
			context = this.Activity;
			InitializeLocationManager();
			getLocation();
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
			//Log.Debug(TAG, "Using " + _locationProvider + ".");
			//Toast.MakeText(this.Activity, _locationProvider, ToastLength.Long).Show();
		}

		async void getLocation()
		{
			if (_currentLocation == null)
			{
				_addressText.Text = "Can't determine the current address. Try again in a few minutes.";
				return;
			}

			Address address = await ReverseGeocodeCurrentLocation();
			DisplayAddress(address);
		}

		async Task<Address> ReverseGeocodeCurrentLocation()
		{
			Geocoder geocoder = new Geocoder(this.Activity);
			IList<Address> addressList =
				await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

			Address address = addressList.FirstOrDefault();
			return address;
		}

		void DisplayAddress(Address address)
		{
			if (address != null)
			{
				StringBuilder deviceAddress = new StringBuilder();
				for (int i = 0; i < address.MaxAddressLineIndex; i++)
				{
					deviceAddress.AppendLine(address.GetAddressLine(i));
				}
				// Remove the last comma from the end of the address.
				_addressText.Text = deviceAddress.ToString();
			}
			else
			{
				_addressText.Text = "Unable to determine the address. Try again in a few minutes.";
			}
		}

		public async void OnLocationChanged(Location location) {
		    _currentLocation = location;
			if (_currentLocation == null)
			{
				_locationText.Text = "Unable to determine your location. Try again in a short while.";
			}
			else
			{
				_locationText.Text = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
				Address address = await ReverseGeocodeCurrentLocation();
				DisplayAddress(address);
				JsonValue json = await FetchWeatherAsync(_currentLocation);
				ParseAndDisplay(json);
				Task delay = new Task(() =>
				{
					Task.Delay(10000);  // Increase refresh time to 10 seconds
				});
			}


		}

		public void OnProviderDisabled(string provider) { }

		public void OnProviderEnabled(string provider) { }

		public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

		public async Task<JsonValue> FetchWeatherAsync(Location location)
		{
			string url = "http://api.openweathermap.org/data/2.5/weather?appid=f30fd8bd2d1f9f1bbdfbd627f9faa54b&lat=" + location.Latitude.ToString() + "&lon=" + location.Longitude.ToString()+ "&units=metric";
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
			request.ContentType = "application/json";
			request.Method = "GET";

			// Send the request to the server and wait for the response:
			using (WebResponse response = await request.GetResponseAsync())
			{
				// Get a stream representation of the HTTP web response:
				using (Stream stream = response.GetResponseStream())
				{
					// Use this stream to build a JSON document object:
					JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
					Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
					//Toast.MakeText(this.Activity, jsonDoc.ToString(), ToastLength.Long).Show();
					//_temperatureText.Text = string.Format("Response: {0}", jsonDoc.ToString());
					// Return the JSON document:
					return jsonDoc;
				}
			}
			//_temperatureText.Text = string.Format("Response: {0}", jsonDoc.ToString());
			//_windText.Text = string.Format("{0:f6},{1:f6}", location.Latitude, location.Longitude);
			//_humidityText.Text = string.Format("{0:f6},{1:f6}", location.Latitude, location.Longitude);
		}

		private void ParseAndDisplay(JsonValue json)
		{
			// Extract the array of name/value results for the field name "weatherObservation". 
			_weatherText.Text = "Weather: " + json["weather"][0]["description"].ToString();
			_temperatureText.Text = "Temperature: " + json["main"]["temp"].ToString() + " Celsius";
			_windText.Text = "Wind Speed: " + json["wind"]["speed"].ToString() + "m/s";
			_humidityText.Text = "Humidity: " + json["main"]["humidity"].ToString() + "%";
		}
	}
}
