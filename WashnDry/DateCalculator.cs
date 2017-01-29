using System;
namespace WashnDry
{
	public class DateCalculator
	{

		private long timeInMonths, timeInDays, timeInHours, timeInMinutes, timeInSeconds;
		private string digital;
		public Verbose verbose = new Verbose();
		public DateCalculator(){}

		public void storeDiffBetweenDates(DateTime laterDate, DateTime earlyDate)
		{
			long daysInCurrentMonth;
			if (earlyDate.Month == 1 || earlyDate.Month == 3 || earlyDate.Month == 5 || earlyDate.Month == 7 || earlyDate.Month == 8 || earlyDate.Month == 10 || earlyDate.Month == 12)
				daysInCurrentMonth = 31;
			else if (earlyDate.Month == 2) daysInCurrentMonth = 28;
			else daysInCurrentMonth = 30;

			long monthDiff = laterDate.Month - earlyDate.Month;
			long hourDiff = laterDate.Hour - earlyDate.Hour;
			long dayDiff = laterDate.Day - earlyDate.Day;
			long minDiff = laterDate.Minute - earlyDate.Minute;
			long secondDiff = laterDate.Second - earlyDate.Second;

			timeInMonths = monthDiff;
			timeInDays = timeInMonths * daysInCurrentMonth + dayDiff;
			timeInHours = timeInDays * 24 + hourDiff;
			timeInMinutes = timeInHours * 60 + minDiff;
			timeInSeconds = timeInMinutes * 60 + secondDiff;
		}

		public void formatSeconds(long sec)
		{
			long _months = 5;
			long _days = sec / 86400;
			long _hours = sec / 3600;
			long _minutes = (sec % 3600) / 60;
			long _seconds = sec % 60;

			string _monthStr = "", _dayStr = "", _hourStr="", _minuteStr="", _secondStr="";

			this.digital = _hours.ToString("D2") + ":" + _minutes.ToString("D2") + ":" + _seconds.ToString("D2");


			if (_months != 0) { _monthStr = _months.ToString() + " " + "Month(s)" + " "; }
			if (_days != 0) { _dayStr = _days.ToString() + " " + "Day(s)" + " "; }
			if (_hours != 0) { _hourStr = _hours.ToString() + " " + "Hour(s)" + " "; }
			if (_minutes != 0) { _minuteStr = timeInMinutes.ToString() + " " + "Minute(s)" + " "; }
			if (_seconds != 0) { _secondStr = timeInSeconds.ToString() + " " + "Second(s)" + " "; }

			verbose.all = _monthStr + _dayStr + _hourStr + _minuteStr + _secondStr;
			verbose.monthDay = _monthStr + _dayStr;
			verbose.dayHour = _dayStr + _hourStr;
			verbose.hourMin = _hourStr + _minuteStr;
			verbose.hourMinSec = _hourStr + _minuteStr + _secondStr;
			verbose.minSec = _minuteStr + _secondStr;
			verbose.min = _minuteStr;
			verbose.sec = _secondStr;
		}


		public long Months
		{
			get { return timeInMonths; }
		}
		public long Days
		{
			get { return timeInDays; }
		}
		public long Hours
		{
			get { return timeInHours; }
		}
		public long Minutes
		{
			get { return timeInMinutes; }
		}
		public long Seconds
		{
			get { return timeInSeconds; }
		}

		public string Digital
		{
			get { return digital; }
		}


		public struct Verbose
		{
			public string all, monthDay, dayHour, hourMinSec, hourMin, min, minSec, sec;
			public string All
			{
				get { return all; }
			}

			public string MonthDay
			{
				get { return monthDay; }
			}

			public string DayHour
			{
				get { return dayHour; }
			}

			public string HourMinSec
			{
				get { return hourMinSec; }
			}

			public string HourMin
			{
				get { return hourMin; }
			}

			public string Min
			{
				get { return min; }
			}

			public string MinSec
			{
				get { return minSec; }
			}

			public string Sec
			{
				get { return sec; }
			}
		}

	}
}
