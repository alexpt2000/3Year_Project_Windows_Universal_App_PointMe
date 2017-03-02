using SqliteUWP.Model;
using SqliteUWP.ViewModel;
using SqliteUWP.Views;
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
 * 
 * Navigate Page
 * // https://channel9.msdn.com/Series/Windows-10-development-for-absolute-beginners/UWP-019-Working-with-Navigation
     
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

                    BasicGeoposition location = new BasicGeoposition();

                    location.Latitude = locationGPS.Coordinate.Latitude;
                    location.Longitude = locationGPS.Coordinate.Longitude;

                    MyMap_Location(location);

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

        private void MyMap_Location(BasicGeoposition location)
        {
            mapWithMyLocation.Center = new Geopoint(location);
            mapWithMyLocation.ZoomLevel = 9;
        }


        private void MyMap_Loaded()
        {
            BasicGeoposition location = new BasicGeoposition();

            location.Latitude = 53.27058;
            location.Longitude = -9.065248;

            mapWithMyLocation.Center = new Geopoint(location);
            mapWithMyLocation.ZoomLevel = 9;
        }

        private void AddMapIcon(BasicGeoposition location, string pointName)
        {

            // Specify a known location.
            Geopoint snPoint = new Geopoint(location);

            // Create a MapIcon.
            MapIcon mapIcon1 = new MapIcon();
            mapIcon1.Location = snPoint;
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon1.Title = pointName;
            mapIcon1.ZIndex = 0;

            // Add the MapIcon to the map.
            mapWithMyLocation.MapElements.Add(mapIcon1);
        }


        private void Arial3D_Click(object sender, RoutedEventArgs e)
        {
            if (mapWithMyLocation.Is3DSupported)
            {
                // Set the aerial 3D view.
                mapWithMyLocation.Style = MapStyle.Aerial3DWithRoads;
            }
            else
            {
                // If 3D views are not supported, display dialog.
                msgDialog.Content = "3D views are not supported.";
                msgDialog.ShowAsync();
            }

        }


        private void NormalMap_Click(object sender, RoutedEventArgs e)
        {
            mapWithMyLocation.Style = MapStyle.Road;
        
        }

        private void TerrainMap_Click(object sender, RoutedEventArgs e)
        {
            mapWithMyLocation.Style = MapStyle.Terrain;
        }

        private void ListPoints_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListPoints));
        }

        private async void mapWithMyLocation_MapHolding(MapControl sender, MapInputEventArgs args)
        {
            BasicGeoposition tappedGeoPosition = args.Location.Position;

            // Creates the text box
            var PointName = new TextBox {Margin = new Thickness(10), PlaceholderText = "Point Name"};
            var PointNotes = new TextBox {Margin = new Thickness(10), PlaceholderText = "Point Notes" };

            // Creates the StackPanel with the content
            var contentPanel = new StackPanel();
            contentPanel.Children.Add(new TextBlock
            {
                Text = "Enter your Point Name and Note",
                Margin = new Thickness(10),
                TextWrapping = TextWrapping.WrapWholeWords
            });
            contentPanel.Children.Add(PointName);
            contentPanel.Children.Add(PointNotes);

            // Creates the Point dialog
            ContentDialog _pointDialog = new ContentDialog
            {
                PrimaryButtonText = "Save",
                IsPrimaryButtonEnabled = false,
                SecondaryButtonText = "Exit",
                Title = "Save Point",
                Content = contentPanel
            };

            // Report that the dialog has been opened to avoid overlapping
            _pointDialog.Opened += delegate
            {
                // HACK - opacity set to 0 to avoid seeing behind dialog
                Window.Current.Content.Opacity = 1;
            };

            // Report that the dialog has been closed to enable it again
            _pointDialog.Closed += delegate
            {
                // HACK - opacity set to 1 to reset the window to original options
                Window.Current.Content.Opacity = 1;
            };

            // Clear inserted password for next logins
            _pointDialog.PrimaryButtonClick += delegate
            {

                DatabaseHelperClass Db_Helper = new DatabaseHelperClass();//Creating object for DatabaseHelperClass.cs from ViewModel/DatabaseHelperClass.cs  
                if (PointName != null)
                {
                    Db_Helper.Insert(new Points(PointName.Text, PointNotes.Text, tappedGeoPosition.Latitude.ToString(), tappedGeoPosition.Longitude.ToString()));

                    AddMapIcon(tappedGeoPosition, PointName.Text);
                }
                else
                {
                    //MessageDialog messageDialog = new MessageDialog("Please fill the field Point Name");//Text should not be empty  
                    //await messageDialog.ShowAsync();
                }


            };

            // Close the app if the user doesn't insert the point
            _pointDialog.SecondaryButtonClick += delegate { Application.Current.Exit(); };

            // Set the binding to enable/disable the accept button 

            _pointDialog.SetBinding(ContentDialog.IsPrimaryButtonEnabledProperty, new Binding
            {
                Source = PointName,
                Path = new PropertyPath("Point"),
                Mode = BindingMode.TwoWay
            });

            var result = await _pointDialog.ShowAsync();
  
        }

    }
}
