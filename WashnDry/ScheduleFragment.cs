
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
	public class ScheduleFragment : Fragment
	{
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
			return rootView;
		}
	}
}
