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
	public class FabricAdaptor : BaseAdapter
	{
		Activity activity;
		List<Fabric> fabrics;
		int resource;

		ImageView fabricImage;


		public FabricAdaptor(Activity a, List<Fabric> f)
		{
			this.activity = a;
			this.fabrics = f;
		}

		public FabricAdaptor(Activity a, int r, List<Fabric> f)
		{
			this.activity = a;
			this.fabrics = f;
			this.resource = r;
		}

		public override int Count
		{
			get { return fabrics.Count; }
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;
		}

		public override long GetItemId(int position)
		{
			return fabrics[position].Id;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View rowView = null;
			if (fabrics[position].isGroupHeader)
			{
				rowView = activity.LayoutInflater.Inflate(Resource.Layout.NavGroupHeader, parent, false);
				var userPic = rowView.FindViewById<ImageView>(Resource.Id.userpic);
				userPic.SetImageResource(Resource.Drawable.i_shirt);
				var username = rowView.FindViewById<TextView>(Resource.Id.usernameText);
				username.Text = "John";
				rowView.SetOnClickListener(null);

			}
			else 
			{
				rowView = activity.LayoutInflater.Inflate(Resource.Layout.FabricListItem, parent, false);
				var fabricType = rowView.FindViewById<TextView>(Resource.Id.fabricType);
				fabricType.Text = fabrics[position].Type;
				fabricImage = rowView.FindViewById<ImageView>(Resource.Id.fabricImage);
				fabricImage.SetImageResource(fabrics[position].Icon);

			}
			//var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.FabricListItem, parent, false);


			return rowView;
		}
	}


	public class Fabric
	{
		public int Id { get; set; }
		public string Type { get; set; }
		public int Icon { get; set; }
		public string Name { get;}

		public Boolean isGroupHeader {get;} = false;

		public Fabric()
		{
		}

		public Fabric(string title)
		{
			isGroupHeader = true;
			this.Name = title;
		}

	}



}
