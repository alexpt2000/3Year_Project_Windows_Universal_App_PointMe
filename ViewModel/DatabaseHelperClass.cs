using PointMe;
using SqliteUWP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteUWP.ViewModel
{
    class DatabaseHelperClass
    {
        //Create Tabble 
        public void CreateDatabase(string DB_PATH)
        {
            if (!CheckFileExists(DB_PATH).Result)
            {
                using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DB_PATH))
                {
                    conn.CreateTable<Points>();

                }
            }
        }


        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }


        // Insert the new Point in the Points table. 
        public void Insert(Points objPoint)
        {
            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
            {
                conn.RunInTransaction(() =>
                {
                    conn.Insert(objPoint);
                });
            }
        }


        // Retrieve the specific Point from the database.   
        public Points ReadPoint(int pointid)
        {
            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
            {
                var existingconact = conn.Query<Points>("select * from Points where Id =" + pointid).FirstOrDefault();
                return existingconact;
            }
        }


        public ObservableCollection<Points> ReadAllPoints()
        {
            try
            {
                using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
                {
                    List<Points> myCollection = conn.Table<Points>().ToList<Points>();
                    ObservableCollection<Points> PointsList = new ObservableCollection<Points>(myCollection);
                    return PointsList;
                }
            }
            catch
            {
                return null;
            }

        }


        //Update existing point 
        public void UpdateDetails(Points ObjPoint)
        {
            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
            {

                var existingconact = conn.Query<Points>("select * from Points where Id =" + ObjPoint.Id).FirstOrDefault();
                if (existingconact != null)
                {

                    conn.RunInTransaction(() =>
                    {
                        conn.Update(ObjPoint);
                    });
                }

            }
        }


        //Delete all points or delete Points table   
        public void DeleteAllPoint()
        {
            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
            {

                conn.DropTable<Points>();
                conn.CreateTable<Points>();
                conn.Dispose();
                conn.Close();

            }
        }


        //Delete specific point   
        public void DeletePoint(int Id)
        {
            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
            {

                var existingpoint = conn.Query<Points>("select * from Points where Id =" + Id).FirstOrDefault();
                if (existingpoint != null)
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.Delete(existingpoint);
                    });
                }
            }
        }
    }
}
