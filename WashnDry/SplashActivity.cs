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
		}

		protected override void OnResume()
		{
			base.OnResume();

			Task startupWork = new Task(() =>
			{
				Task.Delay(1500);  // Simulate a bit of startup work.
			});

			// if user has not setup account, let user set up account here, else go straight to main
			Context mContext = Android.App.Application.Context;
			AppPreferences ap = new AppPreferences(mContext);
			string username = ap.getUsername();

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
