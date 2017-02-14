
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
	[Activity(Label = "FabricCareActivity")]
	public class FabricCareActivity : Activity
	{
		Context mContext = Android.App.Application.Context;
		AppPreferences ap;
		TextView pageHeader, washInstruction, dryInstruction;
		ImageView recProductImage;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.FabricCareDetails);

			ap = new AppPreferences(mContext); // can try using "this" also
			pageHeader = FindViewById<TextView>(Resource.Id.pageHeader);
			washInstruction = FindViewById<TextView>(Resource.Id.washInstruction);
			dryInstruction = FindViewById<TextView>(Resource.Id.dryInstruction);
			recProductImage = FindViewById<ImageView>(Resource.Id.recProductImage);

			int id = Intent.Extras.GetInt("fabricSelectedId");
			pageHeader.Text = FabricData.fList[id].Item1;
			washInstruction.Text = FabricData.fList[id].Item3;
			recProductImage.SetImageResource(FabricData.fList[id].Item4);
			dryInstruction.Text = FabricData.fList[id].Item5;
		}




		public override void OnBackPressed()
		{
			this.Finish();
			base.OnBackPressed();
		}
	}
}
