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

namespace mRides_app
{
    [Activity(Label = "PreferencesActivity")]
    public class PreferencesActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            UserMapper userMapper = UserMapper.getInstance();

            // If the previous activity is the main activity and the user already exists,
            // skip this activity
            long facebookId = Convert.ToInt64(Intent.GetStringExtra(GetString(Resource.String.ExtraData_FacebookId)));
            User currentUser = userMapper.GetUserByFacebookId(facebookId);
            string previousActivity = Intent.GetStringExtra(GetString(Resource.String.ExtraData_PreviousActivity));

            // The user does not exist, create it
            string userName = Intent.GetStringExtra(GetString(Resource.String.ExtraData_UserName));
            string[] names = userName.Split(new char[] { ' ' }, 2);
            currentUser = new User
            {
                facebookID = facebookId,
                firstName = names[0],
                lastName = names[1]
            };
            User.currentUser = userMapper.CreateUser(currentUser);

            // Set the view to preferences layout
            SetContentView(Resource.Layout.Preferences);

            // Obtain the extra data passed, the username, and display it in the Hi message
            TextView textViewHi = FindViewById<TextView>(Resource.Id.textViewHi);
            string prefHi = GetString(Resource.String.Pref_Hi);
            textViewHi.Text = prefHi + " " + userName;

            // Set the back button to go back to the previous activity
            Button backButton = FindViewById<Button>(Resource.Id.buttonBack);
            backButton.Click += delegate { this.Finish(); };

            // Manually set the radio button groups since the radiogroup in the layout
            // is considered as a layout, and will not display the options properly
            // TODO: Find a way to implement this through the layout
            RadioButton rbSmoker = FindViewById<RadioButton>(Resource.Id.radioButtonSmoker);
            RadioButton rbNonSmoker = FindViewById<RadioButton>(Resource.Id.radioButtonNonSmoker);
            RadioButton rbLuggage = FindViewById<RadioButton>(Resource.Id.radioButtonLuggage);
            RadioButton rbNoLuggage = FindViewById<RadioButton>(Resource.Id.radioButtonNoLuggage);
            RadioButton rbHandicap = FindViewById<RadioButton>(Resource.Id.radioButtonHandicap);
            RadioButton rbNoHandicap = FindViewById<RadioButton>(Resource.Id.radioButtonNoHandicap);
            RadioButton rbPet = FindViewById<RadioButton>(Resource.Id.radioButtonPet);
            RadioButton rbNoPet = FindViewById<RadioButton>(Resource.Id.radioButtonNoPet);

            // Create the preference sets that acts as radio button groups
            PreferenceSet smokerPreferences = new PreferenceSet();
            PreferenceSet luggagePreferences = new PreferenceSet();
            PreferenceSet handicapPreferences = new PreferenceSet();
            PreferenceSet petPreferences = new PreferenceSet();

            // Add the preferences to the sets
            smokerPreferences.AddPreference(rbSmoker);
            smokerPreferences.AddPreference(rbNonSmoker);
            luggagePreferences.AddPreference(rbLuggage);
            luggagePreferences.AddPreference(rbNoLuggage);
            handicapPreferences.AddPreference(rbHandicap);
            handicapPreferences.AddPreference(rbNoHandicap);
            petPreferences.AddPreference(rbPet);
            petPreferences.AddPreference(rbNoPet);

            // Set the default checked values
            rbSmoker.Checked = true;
            rbLuggage.Checked = true;
            rbHandicap.Checked = true;
            rbPet.Checked = true;
            smokerPreferences.Click(rbSmoker);
            luggagePreferences.Click(rbLuggage);
            handicapPreferences.Click(rbHandicap);
            petPreferences.Click(rbPet);

            // Populate the gender spinner
            Spinner spinnerGender = FindViewById<Spinner>(Resource.Id.spinnerGender);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.Pref_Gender, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerGender.Adapter = adapter;

            // Set the done button to save and continue to the next activity
            Button doneButton = FindViewById<Button>(Resource.Id.buttonDone);
            doneButton.Click += delegate { this.SaveAndContinue(rbSmoker.Checked, rbLuggage.Checked, rbHandicap.Checked, rbPet.Checked, (int)spinnerGender.SelectedItemId); };
            
        }

        private void SaveAndContinue(Boolean smoker, Boolean luggage, Boolean handicap, Boolean pet, int gender)
        {
            // TODO: Save the preferences
            Console.WriteLine("Smoker=" + smoker + ";Luggage=" + luggage + ";handicap=" + handicap + ";gender=" + gender + ";pet=" + pet);
            User.currentUser.isSmoker = smoker;
            User.currentUser.hasLuggage = luggage;
            User.currentUser.isHandicap = handicap;
            User.currentUser.genderPreference = gender;
            User.currentUser.hasPet = pet;
            //User.currentUser.save();
            
            this.Continue();
        }

        private void Continue()
        {
            // Go to the next activity
            var mapActivity = new Intent(this, typeof(MapActivity));
            mapActivity.PutExtras(Intent);
            StartActivity(mapActivity);
        }

        /**
         * Represents a radio button group
         */
        private class PreferenceSet
        {
            private List<RadioButton> preferenceList = new List<RadioButton>();
            private RadioButton selectedPreference;

            /**
             * Add a new preference to the set
             */ 
            public void AddPreference(RadioButton newPreference)
            {
                // Add it to the set of preferences 
                this.preferenceList.Add(newPreference);

                // Register the click event
                newPreference.Click += delegate
                {
                    this.Click(newPreference);
                };
            }

            /**
             * Updates which button is currently clicked, and unclicked the previous clicked
             * button
             */ 
            public void Click (RadioButton clickedRadioButton)
            {
                if(this.preferenceList.Contains(clickedRadioButton))
                {
                    if(this.selectedPreference != null && !this.selectedPreference.Equals(clickedRadioButton))
                    {
                        this.selectedPreference.Checked = false;
                    }
                    this.selectedPreference = clickedRadioButton;
                }
            }
        } 
    }
}