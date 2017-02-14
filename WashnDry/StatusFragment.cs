using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Java.Util;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Speech;
using Android.Locations;
using System.Threading.Tasks;
using Plugin.Geolocator;
using System.Net;
using System.Json;
using System.IO;
using System.Timers;
using IO.Github.Krtkush.Lineartimer;
using DT = System.Data;            // System.Data.dll  
using QC = System.Data.SqlClient;

namespace WashnDry
{
	public class StatusFragment : Fragment
	{
		public System.Threading.Timer toNextDryTimer;
		public System.Threading.Timer dryingTimer;
		static int timeLeftInSeconds;
		int initialTimeInSeconds;

		enum State { dateNotChosen, ready, notReady, dryingInProgress, laundryFinished };
		static State state = State.notReady;

		TextView nextLaundryButton, timeToNextLaundryTV;
		TextView timerTextView, estTextView;
		Button startDryingButton, restartDryingButton, stopDryingButton;


		LinearLayout afterStartDryingWrapper, beforeStartDryingWrapper;
		//RelativeLayout timerLayoutWrapper;
		LinearTimer linearTimer;
		LinearTimerView linearTimerView;

		DateTime nextLaundryDate;
		DateTime currentDate;

		DateCalculator dc = new DateCalculator();
		DateCalculator nextLaundryDc = new DateCalculator();

		AppPreferences ap;

		View rootView;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			//var r = DB.DBOperation(DB.sql.selectq, "SELECT * FROM WASHNDRYCUSTOMER1");


		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			rootView = inflater.Inflate(Resource.Layout.Status, container, false);
			RetrieveWeatherData.updateFiveDayWashDatesAndThreeBestTimings();
			RetrieveCalendarData.getAndroidCalendarData(this.Activity);
			ap = new AppPreferences(Activity);
			initialTimeInSeconds = ap.getEstimatedTime();

			estTextView = rootView.FindViewById<TextView>(Resource.Id.estTime);
			nextLaundryButton = rootView.FindViewById<Button>(Resource.Id.nextLaundryButton);
			timeToNextLaundryTV = rootView.FindViewById<TextView>(Resource.Id.timeToNextLaundryTextView);
			startDryingButton = rootView.FindViewById<Button>(Resource.Id.startDryingButton);
			stopDryingButton = rootView.FindViewById<Button>(Resource.Id.stopDryingButton);
			restartDryingButton = rootView.FindViewById<Button>(Resource.Id.restartDryingButton);

			beforeStartDryingWrapper = rootView.FindViewById<LinearLayout>(Resource.Id.beforeStartDryingWrapper);
			afterStartDryingWrapper = rootView.FindViewById<LinearLayout>(Resource.Id.afterStartDryingWrapper);

			timerTextView = rootView.FindViewById<TextView>(Resource.Id.timerTextView);

			linearTimerView = rootView.FindViewById<LinearTimerView>(Resource.Id.linearTimerView);
			linearTimer = new LinearTimer(linearTimerView);

			nextLaundryButton.Click += NextLaundryButton_Click;;
			startDryingButton.Click += startDryingHandler;
			restartDryingButton.Click += restartDryingHandler;
			stopDryingButton.Click += stopDryingHandler;
			//RegisterBroadcastReceiver();

			// should save state in a store. Various events should trigger a change in the state
			if (state == State.dateNotChosen) { 
				var intent = new Intent(Activity, typeof(LaundrySelectActivity));
				StartActivityForResult(intent, 100);
			}
			if (state == State.notReady) { }
			if (state == State.ready) { uiEventsOnReadyToStartDrying(); }
			if (state == State.dryingInProgress) { uiEventsOnStartDrying(); }
			if (state == State.laundryFinished) { uiEventsOnFinishedDrying(); }

			uiSelectLaundryDateButton();

			if (ap.getEstimatedTime() != 0) { setEstimatedTime(initialTimeInSeconds);}

			currentDate = DateTime.Now;
			nextLaundryDc.storeDiffBetweenDates(nextLaundryDate, currentDate);
			uiCountDownToNextLaundry();
			uiEventsOnReadyToStartDrying();



			return rootView;
		}

		public override void OnPause()
		{
			base.OnPause();
			if (dryingTimer != null)
			{
				dryingTimer.Dispose();
				dryingTimer = null;
			}
			if (toNextDryTimer != null)
			{
				toNextDryTimer.Dispose();
				toNextDryTimer = null;
			}
		}

		public override void OnResume()
		{
			base.OnResume();

			if (state == State.dryingInProgress) { dryingTimer = new System.Threading.Timer(dryTimer_Elapsed, null, 0, 1000); }
			if (nextLaundryDc.Hours <= 4 && nextLaundryDc.Seconds >= 0)
			{
				int toNextDryTimerInterval;
				if (nextLaundryDc.Hours >= 3) { toNextDryTimerInterval = 300000; Console.WriteLine("3 hours"); } // update UI every 5 minutes
				else if (nextLaundryDc.Minutes >= 60) { toNextDryTimerInterval = 60000; Console.WriteLine("hour"); } // update UI every minute
				else if (nextLaundryDc.Seconds >= 60) { toNextDryTimerInterval = 1000; Console.WriteLine("minute"); } //update UI every second
				else { toNextDryTimerInterval = 1000; }
				toNextDryTimer = new System.Threading.Timer(ToNextDryTimer_Elapsed, null, 0, toNextDryTimerInterval);
			}
		}

	


		void NextLaundryButton_Click(object sender, EventArgs e)
		{
			var intent = new Intent(Activity, typeof(LaundrySelectActivity));
			StartActivityForResult(intent, 100);
		}

		public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok && requestCode == 100)
			{
				setNextLaundryDate(data.GetStringExtra("selectedLaundryDate"));
				setEstimatedTime(data.GetIntExtra("estimatedTime", 0));
				uiCountDownToNextLaundry();
			}
		}

		void ToNextDryTimer_Elapsed(object sender)
		{
			if (nextLaundryDc.Seconds <= 0)
			{
				toNextDryTimer.Dispose();
				toNextDryTimer = null;
			}

			Activity.RunOnUiThread(() =>
			{
				uiCountDownToNextLaundry();
			});
		}

		void dryTimer_Elapsed(object objState)
		{
			if (state == State.laundryFinished)
			{
				stopTimerBroadcast();
				linearTimer.ResetTimer();

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
			uiEventsOnStartDrying();
			dryingTimer = new System.Threading.Timer(dryTimer_Elapsed, null, 0, 1000);
			linearTimer.StartTimer(360, initialTimeInSeconds*1000);
			startTimerBroadcast(initialTimeInSeconds);
		}

		void stopDryingHandler(object sender, EventArgs e)
		{
			state = State.ready;
			Toast.MakeText(Activity, "Stopped Drying", ToastLength.Short).Show();
			//linearTimer.RestartTimer();
			linearTimer.ResetTimer();
			uiEventsOnStoppedDrying();
			stopTimerBroadcast();
		}

		void restartDryingHandler(object sender, EventArgs e)
		{
			state = State.dryingInProgress;
			Toast.MakeText(Activity, "Restarted Drying.", ToastLength.Short).Show();
			Activity.StopService(new Intent(Activity, typeof(TimerService)));
			linearTimer.RestartTimer();
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

		public void prompSpeechInput()
		{
			Intent i = new Intent(RecognizerIntent.ActionRecognizeSpeech);
			i.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
			i.PutExtra(RecognizerIntent.ExtraLanguage, Locale.Default);
			i.PutExtra(RecognizerIntent.ExtraPrompt, "Say Something");		
		}

		void setNextLaundryDate(string s)
		{
			ap.saveSelectedNextLaundryTime(s);
			var dt = DateTime.Parse(s);
			nextLaundryDate = dt;
			var timeStr = string.Format(" {0}:{1}HRS ", dt.Hour, dt.Minute);
			nextLaundryButton.Text = dt.ToLongDateString() + timeStr;
		}

		void setEstimatedTime(int i)
		{
			initialTimeInSeconds = i;
			TimeSpan t = TimeSpan.FromSeconds(i);
			string h="", m="", s="", str="";
			if (t.Hours > 0) h = string.Format("{0} Hours(s)", t.Hours);
			if (t.Minutes > 0) m = string.Format("{0} Minutes(s)", t.Hours);
			s = string.Format("{0} Second(s)", t.Seconds);
			str = h + m + s;
			estTextView.Text = str;
		}

		void uiSelectLaundryDateButton()
		{
			if (ap.getSelectedNextLaundryTime() != "") { setNextLaundryDate(ap.getSelectedNextLaundryTime()); }
			else { 
				nextLaundryButton.Text = "Select Next Laundry Session";

			}
		}

		void uiCountDownToNextLaundry()
		{
			currentDate = DateTime.Now;

			if (ap.getSelectedNextLaundryTime() == "") { timeToNextLaundryTV.Text = "Please select a date to dry your laundry"; }
			else {
				nextLaundryDc.storeDiffBetweenDates(nextLaundryDate, currentDate);
				nextLaundryDc.formatSeconds(nextLaundryDc.Seconds);
				if (nextLaundryDc.Days >= 1) { timeToNextLaundryTV.Text = "3 hours" + nextLaundryDc.verbose.MonthDay; }
				else if (nextLaundryDc.Minutes >= 180) { timeToNextLaundryTV.Text = "hi"; Console.WriteLine("Minutes more than 180"); }//nextLaundryDc.verbose.DayHour; }
				else if (nextLaundryDc.Minutes >= 1) { timeToNextLaundryTV.Text = nextLaundryDc.verbose.HourMin; }
				else if (nextLaundryDc.Seconds >= 1) { timeToNextLaundryTV.Text = nextLaundryDc.verbose.Sec; }
				else if (nextLaundryDc.Minutes >= -60)
				{
					timeToNextLaundryTV.Text = "Begin drying now for optimal results!";
					canStartLaundryDialog();
					uiEventsOnReadyToStartDrying();
				}
				else { timeToNextLaundryTV.Text = "You have missed the recommended timing by over an hour."; }
			}
}


		void uiEventsNotReadyToStartDrying()
		{
			beforeStartDryingWrapper.Visibility = ViewStates.Visible;
			afterStartDryingWrapper.Visibility = ViewStates.Gone;
		}

		void uiEventsOnReadyToStartDrying()
		{
			dc.formatSeconds(initialTimeInSeconds);
			beforeStartDryingWrapper.Visibility = ViewStates.Visible;
			afterStartDryingWrapper.Visibility = ViewStates.Gone;
		}

		void uiEventsOnStartDrying()
		{
			beforeStartDryingWrapper.Visibility = ViewStates.Gone;
			afterStartDryingWrapper.Visibility = ViewStates.Visible;
			dc.formatSeconds(initialTimeInSeconds);
			timerTextView.Text = dc.Digital;
			linearTimerView.PreFillAngle = 0;
		}

		void uiEventsOnFinishedDrying()
		{
			state = State.dateNotChosen;
			dc.formatSeconds(initialTimeInSeconds);
			ap.saveSelectedNextLaundryTime("");
			uiSelectLaundryDateButton();
			uiCountDownToNextLaundry();
			beforeStartDryingWrapper.Visibility = ViewStates.Visible;
			afterStartDryingWrapper.Visibility = ViewStates.Gone;
			finishLaundryDialog();
		}

		void uiEventsOnStoppedDrying()
		{
			dc.formatSeconds(initialTimeInSeconds);
			beforeStartDryingWrapper.Visibility = ViewStates.Visible;
			afterStartDryingWrapper.Visibility = ViewStates.Gone;
		}

		// Dialogs
		void canStartLaundryDialog()
		{
			View view = Activity.LayoutInflater.Inflate(Resource.Layout.Dialog_ReadyToStartDrying, null);
			AlertDialog builder = new AlertDialog.Builder(Activity).Create();
			builder.SetView(view);
			builder.SetCanceledOnTouchOutside(false);
			Button button = view.FindViewById<Button>(Resource.Id.cancel);
			button.Click += delegate
			{
				builder.Dismiss();
			};
			builder.Show();
		}


		void finishLaundryDialog()
		{
			View view = Activity.LayoutInflater.Inflate(Resource.Layout.Dialog_FinishDrying, null);
			AlertDialog builder = new AlertDialog.Builder(Activity).Create();
			builder.SetView(view);
			builder.SetCanceledOnTouchOutside(false);

			Button cancelButton = view.FindViewById<Button>(Resource.Id.cancel);
			Button submitButton = view.FindViewById<Button>(Resource.Id.submit);
			RatingBar ratingBar = view.FindViewById<RatingBar>(Resource.Id.ratingBar);
			ratingBar.Rating = 3;
			cancelButton.Click += delegate
			{
				builder.Dismiss();
			};

			submitButton.Click += delegate {
				builder.Dismiss();
				double r = ratingBar.Rating;

				RatingData.getScaleData();

				double correctedDryingTime = initialTimeInSeconds * RatingData.scale[r];
				Console.WriteLine("corrected dry time" + correctedDryingTime);

				string ins = "INSERT INTO WASHNDRYCUSTOMER1 (TEMPERATURE, HUMIDITY, PRECIPITATION, WINDSPEED, DRYING_TIME ) VALUES (27.8, 48.2, 0.64, 32.1, 2345)";
				//DB.DBOperation(DB.sql.insert, ins);

			};
			builder.Show();

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

	}
}
