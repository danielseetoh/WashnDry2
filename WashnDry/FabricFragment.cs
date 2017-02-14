
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

	public class FabricFragment : Fragment
	{
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var rootView = inflater.Inflate(Resource.Layout.Fabric, container, false);

			FabricData.getFabricList();
			List<Fabric> fabricList = new List<Fabric>();
			for (int i = 1; i <= FabricData.fList.Count; i++)
			{
				Fabric f = new Fabric()
				{
					Icon = FabricData.fList[i].Item2,
					Type = FabricData.fList[i].Item1
				};
				fabricList.Add(f);
			}

			var fabricListView = rootView.FindViewById<ListView>(Resource.Id.FabricListView);
			var fabricAdaptor = new FabricAdaptor(Activity, fabricList);
			fabricListView.Adapter = fabricAdaptor;
			fabricListView.ItemClick += fabricClickEvent;

			return rootView;
		}

		void fabricClickEvent(object sender, AdapterView.ItemClickEventArgs e)
		{
			var f_id = FabricData.fList.Keys.ElementAt(e.Position);
			var intent = new Intent(Activity, typeof(FabricCareActivity));
			intent.PutExtra("fabricSelectedId", f_id);
			StartActivityForResult(intent, 200);
		}
}

}