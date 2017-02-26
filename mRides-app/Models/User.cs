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
    public class User
    {
        // The user object representing the user using the application
        public static User currentUser { get; set; }

        public int id { get; set; }
        public long facebookID { get; set; }
        public object lastName { get; set; }
        public object firstName { get; set; }
        public object prefferedLanguage { get; set; }
        public bool isSmoker { get; set; }
        public bool isHandicap { get; set; }
        public bool hasLuggage { get; set; }
        public bool hasPet { get; set; }
        public int genderPreference { get; set; }
        public int gsd { get; set; }
        public object ridesAsDriver { get; set; }
        public object ridesAsRider { get; set; }
        public object requestsAsDriver { get; set; }
        public List<object> requestAsRider { get; set; }

    }
}