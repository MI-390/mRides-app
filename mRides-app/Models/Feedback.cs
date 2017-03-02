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
    public class Feedback
    {
        public const string GIVEN_AS_RIDER = "rider";
        public const string GIVEN_AS_DRIVER = "driver";

        public string feedback { get; set; }
        public string givenAs { get; set; }
        public User givenBy { get; set; }
        public int ride { get; set; }
        public int stars { get; set; }
        public string time { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Feedback()
        {
            this.feedback = "";
            this.givenAs = GIVEN_AS_RIDER;
            this.givenBy = new Models.User();
            this.ride = 0;
            this.stars = 0;
            this.time = "";
        }
    }
}