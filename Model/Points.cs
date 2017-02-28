using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteUWP.Model
{
    public class Points
    {
        //The Id property is marked as the Primary Key
        [SQLite.Net.Attributes.PrimaryKey, SQLite.Net.Attributes.AutoIncrement]
        public int Id { get; set; }
        public string pointName { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

        public Points()
        {
            //empty constructor
        }
        public Points(string pointName, string latitude, string longitude)
        {
            this.pointName = pointName;
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }
}
