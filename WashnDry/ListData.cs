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
using Android.Preferences;

namespace WashnDry
{
	public class ListData
	{
		public ListData()
		{
		}

		private List<string> mItems;

		Dictionary<int, string> laundrySlots;



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

		public List<int> getLaundryTime_Keys()
		{
			laundrySlots = new Dictionary<int, string>();
			return laundrySlots.Keys.ToList();
		}

		public List<string> getLaundryTime_Values()
		{
			laundrySlots = new Dictionary<int, string>();
			return laundrySlots.Values.ToList();
		}

	}
}
