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

using mRides_app.Constants;

namespace mRides_app
{
    [Activity(Label = "PreferencesActivity")]
    public class PreferencesActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string facebookFirstName = Intent.GetStringExtra(IntentExtraNames.UserFacebookFirstName);

            // Set the view to preferences layout
            SetContentView(Resource.Layout.Preferences);

            // Obtain the extra data passed, the username, and display it in the Hi message
            TextView textViewHi = FindViewById<TextView>(Resource.Id.textViewHi);
            string prefHi = GetString(Resource.String.Pref_Hi);
            textViewHi.Text = prefHi + " " + facebookFirstName;

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
            string genderPreference = "";
            int selectedGenderPref = (int)spinnerGender.SelectedItemId;
            if (selectedGenderPref == 0)
            {
                genderPreference = User.PREFERENCE_GENDER_ANY;
            }
            else if (selectedGenderPref == 1)
            {
                genderPreference = User.PREFERENCE_GENDER_MALE;
            }
            else
            {
                genderPreference = User.PREFERENCE_GENDER_FEMALE;
            }
            doneButton.Click += delegate { this.SaveAndContinue(rbSmoker.Checked, rbLuggage.Checked, rbHandicap.Checked, rbPet.Checked, genderPreference); };

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
                string facebookGender = Intent.GetStringExtra(Constants.IntentExtraNames.UserFacebookGender);
                User newUser = new User
                {
                    facebookID = facebookID,
                    firstName = facebookFirstName,
                    lastName = facebookLastName,
                    isSmoker = smoker,
                    hasLuggage = luggage,
                    isHandicap = handicap,
                    hasPet = pet,
                    genderPreference = gender,
                    gender = facebookGender
                };
                User.currentUser = UserMapper.getInstance().CreateUser(newUser);

                // Go to the next activity
                var mapActivity = new Intent(this, typeof(MapActivity));
                mapActivity.PutExtras(Intent);
                StartActivity(mapActivity);
            }

            // Otherwise, the user already exists, update it, and save those changes on
            // the server
            else
            {
                User.currentUser.isSmoker = smoker;
                User.currentUser.hasLuggage = luggage;
                User.currentUser.isHandicap = handicap;
                User.currentUser.genderPreference = gender;
                User.currentUser.hasPet = pet;
                //UserMapper.getInstance().UpdateUser(User.currentUser);

                // Go back to the previous activity
                this.Finish();
            }
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
            public void Click(RadioButton clickedRadioButton)
            {
                if (this.preferenceList.Contains(clickedRadioButton))
                {
                    if (this.selectedPreference != null && !this.selectedPreference.Equals(clickedRadioButton))
                    {
                        this.selectedPreference.Checked = false;
                    }
                    this.selectedPreference = clickedRadioButton;
                }
            }
        }
    }
}