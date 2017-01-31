using Android.Support.V4;
using Android.Support.V7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Util;
using System.Threading.Tasks;

namespace WashnDry
{
	[Activity(Theme = "@style/MyTheme.Splash", MainLauncher=true, NoHistory = true)]
	public class SplashActivity : AppCompatActivity
	{
		static readonly string TAG = "X:" + typeof(SplashActivity).Name;

		public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
		{
			base.OnCreate(savedInstanceState, persistentState);
			Log.Debug(TAG, "SplashActivity.OnCreate");

			//var retrieveLocationServiceIntent = new Intent(this, typeof(RetrieveLocationService));
			//var retrieveLocationServiceConnection = new RetrieveLocationServiceConnection(this);
			//BindService(retrieveLocationServiceIntent, retrieveLocationServiceConnection, Bind.AutoCreate);


			//StartService(new Intent(this, typeof(SimpleStartedService)));
			//RegisterBroadcastReceiver();
		}

		protected override void OnResume()
		{
			base.OnResume();
			StartService(new Intent(this, typeof(RetrieveLocationService)));
			Task startupWork = new Task(() =>
			{
				Task.Delay(3000);  // Simulate a bit of startup work.
			});

			// if user has not setup account, let user set up account here, else go straight to main
			Context mContext = Android.App.Application.Context;
			AppPreferences ap = new AppPreferences(mContext);
			string username = ap.getUsername();

			//startupWork.ContinueWith(t =>
			//	{
			//		StartActivity(new Intent(Application.Context, typeof(SetupActivity)));
			//	}, TaskScheduler.FromCurrentSynchronizationContext());

			if (username == "")
			{
				startupWork.ContinueWith(t =>
				{
					StartActivity(new Intent(Application.Context, typeof(SetupActivity)));
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}
			else 
			{
				startupWork.ContinueWith(t =>
				{
					StartActivity(new Intent(Application.Context, typeof(MainActivity)));
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}

			startupWork.Start();
		}

	}
}
