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

		public enum TimeFormat { verbose, verbose_DH, verbose_HM, verbose_MS, digital };
		public static string formatSecondsToTime(long unformattedTimeInSeconds, TimeFormat format)
		{
			string formattedTime;

			long timeInDays = unformattedTimeInSeconds / 86400;
			long timeInHours 	= unformattedTimeInSeconds / 3600;
			long timeInMinutes 	= (unformattedTimeInSeconds % 3600) / 60;
			long timeInSeconds = unformattedTimeInSeconds % 60;

			if (format == TimeFormat.digital)
			{
				formattedTime = timeInHours.ToString("D2") + ":" + timeInMinutes.ToString("D2") + ":" + timeInSeconds.ToString("D2");
			}
			else { 
				string days = "", hours = "", minutes = "", seconds = "";
				if (timeInDays != 0) { hours = timeInDays.ToString() + " " + "Month(s)" + " "; }
				if (timeInDays != 0) { hours = timeInDays.ToString() + " " + "Day(s)" + " "; }
				if (timeInHours != 0) { hours = timeInHours.ToString() + " " + "Hour_(s)" + " "; }
				if (timeInMinutes != 0) { minutes = timeInMinutes.ToString() + " " + "Minute_(s)" + " "; }
				if (timeInSeconds != 0) { seconds = timeInSeconds.ToString() + " " + "Second_(s)" + " "; }

				if (format == TimeFormat.verbose_DH) { formattedTime = days + hours; }
				else if (format == TimeFormat.verbose_HM) { formattedTime = days + hours; }
				else if (format == TimeFormat.verbose_MS) { formattedTime = minutes + seconds; }
				else { formattedTime = days + hours + minutes + seconds;}
			}
			return formattedTime;
		}

		public struct DateDifference {
			public long months;
			public long monthDiff;
			public long days;
			public long dayDiff;
			public long hours;
			public long hourDiff;
			public long minutes;
			public long minDiff;
			public long seconds;
			public long secondDiff;

		}
		public static DateDifference diffBetweenDates(DateTime laterDate, DateTime earlyDate)
		{
			DateDifference dateDiff = new DateDifference();

			long daysInCurrentMonth;
			if (earlyDate.Month == 1 || earlyDate.Month == 3 || earlyDate.Month == 5 || earlyDate.Month == 7 || earlyDate.Month == 8 || earlyDate.Month == 10 || earlyDate.Month == 12)
				daysInCurrentMonth = 31;
			else if (earlyDate.Month == 2) daysInCurrentMonth = 28;
			else daysInCurrentMonth = 30;

			dateDiff.monthDiff 	= laterDate.Month - earlyDate.Month;
			dateDiff.hourDiff 	= laterDate.Hour - earlyDate.Hour;
			dateDiff.minDiff 	= laterDate.Minute - earlyDate.Minute;
			dateDiff.secondDiff = laterDate.Second - earlyDate.Second;

			//if (dateDiff.minDiff < 0)
			//{
			//	if (dateDiff.hourDiff >= 0) { dateDiff.hourDiff -= 1; dateDiff.minDiff = 60 + dateDiff.minDiff; }
			//	else if (dateDiff.secondDiff >= 0) { dateDiff.hourDiff += 1; dateDiff.secondDiff = dateDiff.secondDiff - 60; }
			//}

			dateDiff.months 	= dateDiff.monthDiff;
			dateDiff.days 		= dateDiff.monthDiff * 30 + dateDiff.dayDiff;
			dateDiff.hours 		= dateDiff.days * 24 + dateDiff.hourDiff;
			dateDiff.minutes 	= dateDiff.hours * 60 + dateDiff.minDiff;
			dateDiff.seconds 	= dateDiff.minutes * 60 + dateDiff.secondDiff;

			return dateDiff;
		}

		private static int UNBOUNDED = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);

		// To calculate the total height of all items in ListView call with items = adapter.getCount()
		public static int getItemHeightofListView(ListView listView, int items)
		{
			IListAdapter adapter = listView.Adapter;

			int grossElementHeight = 0;
			for (int i = 0; i < items; i++)
			{
				View childView = adapter.GetView(i, null, listView);
				childView.Measure(UNBOUNDED, UNBOUNDED);
				grossElementHeight += childView.MeasuredHeight;
			}
			return grossElementHeight;
		}


		// case A2: before the drying time
		// now: 8:45:40
		// dry time: 9:35:30
		// hour diff = 1, minute diff = -10, second diff: -10;
		// correction: hourDiff -= 1 => 0, minuteDiff = 60 + (-10) = 50, secondDiff = 60 + (-10) =  50, minuteDiff -= 1 = 49
		// final: 49 minutes 50 seconds

		// case B1: past the drying time
		// now: 9:35:30
		// dry time: 8:45:40
		// diff = -50 minutes -10 seconds
		// hour diff = -1, minute diff = 45 - 35 = 10, second Diff = 10
		// correction: hourDiff += 1 => 0, minute diff = 10 - 60 => -50, secondDiff = ; 

		// case B2: past the drying time
		// now: 9:45:30
		// dry time: 8:35:30
		// hourDiff = -1, minute Diff = -10
		// correction: none
	}
}	