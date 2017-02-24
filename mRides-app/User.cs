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
    class User
    {
        private int id { get; set; }
        private String firstName { get; set; }
        private String lastName { get; set; }
        private int gsd { get; set; }
        private bool hasLuggage { get; set; }
        private bool isHandicap { get; set; }
        private bool isSmoker { get; set; }
        private String preferredLanguage { get; set; }


    }
}