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
	public static class RatingData
	{
		public static Dictionary<double, double> scale;
		public static void getScaleData()
		{
			scale = new Dictionary<double, double>();
			scale.Add(1, 0.6);
			scale.Add(2, 0.8);
			scale.Add(3, 1);
			scale.Add(4, 1.3);
			scale.Add(5, 1.5);

			
		}





	}
}
