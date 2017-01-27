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
	public class MyCountDownTimer
	{
		private long initialTimeInSeconds;
		TextView hours, minutes, seconds;
		long timeInHours, timeInMinutes, timeInSeconds;

		public MyCountDownTimer(long _initialTimeInSeconds, TextView _hours, TextView _minutes, TextView _seconds)
		{
			this.initialTimeInSeconds = _initialTimeInSeconds;
			this.hours = _hours;
			this.minutes = _minutes;
			this.seconds = _seconds;
		}

		public void displayTime()
		{
			initialTimeInSeconds -= 1;
			timeInHours = initialTimeInSeconds / 3600;
			timeInSeconds = initialTimeInSeconds % 60;
			timeInMinutes = (initialTimeInSeconds % 3600) / 60;
			hours.Text = timeInHours.ToString("D2") + ":";
			minutes.Text = timeInMinutes.ToString("D2") + ":";
			seconds.Text = timeInSeconds.ToString("D2");
		}

		public bool isTimerDone()
		{
			if (initialTimeInSeconds <= 0) { return true; }
			else { return false;}
		}


	}
}
