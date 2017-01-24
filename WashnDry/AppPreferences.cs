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
	}
}