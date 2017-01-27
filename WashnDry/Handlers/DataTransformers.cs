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


namespace WashnDry
{
	public static class DataTransformers
	{
		public static string sparseArrayToString(SparseBooleanArray mArray)
		{
			String mString = "";
			for (var i = 0; i < mArray.Size(); i++)
				mString += mArray.ValueAt(i) + ",";

			return mString.TrimEnd(',');
		}

		public static List<bool> stringToBooleanList(string mString)
		{
			List<bool> boolList = new List<bool>();
			List<string> strList = mString.Split(',').ToList(); ;
			foreach (string s in strList)
			{
				if (s == "True") { boolList.Add(true); }
				else { boolList.Add(false); }

			}
			return boolList;
		}

		public enum TimeFormat { verbose, digital };
		public static string formatSecondsToTime(int unformattedTimeInSeconds, TimeFormat format)
		{
			string formattedTime;

			int timeInHours = unformattedTimeInSeconds / 3600;
			int timeInSeconds = unformattedTimeInSeconds % 60;
			int timeInMinutes = (unformattedTimeInSeconds % 3600) / 60;

			if (format == TimeFormat.digital)
			{
				formattedTime = timeInHours.ToString("D2") + ":" + timeInMinutes.ToString("D2") + ":" + timeInSeconds.ToString("D2");
			}
			else if (format == TimeFormat.verbose)
			{
				string hours = "", minutes = "", seconds = "";
				if (timeInHours != 0) { hours = timeInHours.ToString() + " " + "Hour(s)" + " "; }
				if (timeInMinutes != 0) { minutes = timeInMinutes.ToString() + " " + "Minute(s)" + " "; }
				if (timeInSeconds != 0) { seconds = timeInSeconds.ToString() + " "  + "Second(s)" + " "; }
				formattedTime = hours + minutes + seconds;
			}
			else { formattedTime = "Please select a format"; }
			return formattedTime;
		}

	}
}	