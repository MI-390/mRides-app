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
        // Constant settings for the values of the gender preferences 
        public const string PREFERENCE_GENDER_MALE = "male";
        public const string PREFERENCE_GENDER_FEMALE = "female";
        public const string PREFERENCE_GENDER_ANY = "any";

        // The user object representing the user using the application
        public static User currentUser { get; set; }

        public int id { get; set; }
        public long facebookID { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string prefferedLanguage { get; set; }
        public bool isSmoker { get; set; }
        public bool isHandicap { get; set; }
        public bool hasLuggage { get; set; }
        public bool hasPet { get; set; }
        public string genderPreference { get; set; }
        public int gsd { get; set; }



        public double latitude { get; set; }
        public double longitude { get; set; }
        public string currentType { get; set; }
        public List<Ride> ridesAsDriver { get; set; }
        public List<UserRides> ridesAsRider { get; set; }
        public List<Request> requestsAsDriver { get; set; }
        public List<RiderRequest> requestAsRider { get; set; }

    }
}