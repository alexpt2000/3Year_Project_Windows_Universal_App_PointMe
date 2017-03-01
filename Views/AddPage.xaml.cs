using PointMe;
using SqliteUWP.Model;
using SqliteUWP.ViewModel;
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
    public sealed partial class AddPage : Page
    {
 
        public AddPage()
        {
            this.InitializeComponent();
        }



        private async void AddPoint_Click(object sender, RoutedEventArgs e)
        {

            DatabaseHelperClass Db_Helper = new DatabaseHelperClass();//Creating object for DatabaseHelperClass.cs from ViewModel/DatabaseHelperClass.cs  
            if (pointNametxtBx.Text != "" & pointNotestxtBx.Text != "" & latitudetxtBx.Text != "" & LongitudetxtBx.Text != "") 
            {

               

                Db_Helper.Insert(new Points(pointNametxtBx.Text, pointNotestxtBx.Text, latitudetxtBx.Text, LongitudetxtBx.Text)); 
                Frame.Navigate(typeof(MainPage));//after add point redirect to point listbox page  
            } 
            else 
            { 
                MessageDialog messageDialog = new MessageDialog("Please fill the field Point Name");//Text should not be empty  
                await messageDialog.ShowAsync(); 
            } 
        }
    }
}
