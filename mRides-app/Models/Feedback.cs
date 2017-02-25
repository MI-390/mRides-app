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
        public string feedback { get; set; }
        public string givenAs { get; set; }
        public int givenBy { get; set; }
        public int Ride { get; set; }
    }
}