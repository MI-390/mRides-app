using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace mRides_app.Models
{
    public class Ride
    {
        public int ID { get; set; }
        public string destination { get; set; }
        public string location { get; set; }
        public DateTime dateTime { get; set; }
        public Boolean isWeekly { get; set; }


        //1 Driver per Ride
        public int? DriverID { get; set; }
        public User Driver { get; set; }

        //Many Riders through an association table
        public ICollection<UserRides> UserRides { get; set; }


        /// <summary>
        /// Default constructor
        /// </summary>
        public Ride ()
        {
            this.ID = 0;
            this.destination = "";
            this.location = "";
            this.dateTime = DateTime.Now;
            this.isWeekly = false;
            this.DriverID = 0;
            this.Driver = new User();
            this.UserRides = new List<UserRides>();
        }

    }
}