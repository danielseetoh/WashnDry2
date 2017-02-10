
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

		GridView gridview;
		
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

			gridview = rootView.FindViewById<GridView>(Resource.Id.gridview);

			//gridview.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
			//{
			//	Toast.MakeText(this, args.Position.ToString(), ToastLength.Short).Show();
			//};

			return rootView;
		}

		public override void OnResume()
		{
			base.OnResume();
			gridview.Adapter = new ImageAdapter(this.Activity);
		}



	}
}
