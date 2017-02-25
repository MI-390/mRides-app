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

namespace mRides_app
{
    public class User
    {
        public int id { get; private set; }
        public String firstName { get; set; }
        public String lastName { get; set; }
        public int gsd { get; set; }
        public bool hasLuggage { get; set; }
        public bool isHandicap { get; set; }
        public bool isSmoker { get; set; }
        public String preferredLanguage { get; set; }
        public int numOfFeedback { get; set; } //we should add this to DB because it helps for front end


    }
}