
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace WashnDry
{
	public class ScheduleFragment : Fragment
	{

		private static readonly DateTime Jan1st1970 = new DateTime
	(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			//return base.OnCreateView(inflater, container, savedInstanceState);

			var rootView = inflater.Inflate(Resource.Layout.Schedule, container, false);
			//var imageId = Resources.GetIdentifier(
			//	"com.companyname.washndry:drawable/earth",
			//	null, null);
			//rootView.FindViewById<ImageView>(Resource.Id.indicators).SetImageResource(imageId);

			var gridview = rootView.FindViewById<GridView>(Resource.Id.gridview);
			string[] events_data = getAndroidCalendarData();
			gridview.Adapter = new ImageAdapter(this.Activity, events_data);
			//gridview.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
			//{
			//	Toast.MakeText(this, args.Position.ToString(), ToastLength.Short).Show();
			//};

			return rootView;
		}

		public string[] getAndroidCalendarData()
		{
			var calendarsUri = CalendarContract.Calendars.ContentUri;
			string[] calendarsProjection = {
				CalendarContract.Calendars.InterfaceConsts.Id,
				CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
				CalendarContract.Calendars.InterfaceConsts.AccountName,
				CalendarContract.Calendars.InterfaceConsts.OwnerAccount
			};

			int _calId = 1;
			DateTime today = DateTime.Today.ToUniversalTime();
			long currentDayMilliseconds = (long)(today - Jan1st1970).TotalMilliseconds;
			DateTime oneWeekLater = DateTime.Today.ToUniversalTime();
			oneWeekLater = oneWeekLater.AddDays(7);
			long oneWeekLaterMilliseconds = (long)(oneWeekLater - Jan1st1970).TotalMilliseconds;
			var cursor1 = this.Activity.ManagedQuery(calendarsUri, calendarsProjection, null, null, null);
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


			// attempt at getting recurring events (instances)
			//var eventsUriBuilder2 = CalendarContract.Instances.ContentUri.BuildUpon();
			//ContentUris.AppendId(eventsUriBuilder2, currentDayMilliseconds);
			//ContentUris.AppendId(eventsUriBuilder2, oneWeekLaterMilliseconds);
			//var eventsUri2 = eventsUriBuilder2.Build();
			//string[] eventsProjection2 = {
			//	CalendarContract.Instances.EventId,
			//	CalendarContract.Instances.Begin,
			//	CalendarContract.Instances.End,
			//};
			//string selection2 = string.Format("CALENDAR_ID={0} AND AVAILABILITY=0 AND " +
			//	 CalendarContract.Instances.Begin+ ">={1} AND " + CalendarContract.Instances.Begin + "<={2}", _calId, currentDayMilliseconds,
			//								  oneWeekLaterMilliseconds);
			//string[] path2 = null;
			//string sortOrder2 = "CalendarContract.Instances.Begin ASC";
			var cursor2 = this.Activity.ManagedQuery(eventsUri, eventsProjection, selection, path, sortOrder);

			string events_titles = "";
			string events_timings = "";
			for (cursor2.MoveToFirst(); !cursor2.IsAfterLast; cursor2.MoveToNext())
			{
				string _calendarId = cursor2.GetString(0);
				string _name = cursor2.GetString(1);
				double _dtstart = cursor2.GetDouble(2);
				double _dtend = cursor2.GetDouble(3);
				string _availability = cursor2.GetString(4);
				if (events_titles == "")
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
			string[] events_data = { events_titles, events_timings };
			return events_data;
		}
	}
}
