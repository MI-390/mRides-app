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

namespace mRides_app.Resources
{
    [Activity(Label = "DriverAcceptDeclineMatchActivity")]
    public class DriverAcceptDeclineMatchActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //going to [name of the destination] INCOMPLETE
             string destinationName = Intent.GetStringExtra.
           

            // Set the view to preferences layout
            SetContentView(Resource.Layout.Preferences);

            // Obtain the extra data passed, the username, and display it in the Hi message
            
            TextView textViewGoingToDestination = FindViewById<TextView>(Resource.Id.textViewGoingToDestination);
            string goinToDestination = GetString(Resource.String.going_to_destination);
            textViewGoingToDestination.Text = goinToDestination + " " + destinationName;

            // Set the back button to go back to the previous activity
            Button backButton = FindViewById<Button>(Resource.Id.buttonBack);
            backButton.Click += delegate { this.Finish(); };

           

      


            //set the accept button to save and continue to the next activity

            Button acceptButton = FindViewById<Button>(Resource.Id.driverMatchButtonAccept);


            //Set the  decline button to save and continue to the next activity 

            Button declineButton = FindViewById<Button>(Resource.Id.driverMatchButtonDecline);
            //...
           declineButton.Click += delegate { this.SaveAndContinue(rbSmoker.Checked, rbLuggage.Checked, rbHandicap.Checked, rbPet.Checked, genderPreference); };

         

           

        }

        private void SaveAndContinue(Boolean smoker, Boolean luggage, Boolean handicap, Boolean pet, string gender)
        {
            // If the current user is null and the previous activity is the main, 
            // create a new user using the preferences
            string previousActivity = Intent.GetStringExtra(Constants.IntentExtraNames.PreviousActivity);
            if (User.currentUser == null && previousActivity.Equals(Constants.ActivityNames.MainActivity))
            {
                long facebookID = Convert.ToInt64(Intent.GetStringExtra(Constants.IntentExtraNames.UserFacebookId));
                string facebookFirstName = Intent.GetStringExtra(Constants.IntentExtraNames.UserFacebookFirstName);
                string facebookLastName = Intent.GetStringExtra(Constants.IntentExtraNames.UserFacebookLastName);
                User newUser = new User
                {
                    facebookID = facebookID,
                    firstName = facebookFirstName,
                    lastName = facebookLastName,
                    isSmoker = smoker,
                    hasLuggage = luggage,
                    isHandicap = handicap,
                    hasPet = pet,
                    genderPreference = gender
                };
                User.currentUser = UserMapper.getInstance().CreateUser(newUser);

                // Go to the next activity
                var mapActivity = new Intent(this, typeof(MapActivity));
                mapActivity.PutExtras(Intent);
                StartActivity(mapActivity);
            }

         
        }

       
    }
}