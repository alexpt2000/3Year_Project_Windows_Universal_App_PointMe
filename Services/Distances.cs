﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace PointMe.Services
{

    /// <summary>  
    /// The distance type to return the results in.  
    /// </summary>  
    /// 

    public enum DistanceType { Miles, Kilometers };
  
    
    
    /// <summary>  
    /// Specifies a Latitude / Longitude point.  
    /// </summary>  

    class Distances
    {

        /// <summary>  
        /// Returns the distance in miles or kilometers of any two  
        /// latitude / longitude points.  
        /// </summary>  

        public double Distance(BasicGeoposition pos1, BasicGeoposition pos2, DistanceType type)
        {
            double R = (type == DistanceType.Miles) ? 3960 : 6371;
            double dLat = this.toRadian(pos2.Latitude - pos1.Latitude);
            double dLon = this.toRadian(pos2.Longitude - pos1.Longitude);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(this.toRadian(pos1.Latitude)) * Math.Cos(this.toRadian(pos2.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;
            return d;
        }


        /// <summary>  
        /// Convert to Radians.  
        /// </summary>  

        private double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }

    }
}
