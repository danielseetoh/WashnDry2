using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace WashnDry
{
	[Activity(Icon = "@drawable/logo", Theme = "@style/WashNDryTheme")] //@android:style/Theme.Material.Light
	public class MainActivity : Activity
	{
		//int count = 1;
		private DrawerLayout _drawer;
		private MyActionBarDrawerToggle _drawerToggle;
		private ListView _drawerList;

		private string _drawerTitle;
		private string _title;
		private string[] _pagesTitles;
		private AppPreferences userInfo;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			userInfo = new AppPreferences(this);

			SetContentView(Resource.Layout.Main);

			_title = _drawerTitle =  ""; //_drawerTitle = Title;
			_pagesTitles = Resources.GetStringArray(Resource.Array.PagesArray);

			//List<string> pTitles = new List<string>(_pagesTitles);

			_drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			_drawerList = FindViewById<ListView>(Resource.Id.left_drawer);

			_drawer.SetDrawerShadow(Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);

			//_drawerList.Adapter = new ArrayAdapter<string>(this,Resource.Layout.DrawerListItem, _pagesTitles);


			FabricData.getFabricList();
			List<NavList> nList = new List<NavList>();
			NavList head = new NavList(userInfo.getUsername(), Resource.Drawable.i_user);
			nList.Add(head);
			for (int i = 0; i < _pagesTitles.Length; i++)
			{
				NavList n = new NavList()
				{
					Id = i,
					Page = _pagesTitles[i]
				};
				nList.Add(n);
			}
			var navAdaptor = new NavListAdaptor(this, nList);

			_drawerList.Adapter = navAdaptor;

			_drawerList.ItemClick += (sender, args) => SelectItem(args.Position);

			var tb = FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbar_actionbar);
			SetActionBar(tb);

			tb.SetNavigationIcon(Resource.Drawable.i_timer);

			ActionBar.SetDisplayHomeAsUpEnabled(true);
			ActionBar.SetHomeButtonEnabled(true);

			//DrawerToggle is the animation that happens with the indicator next to the
			//ActionBar icon. You can choose not to use this.
			_drawerToggle = new MyActionBarDrawerToggle(this, _drawer,
													  Resource.Drawable.ic_drawer_light,
													  Resource.String.DrawerOpen,
													  Resource.String.DrawerClose);


			//You can alternatively use _drawer.DrawerClosed here
			//_drawerToggle.DrawerClosed += delegate
			//{
			//	ActionBar.Title = _title;
			//	InvalidateOptionsMenu();
			//};

			////You can alternatively use _drawer.DrawerOpened here
			//_drawerToggle.DrawerOpened += delegate
			//{
			//	ActionBar.Title = _drawerTitle;
			//	InvalidateOptionsMenu();
			//};

			//_drawer.SetDrawerListener(_drawerToggle);

			if (null == savedInstanceState)
				SelectItem(0);
			


		}

		private void SelectItem(int position)
		{
			switch (position)
			{
				case 1:{
						// Status
						var fragment = new StatusFragment();
						var arguments = new Bundle();
						fragment.Arguments = arguments;

						FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();
						break;
				}
				case 2:{
						// Weather
						var fragment = new WeatherFragment();
						var arguments = new Bundle();
						fragment.Arguments = arguments;

						FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();
						break;
				}
				case 3:{
						// Schedule
						var fragment = new ScheduleFragment();
						var arguments = new Bundle();
						fragment.Arguments = arguments;

						FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();
						break;
				}
				case 4:
				{
					// Fabric Care
					var fragment = new FabricFragment();
					var arguments = new Bundle();
					fragment.Arguments = arguments;

					FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();
					break;
				}
				case 5:{
						// Account
						var fragment = new AccountFragment();
						var arguments = new Bundle();
						fragment.Arguments = arguments;

						FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();
						break;
				}
				default:
					{
						var fragment = new StatusFragment();
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
			var drawerOpen = _drawer.IsDrawerOpen(Resource.Id.drawer_layout);
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

