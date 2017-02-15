using System;
using System.Collections.Generic;
using System.Json;
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
	public static class Helpers
	{
		
		public static void destroyTimer(System.Threading.Timer t)
		{
			if (t != null)
			{
				Console.WriteLine("timer destroyed");
				t.Dispose();
				t = null;
			}
		}
	}
}	