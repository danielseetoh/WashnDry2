using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace WashnDry
{
	public class Dialogs
	{
		public static void onDemandLaundry(Activity activity)
		{
			Context mContext = Android.App.Application.Context;
			AppPreferences ap = new AppPreferences(mContext);
			string lastPromptDate = ap.getLastOnDemandLaundryPromptDate();
			if (DateTime.Today.ToString() == lastPromptDate)
			{
				return;
			}
			else {

				View view = activity.LayoutInflater.Inflate(Resource.Layout.Dialog_OnDemandLaundry, null);
				AlertDialog builder = new AlertDialog.Builder(activity).Create();
				builder.SetView(view);
				builder.SetCanceledOnTouchOutside(false);
				Button button = view.FindViewById<Button>(Resource.Id.cancel);
				button.Click += delegate
				{
					builder.Dismiss();
				};
				builder.Show();
				ap.saveLastOnDemandLaundryPromptDate(DateTime.Today.ToString());
			}
		}
	}
}
