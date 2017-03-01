using PointMe;
using SqliteUWP.Model;
using SqliteUWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SqliteUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListPoints : Page
    {
        ObservableCollection<Points> DB_PointsList = new ObservableCollection<Points>();

        public ListPoints()
        {
            this.InitializeComponent();
            this.Loaded += ReadPointsList_Loaded;
        }
        private void ReadPointsList_Loaded(object sender, RoutedEventArgs e)
        {
            ReadAllPointsList dbcpoints = new ReadAllPointsList();
            DB_PointsList = dbcpoints.GetAllPoints();//Get all DB points  
            if (DB_PointsList.Count > 0)
            {
                //btnDelete.IsEnabled = true;
            }
            listBoxobj.ItemsSource = DB_PointsList.OrderByDescending(i => i.Id).ToList();//Binding DB data to LISTBOX and Latest points ID can Display first.  
        }

        private void listBoxobj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxobj.SelectedIndex != -1)
            {
                Points listitem = listBoxobj.SelectedItem as Points;//Get slected listbox item points ID
                Frame.Navigate(typeof(DetailsPage), listitem);
            }
        }

        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            DatabaseHelperClass delete = new DatabaseHelperClass();
            delete.DeleteAllPoint();//delete all DB points
            DB_PointsList.Clear();//Clear collections
            //btnDelete.IsEnabled = false;
            listBoxobj.ItemsSource = DB_PointsList;
        }
        private void AddPoint_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddPage));
        }


        private void ShowMap_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));

        }

    }
}

