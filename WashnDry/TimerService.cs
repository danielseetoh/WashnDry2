using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading;


using Android.Locations;

namespace WashnDry
{

	[Service]
	public class TimerService : Service
	{
		Timer timer;
		static int initialTimeInSeconds=10;
		int timeInHours, timeInMinutes, timeInSeconds;



		public override void OnCreate()
		{
			base.OnCreate();
		}

		public override IBinder OnBind(Intent intent) { return null; }

		public override void OnDestroy()
		{
			base.OnDestroy();
			timer.Dispose();
			timer = null;

		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			timer = new Timer(HandleTimerCallback, initialTimeInSeconds, 0, 1000);
			return StartCommandResult.NotSticky;
		}

		private void HandleTimerCallback(object state)
		{	
			initialTimeInSeconds -= 1;
			timeInHours = initialTimeInSeconds / 3600;
			timeInSeconds = initialTimeInSeconds % 60;
			timeInMinutes = (initialTimeInSeconds % 3600) / 60;
			if (initialTimeInSeconds >= 0) { BroadcastStarted(false); }
			else { BroadcastStarted(true); }
		}

		private void BroadcastStarted(bool isLaundryDone)
		{
			Intent BroadcastIntent = new Intent(this, typeof(HomeFragment.HomeBroadcastReceiver));
			string action = "SendCountDownTimerData";
			BroadcastIntent.SetAction(action);
			BroadcastIntent.AddCategory(Intent.CategoryDefault);
			BroadcastIntent.PutExtra("timeLeftInSeconds", initialTimeInSeconds);
			BroadcastIntent.PutExtra("timeInHours", timeInHours);
			BroadcastIntent.PutExtra("timeInMinutes", timeInMinutes);
			BroadcastIntent.PutExtra("timeInSeconds", timeInSeconds);
			BroadcastIntent.PutExtra("isLaundryDone", isLaundryDone);
			SendBroadcast(BroadcastIntent);

		}

		[BroadcastReceiver]
		public class TimerBroadcastReceiver : BroadcastReceiver
		{
			private static readonly string sendInitialTimeInSeconds = "sendInitialTimeInSeconds";

			public override void OnReceive(Context context, Intent intent)
			{
				if (intent.Action == sendInitialTimeInSeconds) // There can be multipe broadcast received. This is the general listener. check whether the event received is the one which starts the countdown
				{
					Bundle extras = intent.Extras;
					initialTimeInSeconds = extras.GetInt("initialTimeInSeconds");
					context.StartService(new Intent(context, typeof(TimerService)));
				}



			}
		}

	}
}