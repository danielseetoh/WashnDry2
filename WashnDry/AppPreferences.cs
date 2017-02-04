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
	public class AppPreferences
	{
		private ISharedPreferences prefs;
		private ISharedPreferencesEditor editor; //Declare Context,Prefrences name and Editor name  
		private Context mContext;
		private static String PREFERENCE_USERNAME_KEY = "PREFERENCE_USERNAME_KEY";
		private static String PREFERENCE_PASSWORD_KEY = "PREFERENCE_PASSWORD_KEY";
		private static String PREFERENCE_EMAIL_KEY = "PREFERENCE_EMAIL_KEY";
		private static String PREFERENCE_LAUNDRY_TIME_KEY = "PREFERENCE_LAUNDRY_TIME_KEY";
		private static String PREFERENCE_LATEST_SCHEDULE_DATE = "PREFERENCE_LATEST_SCHEDULE_DATE";
		private static String PREFERENCE_LATEST_VERY_GOOD_POSITIONS = "PREFERENCE_LATEST_VERY_GOOD_POSITIONS";
		private static String PREFERENCE_LATEST_GOOD_POSITIONS = "PREFERENCE_LATEST_GOOD_POSITIONS";
		private static String PREFERENCE_LATEST_OK_POSITIONS = "PREFERENCE_LATEST_OK_POSITIONS";
		private static String PREFERENCE_CURRENT_LATITUDE = "PREFERENCE_CURRENT_LATITUDE";
		private static String PREFERENCE_CURRENT_LONGITUDE = "PREFERENCE_CURRENT_LONGITUDE";

		public AppPreferences(Context context)
		{
			this.mContext = context;
			prefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
			editor = prefs.Edit();
		}

		public void saveUsername(string username)
		{
			editor.PutString(PREFERENCE_USERNAME_KEY, username);
			editor.Commit();

		}

		public string getUsername()
		{
			return prefs.GetString(PREFERENCE_USERNAME_KEY, "");
		}

		public void savePassword(string password)
		{
			editor.PutString(PREFERENCE_PASSWORD_KEY, password);
			editor.Commit();
		}

		public string getPassword()
		{
			return prefs.GetString(PREFERENCE_PASSWORD_KEY, "");
		}

		public void saveEmail(string email)
		{
			editor.PutString(PREFERENCE_EMAIL_KEY, email);
			editor.Commit();
		}

		public string getEmail()
		{
			return prefs.GetString(PREFERENCE_EMAIL_KEY, "");
		}

		// need to edit
		public void saveLaundryTime(string laundryTime)
		{
			editor.PutString(PREFERENCE_LAUNDRY_TIME_KEY, laundryTime);
			editor.Commit();
		}

		// need to edit
		public string getLaundryTime()
		{
			return prefs.GetString(PREFERENCE_LAUNDRY_TIME_KEY, "");
		}

		public void saveLatestScheduleDate(string date)
		{
			editor.PutString(PREFERENCE_LATEST_SCHEDULE_DATE, date);
			editor.Commit();
		}

		public string getLatestScheduleDate()
		{
			return prefs.GetString(PREFERENCE_LATEST_SCHEDULE_DATE, "");
		}

		public void saveLatestVeryGoodPositions(string dates)
		{
			editor.PutString(PREFERENCE_LATEST_VERY_GOOD_POSITIONS, dates);
			editor.Commit();
		}

		public string getLatestVeryGoodPositions()
		{
			return prefs.GetString(PREFERENCE_LATEST_VERY_GOOD_POSITIONS, "");
		}

		public void saveLatestGoodPositions(string dates)
		{
			editor.PutString(PREFERENCE_LATEST_GOOD_POSITIONS, dates);
			editor.Commit();
		}

		public string getLatestGoodPositions()
		{
			return prefs.GetString(PREFERENCE_LATEST_GOOD_POSITIONS, "");
		}

		public void saveLatestOkPositions(string dates)
		{
			editor.PutString(PREFERENCE_LATEST_OK_POSITIONS, dates);
			editor.Commit();
		}

		public string getLatestOkPositions()
		{
			return prefs.GetString(PREFERENCE_LATEST_OK_POSITIONS, "");
		}

		public void saveCurrentLatitude(string latitude)
		{
			editor.PutString(PREFERENCE_CURRENT_LATITUDE, latitude);
			editor.Commit();
		}

		public string getCurrentLatitude()
		{
			return prefs.GetString(PREFERENCE_CURRENT_LATITUDE, "");
		}

		public void saveCurrentLongitude(string longitude)
		{
			editor.PutString(PREFERENCE_CURRENT_LONGITUDE, longitude);
			editor.Commit();
		}

		public string getCurrentLongitude()
		{
			return prefs.GetString(PREFERENCE_CURRENT_LONGITUDE, "");
		}
	}
}