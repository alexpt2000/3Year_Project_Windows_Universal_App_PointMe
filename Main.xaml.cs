using SqliteUWP.Model;
using SqliteUWP.ViewModel;
using SqliteUWP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PointMe
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Main : Page
    {
        ObservableCollection<Points> DB_PointsList = new ObservableCollection<Points>();
        MessageDialog msgDialog = new MessageDialog("Standard Message");
        ReadAllPointsList dbcpoints = new ReadAllPointsList();
        BasicGeoposition location = new BasicGeoposition();
        DatabaseHelperClass Db_FindOne = new DatabaseHelperClass();

        public Main()
        {
            this.InitializeComponent();
            InitializeLocator();
            this.Loaded += ReadPointsList_Loaded;
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
            // Specify a known location.
            Geopoint snPoint = new Geopoint(location);

            // Create a MapIcon.
            MapIcon mapIcon1 = new MapIcon();
            mapIcon1.Location = snPoint;
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon1.Title = "I'm here";
            mapIcon1.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/customicon.png"));
            mapIcon1.ZIndex = 0;

            // Add the MapIcon to the map.
            mapWithMyLocation.MapElements.Add(mapIcon1);

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



        private void ReadPointsList_Loaded(object sender, RoutedEventArgs e)
        {
            //ReadAllPointsList dbcpoints = new ReadAllPointsList();
            //BasicGeoposition location = new BasicGeoposition();

            DB_PointsList = dbcpoints.GetAllPoints();//Get all DB points 

            listBoxobj.ItemsSource = DB_PointsList.OrderByDescending(i => i.Id).ToList();//Binding DB data to LISTBOX and Latest points ID can Display first.  

            foreach (var data in DB_PointsList)
            {
                location.Latitude = Double.Parse(data.latitude);
                location.Longitude = Double.Parse(data.longitude);

                AddMapIcon(location, data.pointName);
            }

            
        }

        private void listPoint_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Button myId = (Button)sender;
            string myIdString = myId.CommandParameter.ToString();
            int value = Int32.Parse(myIdString);

            Points listitem = Db_FindOne.ReadPoint(value) as Points;

            BasicGeoposition location = new BasicGeoposition();
            location.Latitude = Double.Parse(listitem.latitude);
            location.Longitude = Double.Parse(listitem.longitude);
            mapWithMyLocation.Center = new Geopoint(location);
            mapWithMyLocation.ZoomLevel = 15;

        }

        private void listBoxobj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            if (listBoxobj.SelectedIndex != -1)
            {
                Points listitem = listBoxobj.SelectedItem as Points;//Get slected listbox item points ID
                Frame.Navigate(typeof(DetailsPage), listitem);
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
            //LisPointsMenu.Visibility = Visibility.Visible;

            if (LisPointsMenu.Visibility == Visibility.Visible)
            {
                LisPointsMenu.Visibility = Visibility.Collapsed;
            }
            else {
                LisPointsMenu.Visibility = Visibility.Visible;
            }

        }

        private void Grid_GotFocus(object sender, RoutedEventArgs e)
        {
            LisPointsMenu.Visibility = Visibility.Collapsed;
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
            //MapIcon1.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/customicon.png"));
            mapIcon1.ZIndex = 0;

            // Add the MapIcon to the map.
            mapWithMyLocation.MapElements.Add(mapIcon1);
        }



        private async void mapWithMyLocation_MapHolding(MapControl sender, MapInputEventArgs args)
        {
            BasicGeoposition tappedGeoPosition = args.Location.Position;

            // Creates the text box
            var PointName = new TextBox { Margin = new Thickness(10), PlaceholderText = "Point Name" };
            var PointNotes = new TextBox { Margin = new Thickness(10), PlaceholderText = "Point Notes" };

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
                SecondaryButtonText = "Cancel",
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

                    DB_PointsList = dbcpoints.GetAllPoints();//Get all DB points 

                    listBoxobj.ItemsSource = DB_PointsList.OrderByDescending(i => i.Id).ToList();//Binding DB data to LISTBOX and Latest points ID can Display first.

                    AddMapIcon(tappedGeoPosition, PointName.Text);
                }
                else
                {
                    //MessageDialog messageDialog = new MessageDialog("Please fill the field Point Name");//Text should not be empty  
                    //await messageDialog.ShowAsync();
                }


            };

            // Close the app if the user doesn't insert the point
            //_pointDialog.SecondaryButtonClick += delegate { Application.Current.Exit(); };

            // Set the binding to enable/disable the accept button 

            _pointDialog.SetBinding(ContentDialog.IsPrimaryButtonEnabledProperty, new Binding
            {
                Source = PointName,
                Path = new PropertyPath("Point"),
                Mode = BindingMode.TwoWay
            });

            var result = await _pointDialog.ShowAsync();

        }


        private void RoadMap_Click(object sender, RoutedEventArgs e)
        {
            mapWithMyLocation.Style = MapStyle.Road;

        }


        private void Arial3D_Click(object sender, RoutedEventArgs e)
        {
            if (mapWithMyLocation.Style != MapStyle.Aerial3DWithRoads)
            {
                mapWithMyLocation.Style = MapStyle.Aerial3DWithRoads;
                MapLabel.Content = "Map Road";
            }
            else
            {
                mapWithMyLocation.Style = MapStyle.Road;
                MapLabel.Content = "Map Arial 3D";
            }

        }


    }
}
