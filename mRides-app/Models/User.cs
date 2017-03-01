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
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string prefferedLanguage { get; set; }
        public bool isSmoker { get; set; }
        public bool isHandicap { get; set; }
        public bool hasLuggage { get; set; }
        public bool hasPet { get; set; }
        public string genderPreference { get; set; }
        public int gsd { get; set; }
        public List<Ride> ridesAsDriver { get; set; }
        public List<UserRides> ridesAsRider { get; set; }
        public List<Request> requestsAsDriver { get; set; }
        public List<RiderRequest> requestAsRider { get; set; }

        /// <summary>
        /// Default constructor, initialize all attributes to their default values
        /// </summary>
        public User()
        {
            this.id = 0;
            this.facebookID = 0;
            this.lastName = "";
            this.firstName = "";
            this.prefferedLanguage = "";
            this.isSmoker = false;
            this.isHandicap = false;
            this.hasLuggage = false;
            this.hasPet = false;
            this.genderPreference = "any";
            this.gsd = 0;
            this.ridesAsDriver = new List<Ride>();
            this.ridesAsRider = new List<UserRides>();
            this.requestsAsDriver = new List<Request>();
            this.requestAsRider = new List<RiderRequest>();
        }

    }
}