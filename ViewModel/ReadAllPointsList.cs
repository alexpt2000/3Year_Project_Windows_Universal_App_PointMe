using SqliteUWP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteUWP.ViewModel
{
    public class ReadAllPointsList
    {
        DatabaseHelperClass Db_Helper = new DatabaseHelperClass();

        public ObservableCollection<Points> GetAllPoints()
        {
            return Db_Helper.ReadAllPoints();
        }
    }
}
