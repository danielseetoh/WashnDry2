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
	public static class FabricData
	{
		public static TupleList<int, int, string, string, string> f_list;
		public static Dictionary<int, Tuple<string, int, string, int, string>> fList;

		public static void getFabricList()
		{
			fList = new Dictionary<int, Tuple<string, int, string, int, string>>();
			fList.Add(1, Tuple.Create("Whites", Resource.Drawable.i_tShirt, "Regular / Permanent Press \n Hot / Warm Water \n Medium Spin", Resource.Drawable.tide_vividBleach, "Tide Vivid Bleach"));
			fList.Add(2, Tuple.Create("Darks", Resource.Drawable.i_longPants, "Regular / Permanent Press \n Warm Water \n Medium Spin", Resource.Drawable.tide_colorGuard, "Tide Color Guard"));
			fList.Add(3, Tuple.Create("Light Colors", Resource.Drawable.i_socks, "Regular / Permanent Press \n Warm Water \n Medium Spin", Resource.Drawable.tide_lenor, "Lenor"));
			fList.Add(4, Tuple.Create("Bold Colors", Resource.Drawable.i_shirt, "Regular / Permanent Press \n Warm Water \n Medium Spin", Resource.Drawable.tide_colorGuard, "Tide Color Guard"));
			fList.Add(5, Tuple.Create("Delicates", Resource.Drawable.i_dress, "Delicate / Gentle Cycle \n Cold Water \n Slow Spin", Resource.Drawable.tide_gentleLiquid, "Tide Gentle Liquid"));
			fList.Add(6, Tuple.Create("Miscellaneous", Resource.Drawable.i_towels, "Regular / Permanent Press \n Hot / Warm Water \n Medium Spin", Resource.Drawable.tide_touchOfDowney, "Tide Touch of Downey"));
		}



	}
}
