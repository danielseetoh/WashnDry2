//using System;
//using System.Collections.Generic;
//using System.Json;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Android.App;
//using Android.Content;
//using Android.Locations;
//using Android.OS;

//namespace WashnDry
//{
//	[Service]
//	public class RetrieveLocationService: Service, ILocationListener
//	{

//		Location _currentLocation;
//		LocationManager _locationManager;
//		string _locationProvider;
//		Context context;
//		public RetrieveLocationService(Context context)
//		{
//			this.context = context;
//		}

//		void InitializeLocationManager()
//		{

//			_locationManager = (LocationManager)context.GetSystemService(Context.LocationService);
//			Criteria criteriaForLocationService = new Criteria
//			{
//				Accuracy = Accuracy.Fine
//			};
//			IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

//			if (acceptableLocationProviders.Any())
//			{
//				_locationProvider = acceptableLocationProviders.First();
//			}
//			else
//			{
//				_locationProvider = string.Empty;
//			}
//		}

//		async void getLocation()
//		{
//			if (_currentLocation == null)
//			{
//				_addressText.Text = "Can't determine the current address. Try again in a few minutes.";
//				return;
//			}

//			Address address = await ReverseGeocodeCurrentLocation();
//			DisplayAddress(address);
//		}

//		async Task<Address> ReverseGeocodeCurrentLocation()
//		{
//			Geocoder geocoder = new Geocoder(this.Activity);
//			IList<Address> addressList =
//				await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

//			Address address = addressList.FirstOrDefault();
//			return address;
//		}

//		public string getAddress(Address address)
//		{
//			if (address != null)
//			{
//				StringBuilder deviceAddress = new StringBuilder();
//				for (int i = 0; i < address.MaxAddressLineIndex; i++)
//				{
//					deviceAddress.AppendLine(address.GetAddressLine(i));
//				}
//				// Remove the last comma from the end of the address.
//				return deviceAddress.ToString();
//			}
//			else
//			{
//				return  "Unable to determine the address. Try again in a few minutes.";
//			}
//		}

//		public async void OnLocationChanged(Location location)
//		{
//			_currentLocation = location;
//			if (_currentLocation == null)
//			{
//				_locationText.Text = "Unable to determine your location. Try again in a short while.";
//			}
//			else
//			{
//				_locationText.Text = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
//				Address address = await ReverseGeocodeCurrentLocation();
//				DisplayAddress(address);
//				JsonValue json = await FetchWeatherAsync(_currentLocation);
//				ParseAndDisplay(json);
//			}

//			Task delay = new Task(() =>
//			{
//				Task.Delay(10000);  // Reduce refresh time to 10 seconds
//			});
//		}

//		public void OnProviderDisabled(string provider) { }

//		public void OnProviderEnabled(string provider) { }

//		public void OnStatusChanged(string provider, Availability status, Bundle extras) { }
//	}
//}
