using PointMe;
using SqliteUWP.Model;
using SqliteUWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SqliteUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailsPage : Page
    {
        DatabaseHelperClass Db_Helper = new DatabaseHelperClass();
        Points currentPoint = new Points();

        public DetailsPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            currentPoint = e.Parameter as Points;
            //currentcontact = Db_Helper.ReadPoint(Selected_PointId);//Read selected DB point
            pointNametxtBx.Text = currentPoint.pointName;//get point Name
            pointNotestxtBx.Text = currentPoint.notes;
            latitudetxtBx.Text = currentPoint.latitude;
            longitudeTxtBx.Text = currentPoint.longitude;
        }

        private void UpdatePoint_Click(object sender, RoutedEventArgs e)
        {
            currentPoint.pointName = pointNametxtBx.Text;
            currentPoint.notes = pointNotestxtBx.Text;
            currentPoint.latitude = latitudetxtBx.Text;
            currentPoint.longitude = longitudeTxtBx.Text;
            Db_Helper.UpdateDetails(currentPoint);//Update selected DB poin Id
            Frame.Navigate(typeof(ListPoints));
        }

        private void DeletePoint_Click(object sender, RoutedEventArgs e)
        {
            Db_Helper.DeletePoint(currentPoint.Id);//Delete selected DB point Id.
            Frame.Navigate(typeof(ListPoints));
        }


        private void ShowMap_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));

        }
    }
}

