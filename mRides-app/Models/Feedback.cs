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
    /// <summary>
    /// Class that corresponds to the ratings and reviews that constitute a feedbck object.
    /// </summary>
    public class Feedback
    {
        public const string GIVEN_AS_RIDER = "rider";
        public const string GIVEN_AS_DRIVER = "driver";

        public string feedbackText { get; set; }
        public string givenAs { get; set; }
        public User givenBy { get; set; }
        public int ride { get; set; }
        public int stars { get; set; }
        public string time { get; set; }
    }
}