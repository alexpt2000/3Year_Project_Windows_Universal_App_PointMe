using PointMe.Services;
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
using Windows.ApplicationModel.DataTransfer;
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
        BasicGeoposition MyLocation = new BasicGeoposition();

        BasicGeoposition ShareLocation = new BasicGeoposition();
        String ShareName;



        public Main()
        {
            this.InitializeComponent();
            InitializeLocator();
            Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;

            //this.Loaded += ReadPointsList_Loaded;
        }


        /// <summary>  
        /// It will interact with the user when requesting access 
        /// authorization to use the location of the device.
        /// </summary> 
        private async void InitializeLocator()
        {
            var userPermission = await Geolocator.RequestAccessAsync();

            switch (userPermission)
            {
                case GeolocationAccessStatus.Allowed:

                    //Get positions
                    Geolocator geolocator = new Geolocator();
                    Geoposition locationGPS = await geolocator.GetGeopositionAsync();

                    MyLocation.Latitude = locationGPS.Coordinate.Latitude;
                    MyLocation.Longitude = locationGPS.Coordinate.Longitude;

                    // Load all points on Map
                    ReadPointsList_Loaded();

                    // Point my location
                    MyMap_Location(MyLocation);

                    break;

                case GeolocationAccessStatus.Denied:
                    msgDialog.Content = "I cannot check the location if you don't give me the access to your location...";

                    // Show deault location
                    MyMap_Loaded();
                    await msgDialog.ShowAsync();
                    break;

                case GeolocationAccessStatus.Unspecified:
                    msgDialog.Content = "I got an error while getting location permission. Please try again...";

                    // Show deault location
                    MyMap_Loaded();
                    await msgDialog.ShowAsync();
                    break;

            }

        }


        /// <summary>  
        /// Will show the current position with a custom icon
        /// </summary> 
        private void MyMap_Location(BasicGeoposition location)
        {

            // Specify a known location.
            Geopoint snPoint = new Geopoint(location);

            // Create a MapIcon.
            MapIcon mapIcon1 = new MapIcon();
            mapIcon1.Location = snPoint;
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon1.Title = "I'm here";

            // Geting custon Icon
            mapIcon1.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/customicon.png"));
            mapIcon1.ZIndex = 0;

            // Add the MapIcon to the map.
            mapWithMyLocation.MapElements.Add(mapIcon1);

            // Centre and zoom Me
            mapWithMyLocation.Center = new Geopoint(location);
            mapWithMyLocation.ZoomLevel = 9;

        }


        /// <summary>  
        /// If the location is not available or the user does not 
        /// authorize, the map will have a defult location. 
        /// </summary> 
        private void MyMap_Loaded()
        {
            BasicGeoposition location = new BasicGeoposition();

            location.Latitude = 53.27058;
            location.Longitude = -9.065248;

            mapWithMyLocation.Center = new Geopoint(location);
            mapWithMyLocation.ZoomLevel = 9;
        }


        /// <summary>  
        /// Popular object with points and send to the screen display(Map). 
        /// </summary> 
        private void ReadPointsList_Loaded()
        {
            DB_PointsList = dbcpoints.GetAllPoints();//Get all DB points 

            listBoxobj.ItemsSource = DB_PointsList.OrderByDescending(i => i.Id).ToList();//Binding DB data to LISTBOX and Latest points ID can Display first.  

            foreach (var data in DB_PointsList)
            {
                location.Latitude = Double.Parse(data.latitude);
                location.Longitude = Double.Parse(data.longitude);

                // Add an Icon for each location
                AddMapIcon(location, data.pointName);
            }


        }


        /// <summary>  
        ///  When you select an icon in the points list, the location of the selected 
        ///  item will be sent to the map, the selected point will be centered on the 
        ///  map and the zoom will be applied.
        /// </summary>
        private void listPoint_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Get id by button after parse as int
            Button myId = (Button)sender;
            string myIdString = myId.CommandParameter.ToString();
            int value = Int32.Parse(myIdString);

            // Location on Database
            Points listitem = Db_FindOne.ReadPoint(value) as Points;

            // Get location as string and parse as Double
            BasicGeoposition location = new BasicGeoposition();
            location.Latitude = Double.Parse(listitem.latitude);
            location.Longitude = Double.Parse(listitem.longitude);

            // Centre and Zoom
            mapWithMyLocation.Center = new Geopoint(location);
            mapWithMyLocation.ZoomLevel = 15;

        }


        /// <summary>  
        ///  When you select a name in the points list, the id of the selected item 
        ///  will be sent to the details page to Delete or Refresh.
        /// </summary>
        private void listBoxobj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if (listBoxobj.SelectedIndex != -1)
            {
                Points listitem = listBoxobj.SelectedItem as Points;//Get slected listbox item points ID

                // Send details page and pass the object listitem
                Frame.Navigate(typeof(DetailsPage), listitem);
            }
        }

        /// <summary>  
        /// Open or close the side menu 
        /// </summary> 
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;

        }


        /// <summary>  
        /// Add Icon on the map, based on the location stored in the database 
        /// </summary> 
        private void AddMapIcon(BasicGeoposition location, string pointName)
        {

            // Specify a known location.
            Geopoint snPoint = new Geopoint(location);

            // instance class Distances on Services
            Distances distance = new Distances();

            // Passing Locations to Get distances
            double dist = distance.Distance(location, MyLocation, DistanceType.Kilometers);

            // Create a MapIcon.
            MapIcon mapIcon1 = new MapIcon();
            mapIcon1.Location = snPoint;
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon1.Title = pointName + "\n" + dist.ToString("0.##") + " Km"; //Add name on Icon and format distance two decimal point

            mapIcon1.ZIndex = 0;

            // Add the MapIcon to the map.
            mapWithMyLocation.MapElements.Add(mapIcon1);

        }


        /// <summary>  
        /// Save the position where the map was clicked   
        /// </summary>  
        private async void mapWithMyLocation_MapClick(MapControl sender, MapInputEventArgs args)
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

            };

            // Set the binding to enable/disable the accept button 

            _pointDialog.SetBinding(ContentDialog.IsPrimaryButtonEnabledProperty, new Binding
            {
                Source = PointName,
                Path = new PropertyPath("Point"),
                Mode = BindingMode.TwoWay
            });

            var result = await _pointDialog.ShowAsync();

        }


        /// <summary>  
        /// Change the map Style, Aerial3D or Road  
        /// </summary>  
        private void Arial3D_Click(object sender, RoutedEventArgs e)
        {
            // Verify actual select map Style
            if (mapWithMyLocation.Style != MapStyle.Aerial3DWithRoads)
            {
                mapWithMyLocation.Style = MapStyle.Aerial3DWithRoads;
                MapLabel.Text = "Map Road";
            }
            else
            {
                mapWithMyLocation.Style = MapStyle.Road;
                MapLabel.Text = "Map Arial 3D";
            }

        }

        /// <summary>  
        /// Will center my actual position on map  
        /// </summary>  
        private void WhereIam_Click(object sender, RoutedEventArgs e)
        {
            mapWithMyLocation.Center = new Geopoint(MyLocation);
            mapWithMyLocation.ZoomLevel = 15;
        }


        private void Share_Click(object sender, RoutedEventArgs e)
        {
            // Get id by button after parse as int
            Button myId = (Button)sender;
            string myIdString = myId.CommandParameter.ToString();
            int value = Int32.Parse(myIdString);

            // Location on Database
            Points listitem = Db_FindOne.ReadPoint(value) as Points;

            // Get location as string and parse as Double
            //BasicGeoposition location = new BasicGeoposition();
            ShareLocation.Latitude = Double.Parse(listitem.latitude);
            ShareLocation.Longitude = Double.Parse(listitem.longitude);
            ShareName = listitem.pointName;

            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();

        }

        void MainPage_DataRequested(Windows.ApplicationModel.DataTransfer.DataTransferManager sender, Windows.ApplicationModel.DataTransfer.DataRequestedEventArgs args)
        {

            args.Request.Data.SetText("https://maps.google.com/maps?q=" + ShareLocation.Latitude.ToString() + "," + ShareLocation.Longitude.ToString());

            args.Request.Data.Properties.Title = ShareName;
            
 
        }
    }
}
