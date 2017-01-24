using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace WashnDry
{
	[Activity(Label = "Wash & Dry", Icon = "@mipmap/icon", Theme = "@android:style/Theme.DeviceDefault.Light")]
	public class MainActivity : Activity
	{
		//int count = 1;
		private DrawerLayout _drawer;
		private MyActionBarDrawerToggle _drawerToggle;
		private ListView _drawerList;

		private string _drawerTitle;
		private string _title;
		private string[] _pagesTitles;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			_title = _drawerTitle = Title;
			_pagesTitles = Resources.GetStringArray(Resource.Array.PagesArray);
			_drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			_drawerList = FindViewById<ListView>(Resource.Id.left_drawer);

			_drawer.SetDrawerShadow(Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);

			_drawerList.Adapter = new ArrayAdapter<string>(this,
				Resource.Layout.DrawerListItem, _pagesTitles);
			_drawerList.ItemClick += (sender, args) => SelectItem(args.Position);


			ActionBar.SetDisplayHomeAsUpEnabled(true);
			ActionBar.SetHomeButtonEnabled(true);
			ActionBar.Title = "Wash & Dry";

			//DrawerToggle is the animation that happens with the indicator next to the
			//ActionBar icon. You can choose not to use this.
			_drawerToggle = new MyActionBarDrawerToggle(this, _drawer,
													  Resource.Drawable.ic_drawer_light,
													  Resource.String.DrawerOpen,
													  Resource.String.DrawerClose);

			//You can alternatively use _drawer.DrawerClosed here
			_drawerToggle.DrawerClosed += delegate
			{
				ActionBar.Title = _title;
				InvalidateOptionsMenu();
			};

			//You can alternatively use _drawer.DrawerOpened here
			_drawerToggle.DrawerOpened += delegate
			{
				ActionBar.Title = _drawerTitle;
				InvalidateOptionsMenu();
			};

			_drawer.SetDrawerListener(_drawerToggle);

			if (null == savedInstanceState)
				SelectItem(0);
		}

		private void SelectItem(int position)
		{
			switch (position)
			{
				case 0:{
						// Home
						var fragment = new HomeFragment();
						var arguments = new Bundle();
						fragment.Arguments = arguments;

						FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();
						break;
				}
				case 1:{
						// Status
						var fragment = new StatusFragment();
						var arguments = new Bundle();
						fragment.Arguments = arguments;

						FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();
						break;
				}
				case 2:{
						// Schedule
						var fragment = new ScheduleFragment();
						var arguments = new Bundle();
						fragment.Arguments = arguments;

						FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();
						break;
				}
				case 3:{
						// Account
						var fragment = new AccountFragment();
						var arguments = new Bundle();
						fragment.Arguments = arguments;

						FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();
						break;
				}
				default:
					{
						var fragment = new HomeFragment();
						var arguments = new Bundle();
						fragment.Arguments = arguments;

						FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();
						break;
					}
			}



			_drawerList.SetItemChecked(position, true);
			_drawer.CloseDrawer(_drawerList);

		}

		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			_drawerToggle.SyncState();
		}

		public override void OnConfigurationChanged(Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			_drawerToggle.OnConfigurationChanged(newConfig);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.main, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnPrepareOptionsMenu(IMenu menu)
		{
			var drawerOpen = _drawer.IsDrawerOpen(Resource.Id.left_drawer);
			//menu.FindItem(Resource.Id.action_websearch).SetVisible(!drawerOpen);
			return base.OnPrepareOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			if (_drawerToggle.OnOptionsItemSelected(item))
				return true;
			return base.OnOptionsItemSelected(item);
		}

	}
}

