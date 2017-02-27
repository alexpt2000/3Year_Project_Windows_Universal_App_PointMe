using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409


// Ref Compass
// https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/use-the-compass

// Ref Side Menu
// https://blogs.msdn.microsoft.com/quick_thoughts/2015/06/01/windows-10-splitview-build-your-first-hamburger-menu/

// Ref Maps
// https://github.com/Microsoft/Windows-universal-samples/tree/master/Samples/MapControl

// Ref Youtube
// https://www.youtube.com/watch?v=xJveKt99MXY
// https://github.com/jQuery2DotNet/UWP-Samples

// Ref Storage
// https://code.msdn.microsoft.com/windowsapps/Sqlite-Sample-for-Windows-ad3af7ae

/*
 * Wheather
 * https://channel9.msdn.com/Series/Windows-10-development-for-absolute-beginners/UWP-058-UWP-Weather-Setup-and-Working-with-the-Weather-API
     
     */



namespace PointMe
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        MessageDialog msgDialog = new MessageDialog("Standard Message");

        public MainPage()
        {
            this.InitializeComponent();
            InitializeLocator();


        }

        private async void InitializeLocator()
        {
            var userPermission = await Geolocator.RequestAccessAsync();

            switch (userPermission)
            {
                case GeolocationAccessStatus.Allowed:
                    Geolocator geolocator = new Geolocator();
                    Geoposition locationGPS = await geolocator.GetGeopositionAsync();
                    AddMapIcon(locationGPS);
                    break;

                case GeolocationAccessStatus.Denied:
                    msgDialog.Content = "I cannot check the location if you don't give me the access to your location...";
                    MyMap_Loaded();
                    await msgDialog.ShowAsync();
                    break;

                case GeolocationAccessStatus.Unspecified:
                    msgDialog.Content = "I got an error while getting location permission. Please try again...";
                    MyMap_Loaded();
                    await msgDialog.ShowAsync();
                    break;

            }
        
        }


        private void MyMap_Loaded()
        {
            BasicGeoposition location = new BasicGeoposition();

            location.Latitude = 53.27058;
            location.Longitude = -9.065248;

            mapWithMyLocation.Center = new Geopoint(location);
            mapWithMyLocation.ZoomLevel = 9;
        }

        private void AddMapIcon(Geoposition locationGPS)
        {
            BasicGeoposition location = new BasicGeoposition();

            location.Latitude = locationGPS.Coordinate.Point.Position.Latitude;
            location.Longitude = locationGPS.Coordinate.Point.Position.Longitude;

            MapIcon mapIcon;
            mapIcon = new MapIcon();
            if (mapIcon != null)
            {
                mapWithMyLocation.MapElements.Remove(mapIcon);
            }
            mapIcon.Location = new Geopoint(location);
            mapIcon.Title = "GMIT";
            mapWithMyLocation.MapElements.Add(mapIcon);
            mapWithMyLocation.Center = new Geopoint(location);
           
        }


        private void mapWithMyLocation_MapHolding(MapControl sender, MapInputEventArgs args)
        {
            var tappedGeoPosition = args.Location.Position;
            string status = "MapTapped at \n\nLatitude:" + tappedGeoPosition.Latitude + "\nLongitude: " + tappedGeoPosition.Longitude;

            msgDialog.Content = status;
            msgDialog.ShowAsync();
        }

/*

        private async void ShowStreetsideView(object sender, TappedRoutedEventArgs e)
        {
            // Check if Streetside is supported.
            if (mapWithMyLocation.IsStreetsideSupported)
            {
                // Find a panorama near Avenue Gustave Eiffel.
                BasicGeoposition cityPosition = new BasicGeoposition() { Latitude = 48.858, Longitude = 2.295 };
                Geopoint cityCenter = new Geopoint(cityPosition);
                StreetsidePanorama panoramaNearCity = await StreetsidePanorama.FindNearbyAsync(cityCenter);

                // Set the Streetside view if a panorama exists.
                if (panoramaNearCity != null)
                {
                    // Create the Streetside view.
                    StreetsideExperience ssView = new StreetsideExperience(panoramaNearCity);
                    ssView.OverviewMapVisible = true;
                    mapWithMyLocation.CustomExperience = ssView;
                }
            }
            else
            {
                // If Streetside is not supported
                ContentDialog viewNotSupportedDialog = new ContentDialog()
                {
                    Title = "Streetside is not supported",
                    Content = "\nStreetside views are not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                await viewNotSupportedDialog.ShowAsync();
            }
        }
*/





        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (mapWithMyLocation.Is3DSupported)
            {
                // Set the aerial 3D view.
                mapWithMyLocation.Style = MapStyle.Aerial3DWithRoads;

                // Specify the location.
                BasicGeoposition hwGeoposition = new BasicGeoposition() { Latitude = 34.134, Longitude = -118.3216 };
                Geopoint hwPoint = new Geopoint(hwGeoposition);

                // Create the map scene.
                MapScene hwScene = MapScene.CreateFromLocationAndRadius(hwPoint,
                                                                                     80, /* show this many meters around */
                                                                                     0, /* looking at it to the North*/
                                                                                     60 /* degrees pitch */);
                // Set the 3D view with animation.
                mapWithMyLocation.TrySetSceneAsync(hwScene, MapAnimationKind.Bow);
            }
            else
            {
                // If 3D views are not supported, display dialog.
                ContentDialog viewNotSupportedDialog = new ContentDialog()
                {
                    Title = "3D is not supported",
                    Content = "\n3D views are not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                viewNotSupportedDialog.ShowAsync();
            }

        }

        private async void MenuFlyoutItem_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Check if Streetside is supported.
            if (mapWithMyLocation.IsStreetsideSupported)
            {
                // Find a panorama near Avenue Gustave Eiffel.
                BasicGeoposition cityPosition = new BasicGeoposition() { Latitude = 48.858, Longitude = 2.295 };
                Geopoint cityCenter = new Geopoint(cityPosition);
                StreetsidePanorama panoramaNearCity = await StreetsidePanorama.FindNearbyAsync(cityCenter);

                // Set the Streetside view if a panorama exists.
                if (panoramaNearCity != null)
                {
                    // Create the Streetside view.
                    StreetsideExperience ssView = new StreetsideExperience(panoramaNearCity);
                    ssView.OverviewMapVisible = true;
                    mapWithMyLocation.CustomExperience = ssView;
                }
            }
            else
            {
                // If Streetside is not supported
                ContentDialog viewNotSupportedDialog = new ContentDialog()
                {
                    Title = "Streetside is not supported",
                    Content = "\nStreetside views are not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                await viewNotSupportedDialog.ShowAsync();
            }

        }
    }
}
