using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Provider;

namespace WashnDry
{
	public class RetrieveCalendarData
	{
		private static readonly DateTime Jan1st1970 = new DateTime
	(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		
		public static void getAndroidCalendarData(Context context)
		{
			var calendarsUri = CalendarContract.Calendars.ContentUri;
			string[] calendarsProjection = {
				CalendarContract.Calendars.InterfaceConsts.Id,
				CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
				CalendarContract.Calendars.InterfaceConsts.AccountName,
				CalendarContract.Calendars.InterfaceConsts.OwnerAccount
			};
			MainActivity activity = new MainActivity();
			int _calId = 1;
			DateTime today = DateTime.Today.ToUniversalTime();
			long currentDayMilliseconds = (long)(today - Jan1st1970).TotalMilliseconds;
			DateTime oneWeekLater = DateTime.Today.ToUniversalTime();
			oneWeekLater = oneWeekLater.AddDays(7);
			long oneWeekLaterMilliseconds = (long)(oneWeekLater - Jan1st1970).TotalMilliseconds;
			var cursor1 = context.ContentResolver.Query(calendarsUri, calendarsProjection, null, null, null);
			//var cursor1 = activity.ManagedQuery(calendarsUri, calendarsProjection, null, null, null);
			for (cursor1.MoveToFirst(); !cursor1.IsAfterLast; cursor1.MoveToNext())
			{
				int calendarId = cursor1.GetInt(0);
				string _calendar = cursor1.GetString(1);
				if (_calendar == "Samsung Calendar")
				{
					_calId = calendarId;
				}
			}
			var eventsUri = CalendarContract.Events.ContentUri;
			string[] eventsProjection = {
				CalendarContract.Events.InterfaceConsts.CalendarId,
				CalendarContract.Events.InterfaceConsts.Title,
				CalendarContract.Events.InterfaceConsts.Dtstart,
				CalendarContract.Events.InterfaceConsts.Dtend,
				CalendarContract.Events.InterfaceConsts.Availability
			};
			string selection = string.Format("CALENDAR_ID={0} AND AVAILABILITY=0 AND " +
				 "DTSTART>={1} AND DTSTART<={2}", _calId, currentDayMilliseconds,
											 oneWeekLaterMilliseconds);
			string[] path = null;
			string sortOrder = "dtstart ASC";
			var cursor2 = context.ContentResolver.Query(eventsUri, eventsProjection, selection, path, sortOrder);
			//var cursor2 = activity.ManagedQuery(eventsUri, eventsProjection, selection, path, sortOrder);

			string events_titles = "";
			string events_timings = "";
			for (cursor2.MoveToFirst(); !cursor2.IsAfterLast; cursor2.MoveToNext())
			{
				string _calendarId = cursor2.GetString(0);
				string _name = cursor2.GetString(1);
				double _dtstart = cursor2.GetDouble(2);
				double _dtend = cursor2.GetDouble(3);
				string _availability = cursor2.GetString(4);
				if (events_titles == "" || events_timings == "")
				{
					events_titles = _name;
					events_timings = _dtstart.ToString() + "," + _dtend.ToString();
				}
				else {
					events_titles = events_titles + "," + _name;
					events_timings = events_timings + "," + _dtstart.ToString() + "," + _dtend.ToString();
				}

			}
			//Toast.MakeText(this, test_data, ToastLength.Long).Show();
			//string[] events_data = { events_titles, events_timings };
			//return events_data;

			//DateTime today = DateTime.Today.ToUniversalTime();
			//long currentDayMilliseconds = (long)(today - Jan1st1970).TotalMilliseconds;
			//DateTime today = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(currentDayMilliseconds);
			//today = testtoday.ToLocalTime();

			//if (events_data == null)
			//	return;

			//string events_titles = events_data[0];
			//string events_timings = events_data[1];
			Console.Out.WriteLine("events_titles: " + events_titles);
			Console.Out.WriteLine("events_timings: " + events_timings);
			if (events_titles.Length > 0 && events_timings.Length > 0)
			{
				List<string> titles = events_titles.Split(',').ToList();
				List<string> timings = events_timings.Split(',').ToList();
				// parse milliseconds into a block
				string all_event_dates = "";
				for (int i = 0; i < timings.Count; i = i + 2)
				{
					string title = titles[i / 2];
					Console.WriteLine("Timings: " + timings[i]);
					Console.Out.WriteLine(i);
					Console.Out.WriteLine(timings[i]);
					DateTime event_start_time = Jan1st1970.AddMilliseconds(long.Parse(timings[i]));
					//int start_hours = (int)(event_start_time - DateTime.Today.ToUniversalTime()).TotalHours;
					//int start_position = getPosition(start_hours);
					DateTime event_end_time = Jan1st1970.AddMilliseconds(long.Parse(timings[i + 1]));
					//int end_hours = (int)(event_end_time - DateTime.Today.ToUniversalTime()).TotalHours;
					//int end_position = getPosition(end_hours);
					Console.Out.WriteLine("real_start_time: " + event_start_time.ToLocalTime());
					Console.Out.WriteLine("real_end_time: " + event_end_time.ToLocalTime());
					int counter = (int)(event_end_time - event_start_time).TotalHours;
					//for (int j = start_position; j < end_position; j = j + 6)
					//{
					//	event_timing_blocks.Add(j);
					//	event_title_blocks.Add(title);
					//	counter = counter + 1;
					//}
					for (int k = 0; k < counter; k++)
					{
						if (all_event_dates == "")
						{
							all_event_dates = event_start_time.AddHours(k).ToLocalTime().ToString();
						}
						else {
							all_event_dates = all_event_dates + "," + event_start_time.AddHours(k).ToLocalTime().ToString();
						}
					}
				}
				Context mContext = Android.App.Application.Context;
				AppPreferences ap = new AppPreferences(mContext);
				ap.saveCalendarEventsDates(all_event_dates);

			}
		}
	}
}
