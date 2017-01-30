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
		public System.Threading.Timer toNextDryTimer;
		public System.Threading.Timer dryingTimer;
		static int timeLeftInSeconds;
		int initialTimeInSeconds;

		enum State { ready, notReady, dryingInProgress, laundryFinished };
		static State state = State.notReady;

		TextView nextLaundryTV, timeToNextLaundryTV;
		TextView timerTextView, estTextView, timeTakenTextView;
		Button startDryingButton, restartDryingButton, stopDryingButton;

		DateTime nextLaundryDate;
		DateTime currentDate;

		DateCalculator dc = new DateCalculator();
		DateCalculator nextLaundryDc = new DateCalculator();

		TextView descriptionText;

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

			estTextView 			= rootView.FindViewById<TextView>(Resource.Id.estTime);
			nextLaundryTV 			= rootView.FindViewById<TextView>(Resource.Id.nextLaundryTextView);
			timeToNextLaundryTV 	= rootView.FindViewById<TextView>(Resource.Id.timeToNextLaundryTextView);
			startDryingButton 		= rootView.FindViewById<Button>(Resource.Id.startDryingButton);
			stopDryingButton 		= rootView.FindViewById<Button>(Resource.Id.stopDryingButton);
			restartDryingButton 	= rootView.FindViewById<Button>(Resource.Id.restartDryingButton);
			descriptionText 		= rootView.FindViewById<TextView>(Resource.Id.laundryDoneAlert);
			timerTextView 			= rootView.FindViewById<TextView>(Resource.Id.timerTextView);
			timeTakenTextView	 	= rootView.FindViewById<TextView>(Resource.Id.timeTaken);
			nextLaundryDate 		= new DateTime(2016, 1, 29, 21, 56, 59);
			currentDate 			= DateTime.Now;
			nextLaundryDc.storeDiffBetweenDates(nextLaundryDate, currentDate);
			initialTimeInSeconds = 17; // should retrieve this value from the app's calculations
			dc.formatSeconds(initialTimeInSeconds);
			//uiOnDateDifference();
			//uiEventsOnReadyToStartDrying();



			//startDryingButton.Click 	+= startDryingHandler;
			//restartDryingButton.Click 	+= restartDryingHandler;
			//stopDryingButton.Click 		+= stopDryingHandler;
			//RegisterBroadcastReceiver();

			// should save state in a store. Various events should trigger a change in the state
			if (state == State.notReady) { }
			if (state == State.ready) { uiEventsOnReadyToStartDrying();}
			if (state == State.dryingInProgress) { uiEventsOnStartDrying();}
			if (state == State.laundryFinished) { uiEventsOnFinishedDrying(); }

			nextLaundryTV.Text = nextLaundryDate.ToString();
			estTextView.Text = dc.verbose.All;



			locationTrigger();
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

			if (state == State.dryingInProgress){ dryingTimer = new System.Threading.Timer(dryTimer_Elapsed, null, 0, 1000); }
			if (nextLaundryDc.Hours <= 4 && nextLaundryDc.Seconds >= 0 )
			{
				int toNextDryTimerInterval;
				if (nextLaundryDc.Hours >= 3) { toNextDryTimerInterval = 300000; Console.WriteLine("3 hours");} // update UI every 5 minutes
				else if (nextLaundryDc.Minutes >= 60) { toNextDryTimerInterval = 60000; Console.WriteLine("hour");} // update UI every minute
				else if (nextLaundryDc.Seconds >= 60) { toNextDryTimerInterval = 1000; Console.WriteLine("minute");} //update UI every second
				else { toNextDryTimerInterval = 1000; }
				toNextDryTimer = new System.Threading.Timer(ToNextDryTimer_Elapsed, null, 0, toNextDryTimerInterval);
			}
		}

		void ToNextDryTimer_Elapsed(object sender)
		{
			
			Console.WriteLine("to next drying event timer ticking");
			if (nextLaundryDc.Seconds <= 0)
			{
				toNextDryTimer.Dispose(); 
				toNextDryTimer = null;
			}

			Activity.RunOnUiThread(() =>
			{
				
				uiOnDateDifference();
			});
		}

		void dryTimer_Elapsed(object objState)
		{
			Console.WriteLine("dry timer elapsed ticking");
			if (state == State.laundryFinished)
			{
				Console.WriteLine("laundry finished timer message liao");
				stopTimerBroadcast();
				Activity.RunOnUiThread(() =>
				{
					Toast.MakeText(Activity, "Finished Drying", ToastLength.Short).Show();
					uiEventsOnFinishedDrying();
				});

				// create push notification
				Notification.Builder builder = new Notification.Builder(Activity).SetContentTitle("Finished Drying").SetContentText("Time finished is").SetSmallIcon(Resource.Drawable.i_splash);
				Notification notification = builder.Build();
				NotificationManager notificationManager = Activity.GetSystemService(Context.NotificationService) as NotificationManager;
				const int notificationId = 0;
				notificationManager.Notify(notificationId, notification);
				return;

			}
			else if (state == State.dryingInProgress)
			{
				if (dryingTimer != null) // context guard against null reference exception
				{
					dc.formatSeconds(timeLeftInSeconds);
					Activity.RunOnUiThread(() =>
					{
						timerTextView.Text = dc.Digital; // DataTransformers.formatSecondsToTime(timeLeftInSeconds, DataTransformers.TimeFormat.digital);
					});
				}
			}

		}

		void startDryingHandler(object sender, EventArgs e)
		{
			Toast.MakeText(Activity, "Started Drying now!", ToastLength.Short).Show();
			state = State.dryingInProgress;
			dryingTimer = new System.Threading.Timer(dryTimer_Elapsed, null, 0, 1000);
			uiEventsOnStartDrying();
			startTimerBroadcast(initialTimeInSeconds);

		}

		void stopDryingHandler(object sender, EventArgs e)
		{
			state = State.ready;
			Toast.MakeText(Activity, "Stopped Drying.", ToastLength.Short).Show();
			uiEventsOnStoppedDrying();
			stopTimerBroadcast();
		}

		void restartDryingHandler(object sender, EventArgs e)
		{
			state = State.dryingInProgress;
			Toast.MakeText(Activity, "Restarted Drying.", ToastLength.Short).Show();
			Activity.StopService(new Intent(Activity, typeof(TimerService)));
			startTimerBroadcast(initialTimeInSeconds);

		}

		void stopTimerBroadcast()
		{
			Activity.StopService(new Intent(Activity, typeof(TimerService)));
			if (dryingTimer != null)
			{
				Console.WriteLine("timer destroyed");
				dryingTimer.Dispose();
				dryingTimer = null;
			}
		}

		void startTimerBroadcast(int initialTime)
		{
			Intent BroadcastIntent = new Intent(Activity, typeof(TimerService.TimerBroadcastReceiver));
			string action = "sendInitialTimeInSeconds";
			BroadcastIntent.SetAction(action);
			BroadcastIntent.AddCategory(Intent.CategoryDefault);
			BroadcastIntent.PutExtra("initialTimeInSeconds", initialTime);
			Activity.SendBroadcast(BroadcastIntent); //when this broadcast is received, it triggers the start of the TimerService
		}

		void uiOnDateDifference()
		{
			currentDate = DateTime.Now;
			nextLaundryDc.storeDiffBetweenDates(nextLaundryDate, currentDate);
			nextLaundryDc.formatSeconds(nextLaundryDc.Seconds);
			if (nextLaundryDc.Days >= 1) { timeToNextLaundryTV.Text = nextLaundryDc.verbose.MonthDay;  }
			else if (nextLaundryDc.Minutes >= 180) { timeToNextLaundryTV.Text = nextLaundryDc.verbose.DayHour; }
			else if (nextLaundryDc.Minutes >= 1) { timeToNextLaundryTV.Text = nextLaundryDc.verbose.HourMin; }
			else if (nextLaundryDc.Seconds >= 1) { timeToNextLaundryTV.Text = nextLaundryDc.verbose.Sec; }
			else if (nextLaundryDc.Minutes >= -60)
			{
				timeToNextLaundryTV.Text = "Begin drying now for optimal results!";
				uiEventsOnReadyToStartDrying();
			}
			else { timeToNextLaundryTV.Text = "You have missed the recommended timing by over an hour."; }

		}


		void uiEventsNotReadyToStartDrying()
		{
			startDryingButton.Visibility = ViewStates.Gone;
			stopDryingButton.Visibility = ViewStates.Gone;
			restartDryingButton.Visibility = ViewStates.Gone;
			timerTextView.Visibility = ViewStates.Gone;
			descriptionText.Visibility = ViewStates.Gone;
			timeTakenTextView.Visibility = ViewStates.Gone;
		}

		void uiEventsOnReadyToStartDrying()
		{
			dc.formatSeconds(initialTimeInSeconds);
			startDryingButton.Visibility 	= ViewStates.Visible;
			stopDryingButton.Visibility 	= ViewStates.Gone;
			restartDryingButton.Visibility 	= ViewStates.Gone;
			timerTextView.Visibility 		= ViewStates.Gone;
			descriptionText.Text 			= "Press start to start drying";
			descriptionText.Visibility 		= ViewStates.Visible;
			timeTakenTextView.Text = "Time to dry: \n" + dc.verbose.All; //DataTransformers.formatSecondsToTime(initialTimeInSeconds, DataTransformers.TimeFormat.verbose);
			timeTakenTextView.Visibility 	= ViewStates.Gone;
		}

		void uiEventsOnStartDrying()
		{
			startDryingButton.Visibility 	= ViewStates.Gone;
			restartDryingButton.Visibility 	= ViewStates.Visible;
			stopDryingButton.Visibility 	= ViewStates.Visible;
			timerTextView.Visibility 		= ViewStates.Visible;
			descriptionText.Visibility 		= ViewStates.Gone;
			timeTakenTextView.Visibility 	= ViewStates.Gone;

		}

		void uiEventsOnFinishedDrying()
		{
			dc.formatSeconds(initialTimeInSeconds);
			startDryingButton.Visibility 	= ViewStates.Invisible;
			stopDryingButton.Visibility 	= ViewStates.Gone;
			restartDryingButton.Visibility 	= ViewStates.Gone;
			timerTextView.Visibility 		= ViewStates.Gone;
			descriptionText.Text 			= "Laundry Completed. Based on your personal schedule, the app will suggest the next suitable date to start your laundry again :)";
			descriptionText.Visibility 		= ViewStates.Visible;
			timeTakenTextView.Text 			= "Time taken to dry: \n" + dc.verbose.All;
			timeTakenTextView.Visibility 	= ViewStates.Visible;
		}

		void uiEventsOnStoppedDrying()
		{
			dc.formatSeconds(initialTimeInSeconds);
			startDryingButton.Visibility = ViewStates.Visible;
			stopDryingButton.Visibility = ViewStates.Gone;
			restartDryingButton.Visibility = ViewStates.Gone;
			timerTextView.Visibility = ViewStates.Gone;
			descriptionText.Text = "Stopped. Press start to start drying \n";
			descriptionText.Visibility = ViewStates.Visible;
			timeTakenTextView.Text = "Time to dry: \n" + dc.verbose.All;
			timeTakenTextView.Visibility = ViewStates.Visible;
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
					if (extras.GetBoolean("isLaundryDone")) { state = State.laundryFinished; }
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
