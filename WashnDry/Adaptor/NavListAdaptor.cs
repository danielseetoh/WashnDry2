using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;

namespace WashnDry
{
	public class NavListAdaptor : BaseAdapter
	{
		Activity activity;
		List<NavList> navItems;
		int resource;

		ImageView fabricImage;


		public NavListAdaptor(Activity a, List<NavList> n)
		{
			this.activity = a;
			this.navItems = n;
		}

		public NavListAdaptor(Activity a, int r, List<NavList> n)
		{
			this.activity = a;
			this.navItems = n;
			this.resource = r;
		}

		public override int Count
		{
			get { return navItems.Count; }
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;
		}

		public override long GetItemId(int position)
		{
			return navItems[position].Id;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View rowView = null;
			if (navItems[position].isGroupHeader)
			{
				rowView = activity.LayoutInflater.Inflate(Resource.Layout.NavGroupHeader, parent, false);
				var userPic = rowView.FindViewById<ImageView>(Resource.Id.userpic);
				userPic.SetImageResource(navItems[position].Icon);
				var username = rowView.FindViewById<TextView>(Resource.Id.usernameText);
				username.Text = navItems[position].Name;
				rowView.SetOnClickListener(null);

			}
			else 
			{
				rowView = activity.LayoutInflater.Inflate(Resource.Layout.NavListItem, parent, false);
				var navItem = rowView.FindViewById<TextView>(Resource.Id.navItem);
				navItem.Text = navItems[position].Page;
			}
			//var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.FabricListItem, parent, false);


			return rowView;
		}
	}


	public class NavList
	{
		public int Id { get; set; }
		public int Icon { get; }
		public string Name { get;}
		public Boolean isGroupHeader {get;} = false;
		public string Page { get; set;}

		public NavList()
		{
		}

		public NavList(string title, int i)
		{
			isGroupHeader = true;
			this.Icon = i;
			this.Name = title;
		}

	}



}
