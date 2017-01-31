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
using Android.Util;


namespace WashnDry
{
	[Activity(Label = "SetupActivity", Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar")]
	public class SetupActivity : Activity
	{
		private ListView mListView;
		private Dictionary<int, string> laundryTime;
		private SparseBooleanArray updatedLaundryTimes;
		private LinearLayout mLayout;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here

			SetContentView(Resource.Layout.Setup);

			ListData data = new ListData();
			laundryTime = data.getLaundryTimeSlots();
			List<int> laundryTime_Keys = laundryTime.Keys.ToList();
			List<string> laundryTime_Values = laundryTime.Values.ToList();
			mLayout = FindViewById<LinearLayout>(Resource.Id.pageWrapper);


			EditText username = FindViewById<EditText>(Resource.Id.Username);
			EditText password = FindViewById<EditText>(Resource.Id.Password);
			EditText email = FindViewById<EditText>(Resource.Id.Email);
			Button createAccountButton = FindViewById<Button>(Resource.Id.CreateAccountButton);

			mListView = FindViewById<ListView>(Resource.Id.laundryTime_ListView);
			//mLayout.AddView(mListView);



			ArrayAdapter<string> adaptor = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemMultipleChoice, laundryTime_Values);
			mListView.Adapter = adaptor;
			mListView.ChoiceMode = Android.Widget.ChoiceMode.Multiple;
			updatedLaundryTimes = mListView.CheckedItemPositions;
			// initializing the laundry time checkboxes as false
			for (int i = 0; i < laundryTime_Keys.Count; i++) { mListView.SetItemChecked(laundryTime_Keys[i], false); }

			int lv_height = DataTransformers.getItemHeightofListView(mListView, laundryTime.Count);
			Console.WriteLine("set height of list view:" + lv_height.ToString());

			mListView.SetMinimumHeight(lv_height);
			mListView.RequestLayout();

			createAccountButton.Enabled = true;

			createAccountButton.Click += (object sender, EventArgs e) =>
			{
				Context mContext = Android.App.Application.Context;
				AppPreferences ap = new AppPreferences(mContext);
				ap.saveUsername(username.Text);
				ap.savePassword(password.Text);
				ap.saveEmail(email.Text);
				string updatedLaundryTimes_String = DataTransformers.sparseArrayToString(updatedLaundryTimes);
				ap.saveLaundryTime(updatedLaundryTimes_String);

				// save preferences for laundry timing in proper format
				// send data over to server to create a unique model for this username
				StartActivity(new Intent(Application.Context, typeof(MainActivity)));
			};
		}
	}
}