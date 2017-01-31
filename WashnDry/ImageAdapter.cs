using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace WashnDry
{
	public class ImageAdapter : BaseAdapter
	{
		Context context;
		string backgroundColor = "#F2F2F2";
		string emptyColor = "#FFFFFF";
		string veryGoodColor = "#009900";
		string goodColor = "#66ff66";
		string okColor = "#ccffcc";
		string busyColor = "#b6b6b6";
		string[] events_data;
		List<string> event_title_blocks;
		List<int> event_timing_blocks;
		List<string> wash_title_blocks;
		List<int> wash_timing_blocks;

		public ImageAdapter(Context c, string[] ed)
		{
			context = c;
			events_data = ed;
			parseEventsData(); 
			getWashDates();
		}

		public override int Count
		{
			//get { return thumbIds.Length; }
			get { return 150; }
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;
		}

		public override long GetItemId(int position)
		{
			return 0;
		}

		// create a new ImageView for each item referenced by the Adapter
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var time = 0;
			TextView view;
			DateTime now = DateTime.Now;




			if (convertView == null)
			{
				view = new TextView(context);
				view.LayoutParameters = new GridView.LayoutParams(100, 60);
				view.SetPadding(0, 0, 0, 0);
			}
			else {
				view = (TextView)convertView;
			}
			if (position <= 5)
			{
				// days row
				view.SetBackgroundColor(Color.ParseColor(backgroundColor));
				switch (position)
				{
					case 1:
						view.Text = now.ToString("dd MMM");
						break;
					case 2:
						view.Text = now.AddDays(1).ToString("dd MMM");
						break;
					case 3:
						view.Text = now.AddDays(2).ToString("dd MMM");
						break;
					case 4:
						view.Text = now.AddDays(3).ToString("dd MMM");
						break;
					case 5:
						view.Text = now.AddDays(4).ToString("dd MMM");
						break;
					default:
						view.Text = "";
						break;
				}
				view.Gravity = GravityFlags.Center;
			}
			else if (position % 6 == 0)
			{
				// timings column
				if (position > 5)
				{
					time = (position / 6) - 1;
					view.Text = time + ":00";
				}
				else {
					view.Text = "";
				}
				view.Gravity = GravityFlags.Center;
				view.SetBackgroundColor(Color.ParseColor(backgroundColor));
			}
			else {
				// slots in each day

				view.SetBackgroundColor(Color.ParseColor(emptyColor));
				view.Text = "";
				if (wash_timing_blocks.Contains(position))
				{
					switch (wash_title_blocks[wash_timing_blocks.IndexOf(position)])
					{
						case "Ok":
							view.SetBackgroundColor(Color.ParseColor(okColor));
							break;
						case "Good":
							view.SetBackgroundColor(Color.ParseColor(goodColor));
							break;
						case "VeryGood":
							view.SetBackgroundColor(Color.ParseColor(veryGoodColor));
							break;
					}
				}

				if (event_timing_blocks.Contains(position))
				{
					view.SetBackgroundColor(Color.ParseColor(busyColor));
					//view.Text = event_title_blocks[event_timing_blocks.IndexOf(position)];
				}
			}

			return view;

		}


		private void parseEventsData()
		{
			event_timing_blocks = new List<int>();
			event_title_blocks = new List<string>();
			// return strings of titles and start block and end block positions
			DateTime start_of_time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			//DateTime today = DateTime.Today.ToUniversalTime();
			//long currentDayMilliseconds = (long)(today - Jan1st1970).TotalMilliseconds;
			//DateTime today = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(currentDayMilliseconds);
			//today = testtoday.ToLocalTime();

			if (events_data == null)
				return;

			string events_titles = events_data[0];
			string events_timings = events_data[1];
			List<string> titles = events_titles.Split(',').ToList();
			List<string> timings = events_timings.Split(',').ToList();
			// parse milliseconds into a block

			for (int i = 0; i < timings.Count; i = i + 2)
			{
				string title = titles[i / 2];
				Console.WriteLine("Timings: " + timings[i]);
				DateTime event_start_time = start_of_time.AddMilliseconds(long.Parse(timings[i]));
				int start_hours = (int)(event_start_time - DateTime.Today.ToUniversalTime()).TotalHours;
				int start_position = getPosition(start_hours);
				DateTime event_end_time = start_of_time.AddMilliseconds(long.Parse(timings[i + 1]));
				int end_hours = (int)(event_end_time - DateTime.Today.ToUniversalTime()).TotalHours;
				int end_position = getPosition(end_hours);
				for (int j = start_position; j <= end_position; j = j + 6)
				{
					event_timing_blocks.Add(j);
					event_title_blocks.Add(title);
				}

			}
		}

		private int getPosition(int hours)
		{
			// returns position based on the number of hours from the beginning of calendar view period
			int position = 6 + hours / 24 + 1 + (hours % 24) * 6;
			if (position > 150)
			{
				position = 150;
			}
			return position;
		}


		private void getWashDates()
		{
			wash_title_blocks = new List<string>();
			wash_timing_blocks = new List<int>();
			//string currentDate = DateTime.Now.ToString("dd MMM");
			//Context mContext = Android.App.Application.Context;
			//AppPreferences ap = new AppPreferences(mContext);
			//string latestScheduleDate = ap.getUsername();

			//if (latestScheduleDate == currentDate)
			//{
			//	Toast.MakeText(context, "Forecast up to date", ToastLength.Long);
			//	string[] very_good_blocks = ap.getLatestVeryGoodPositions().Split(',');
			//	string[] good_blocks = ap.getLatestGoodPositions().Split(',');
			//	string[] ok_blocks = ap.getLatestOkPositions().Split(',');
			//	for (int i = 0; i < very_good_blocks.Count(); i++)
			//	{
			//		wash_title_blocks.Add("VeryGood");
			//		wash_timing_blocks.Add(int.Parse(very_good_blocks[i]));
			//	}
			//	for (int i = 0; i < good_blocks.Count(); i++)
			//	{
			//		wash_title_blocks.Add("Good");
			//		wash_timing_blocks.Add(int.Parse(good_blocks[i]));
			//	}
			//	for (int i = 0; i < ok_blocks.Count(); i++)
			//	{
			//		wash_title_blocks.Add("Ok");
			//		wash_timing_blocks.Add(int.Parse(ok_blocks[i]));
			//	}
			//	//string combined_blocks = very_good_blocks + "," + good_blocks + "," + ok_blocks;
			//	//string[] arr = combined_blocks.Split(',');
			//	//wash_timing_blocks = Array.ConvertAll(arr, s=>int.Parse(s)).ToList();
			//}
			//else {
			//	Toast.MakeText(context, "Forecast outdated, updating now", ToastLength.Long);
			//	// get latest forecast, using location. perhaps requires a service

			//	// pass latest forecast and username to server, get values back and parse it
			//	//string very_good_blocks;
			//	//string good_blocks;
			//	//string ok_blocks;
			//	//ap.saveLatestVeryGoodPositions(very_good_blocks);
			//	//ap.saveLatestVeryGoodPositions(good_blocks);
			//	//ap.saveLatestVeryGoodPositions(ok_blocks);

			//	// update latestScheduleDate to current date
			//	latestScheduleDate = currentDate;
			//}

			// get dummy data
			wash_title_blocks.Add("Ok");
			wash_title_blocks.Add("Ok");
			wash_title_blocks.Add("Ok");
			wash_title_blocks.Add("VeryGood");
			wash_title_blocks.Add("Good");
			wash_title_blocks.Add("Good");
			wash_title_blocks.Add("VeryGood");
			wash_title_blocks.Add("Ok");

			wash_timing_blocks.Add(67);
			wash_timing_blocks.Add(73);
			wash_timing_blocks.Add(79);
			wash_timing_blocks.Add(92);
			wash_timing_blocks.Add(114);
			wash_timing_blocks.Add(128);
			wash_timing_blocks.Add(129);
			wash_timing_blocks.Add(134);
		}

	}
}
