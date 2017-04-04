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
using mRides_app.Models;
using mRides_app.Mappers;
using Android.Graphics;
using System.Net;
using Xamarin.Auth;
using Android.Graphics.Drawables;

namespace mRides_app
{
    [Activity(Label = "MainMenuActivity")]
    public class MainMenuActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            

        }
    }
}