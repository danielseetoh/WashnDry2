
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

	public class ProgressFragment : Fragment
	{
		private LinearLayout pageWrapper;
		private ScrollView scrollView1;
		private TextView name;
		private TextView country;
		private EditText changeEmail;
		private EditText changePassword;
		private CheckBox allowLocationServices;
		private Spinner laundryFrequencySpinner;
		private ListView mListView;
		private Button editSettings;
		private Button resetSettings;
		private Button saveChangesButton;

		private Dictionary<int, string> laundryTime;
		private SparseBooleanArray updatedLaundryTimes;
		private AppPreferences userInfo;

		private ICollection<string> myStringCollection;
		private List<int> laundryTime_Keys;
		private List<string> laundryTime_Values;
		private List<bool> laundryTime_Checked;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var rootView = inflater.Inflate(Resource.Layout.Account, container, false);
			//var imageId = Resources.GetIdentifier(
			//	"com.companyname.washndry:drawable/earth",
			//	null, null);

			// Constructors of other classes to get stored data
			userInfo = new AppPreferences(Activity);
			ListData data = new ListData();
			laundryTime = data.getLaundryTimeSlots();

			// retrieving data from db
			laundryTime_Keys = laundryTime.Keys.ToList();
			laundryTime_Values = laundryTime.Values.ToList();
			laundryTime_Checked = DataTransformers.stringToBooleanList(userInfo.getLaundryTime());

			// retrieving UI elements as objects
			pageWrapper = rootView.FindViewById<LinearLayout>(Resource.Id.pageWrapper);
			scrollView1 = rootView.FindViewById<ScrollView>(Resource.Id.scrollView1);
			name = rootView.FindViewById<TextView>(Resource.Id.name);
			country = rootView.FindViewById<TextView>(Resource.Id.country);
			changeEmail = rootView.FindViewById<EditText>(Resource.Id.changeEmail);
			changePassword = rootView.FindViewById<EditText>(Resource.Id.changePassword);
			allowLocationServices = rootView.FindViewById<CheckBox>(Resource.Id.allowLocationServices);
			laundryFrequencySpinner = rootView.FindViewById<Spinner>(Resource.Id.laundryFrequency_Spinner);
			mListView = rootView.FindViewById<ListView>(Resource.Id.laundryTime_ListView);
			editSettings = rootView.FindViewById<Button>(Resource.Id.editSettings);
			resetSettings = rootView.FindViewById<Button>(Resource.Id.resetSettings);
			saveChangesButton = rootView.FindViewById<Button>(Resource.Id.saveChanges);



			pageWrapper.SetPadding(80, 0, 80, 0);


			enableUIElements(false);

			// Array adaptors for the list view
			ArrayAdapter<string> adaptor = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItemMultipleChoice, laundryTime_Values);
			mListView.Adapter = adaptor;
			mListView.ChoiceMode = Android.Widget.ChoiceMode.Multiple;

			updatedLaundryTimes = mListView.CheckedItemPositions; // Reads the value of the checkboxes (Whether they are selected or not) and then does someting with the data

			// Assigning values to the various UI elements
			name.Text = userInfo.getUsername();
			country.Text = "Japan, Tokyo";
			initializeFieldsFromDB();

			// Laundry Frequency Spinner Selector
			string firstItem = laundryFrequencySpinner.SelectedItem.ToString();
			laundryFrequencySpinner.ItemSelected += (s, e) =>
			{
				string selected = laundryFrequencySpinner.SelectedItem.ToString();
				if (firstItem.Equals(selected))
				{
					//do something
				}
				else {
					//Toast.MakeText(Activity, "Selected: " + selected, ToastLength.Short).Show();
				}

			};

			editSettings.Click += editSettingsEvent;
			resetSettings.Click += resetSettingsEvent;
			saveChangesButton.Click += saveChangesEvent;

			return rootView;
		}

		void click_event(object sender, AdapterView.ItemClickEventArgs e)
		{
			//var selectedIndex = e.Position.ToString();
			//Toast.MakeText(Activity, "clicked: " + selectedIndex, ToastLength.Short).Show();
			//throw new NotImplementedException();
		}

		void editSettingsEvent(object sender, EventArgs e)
		{
			enableUIElements(true);
			Toast.MakeText(Activity, "In Edit Mode", ToastLength.Short).Show();
			//throw new NotImplementedException();
		}

		void resetSettingsEvent(object sender, EventArgs e)
		{
			initializeFieldsFromDB();
			Toast.MakeText(Activity, "changes reset", ToastLength.Short).Show();
			//throw new NotImplementedException();
		}

		void saveChangesEvent(object sender, EventArgs e)
		{
			String updatedLaundryTimes_String = DataTransformers.sparseArrayToString(updatedLaundryTimes);
			userInfo.saveLaundryTime(updatedLaundryTimes_String);
			enableUIElements(false);
			Toast.MakeText(Activity, "changes saved", ToastLength.Short).Show();
		}

		void initializeFieldsFromDB()
		{
			changeEmail.Text = userInfo.getEmail();
			changePassword.Text = "";
			for (int i = 0; i < laundryTime_Checked.Count; i++) { mListView.SetItemChecked(laundryTime_Keys[i], laundryTime_Checked[i]); }
		}

		void enableUIElements(bool isEnabled)
		{
			changeEmail.Enabled = isEnabled;
			changePassword.Enabled = isEnabled;
			allowLocationServices.Enabled = isEnabled;
			laundryFrequencySpinner.Enabled = isEnabled;
			mListView.Enabled = isEnabled;
			editSettings.Enabled = !isEnabled;
			resetSettings.Enabled = isEnabled;
			saveChangesButton.Enabled = isEnabled;
		}

	}

}