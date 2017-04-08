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

            SetContentView(Resource.Layout.MainMenu);

            TextView welcomeText;
            Button createRideButton;
            Button preferencesButton;
            Button myRidesButton;
            Button chatButton;
            Button myProfileButton;
            Button logOutButton;

            welcomeText = FindViewById<TextView>(Resource.Id.mainMenuWelcomeText);
            welcomeText.Text= "Welcome, " + User.currentUser.firstName;

            createRideButton = FindViewById<Button>(Resource.Id.createRideButton);
            preferencesButton = FindViewById<Button>(Resource.Id.preferencesButton);
            myRidesButton = FindViewById<Button>(Resource.Id.myRidesButton);
            chatButton = FindViewById<Button>(Resource.Id.chatButton);
            myProfileButton = FindViewById<Button>(Resource.Id.myProfileButton);
            logOutButton = FindViewById<Button>(Resource.Id.logOutButton);

            createRideButton.Click += delegate
            {
                createRideButton.Pressed = true;
                if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
                {
                    AlertDialog.Builder permissionAlert = new AlertDialog.Builder(this, Resource.Style.AlertDialogCustom);
                    permissionAlert.SetTitle("Permission to access fine location");
                    permissionAlert.SetMessage("To create a new ride, you need to give us permission to access your location");

                    //When user clicks on "Proceed"
                    permissionAlert.SetPositiveButton("Ok", new EventHandler<DialogClickEventArgs>((senderAlert, args) =>
                    {
                        if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
                            Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new string[] { Android.Manifest.Permission.AccessFineLocation }, 1);
                    }));

                    Dialog destinationDialog = permissionAlert.Create();
                    destinationDialog.SetCancelable(false);
                    destinationDialog.SetCanceledOnTouchOutside(false);
                    destinationDialog.Show();
                }
                else
                {
                    Intent i = new Intent(this, typeof(MapActivity));
                    StartActivity(i);
                }  
                           
            };

            preferencesButton.Click += delegate
            {
                preferencesButton.Pressed = true;
                Intent i = new Intent(this, typeof(PreferencesActivity));
                StartActivity(i);
            };

            myRidesButton.Click += delegate
            {
                myRidesButton.Pressed = true;
                Intent i = new Intent(this, typeof(UserCancelRideActivity));
                StartActivity(i);
                //Will later include the code of the "my rides" page
            };

            chatButton.Click += delegate
            {
                chatButton.Pressed = true;
                Intent i = new Intent(this, typeof(ChatListActivity));
                StartActivity(i);
            };

            myProfileButton.Click += delegate
            {
                myProfileButton.Pressed = true;
                Intent i = new Intent(this, typeof(UserProfileActivity));
                i.PutExtra("id", User.currentUser.id.ToString());
                StartActivity(i);
            };

            logOutButton.Click += delegate
            {
                logOutButton.Pressed = true;
                //Will later include code that logs a user out of the application
            };

            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
                Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new string[] { Android.Manifest.Permission.AccessFineLocation }, 1);
        }
    }
}