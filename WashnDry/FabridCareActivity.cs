
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
	[Activity(Label = "Fabric Care Information")]
	public class FabricCareActivity : Activity
	{
		Context mContext = Android.App.Application.Context;
		AppPreferences ap;
		TextView pageHeader, washInstruction, dryInstruction;
		Button buy, back;
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
			buy = FindViewById<Button>(Resource.Id.buy);
			back = FindViewById<Button>(Resource.Id.back);
			buy.Click += delegate { 
				this.Finish();
			};
			back.Click += delegate { this.Finish(); };

			int id = Intent.Extras.GetInt("fabricSelectedId");
			pageHeader.Text = FabricData.fList[id].Item1;
			washInstruction.Text = FabricData.fList[id].Item3;
			recProductImage.SetImageResource(FabricData.fList[id].Item4);
			dryInstruction.Text = FabricData.fList[id].Item5;
		}

		void BackButton_Click(object sender, EventArgs e)
		{

			this.Finish();
		}


		public override void OnBackPressed()
		{
			this.Finish();
			base.OnBackPressed();
		}
	}
}
