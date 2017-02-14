
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

namespace WashnDry
{
	[Activity(Label = "LaundrySelectActivity")]
	public class LaundrySelectActivity : Activity
	{
		Dictionary<int, string> laundrySlots;

		Context mContext = Android.App.Application.Context;
		AppPreferences ap;

		Button[] nextLaundryButton = new Button[3];

		public Dictionary<int, string> getLaundryTimeSlots()
		{
			laundrySlots = new Dictionary<int, string>();
			laundrySlots.Add(0, "5am - 8am");
			laundrySlots.Add(1, "8am - 12nn");
			laundrySlots.Add(2, "4pm - 8pm");
			laundrySlots.Add(3, "8pm - 1am");
			laundrySlots.Add(4, "1am - 5am");

			return laundrySlots;
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.LaundrySelect);
			ap = new AppPreferences(mContext); // can try using "this" also

			DateTime[] dt = new DateTime[3];
			dt[0] = new DateTime(2017, 2, 15, 14, 55, 4);
			dt[1] = new DateTime(2017, 2, 16, 16, 35, 43);
			dt[2] = new DateTime(2017, 2, 17, 23, 26, 14);

			string d0s = dt[0].ToString();
			string d1s = dt[1].ToString();
			string d2s = dt[2].ToString();

			string arrdt = d0s + "," + d1s + "," + d2s + "_" + "27.44,16.33,7";
			arrdt = ap.getThreeBestTimings();
			Console.WriteLine("3 best " + ap.getThreeBestTimings());

			// a function to split the string from DB into a DateTime array
			string[] s = arrdt.Split('_');
			DateTime[] d_t = Array.ConvertAll(s[0].Split(','), DateTime.Parse);
			double[] est_T = Array.ConvertAll(s[1].Split(','), double.Parse);

			nextLaundryButton[0] = FindViewById<Button>(Resource.Id.nextLaundryButton0);
			nextLaundryButton[1] = FindViewById<Button>(Resource.Id.nextLaundryButton1);
			nextLaundryButton[2] = FindViewById<Button>(Resource.Id.nextLaundryButton2);

			for (int i = 0; i < d_t.Length; i++)
			{
				string t = "AM";
				if (dt[i].Hour / 12 >= 1) { t = "PM";}
				nextLaundryButton[i].SetTag(nextLaundryButton[i].Id, d_t[i]+","+est_T[i]);
				var timeStr = string.Format("{0}", d_t[i].Hour%12) + t;
				TimeSpan ts = TimeSpan.FromSeconds(est_T[i]);
				string h = "";
				if (ts.Hours > 0) { h = string.Format(" {0:D2} HRS ", ts.Hours); }
				var timeToNext = "\n" + h + string.Format("{0:D2} min(s)", ts.Minutes);
				nextLaundryButton[i].Text = d_t[i].ToString("dd MMM ") + timeStr + timeToNext;
				nextLaundryButton[i].Click += BackButton_Click;
			}
		}


		void BackButton_Click(object sender, EventArgs e)
		{
			Button b = sender as Button;
			var _dt = b.GetTag(b.Id);
			var _dt_str = _dt.ToString().Split(',');
			int _estTime = (int)double.Parse(_dt_str[1]);

			var intent = new Intent();
			intent.PutExtra("selectedLaundryDate", _dt_str[0]);
			intent.PutExtra("estimatedTime", _estTime);

			SetResult(Result.Ok, intent);
			ap.saveSelectedNextLaundryTime(_dt_str[0]);
			ap.saveEstimatedTime(_estTime);

			this.Finish();
		}

		public override void OnBackPressed()
		{
			base.OnBackPressed();
		}
	}
}
