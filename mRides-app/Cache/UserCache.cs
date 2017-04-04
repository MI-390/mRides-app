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

namespace mRides_app.Cache
{
    /// <summary>
    /// The UserCache class is responsible for managing local persistant data, with the help of either
    /// key value pairs preferences storage, cached filed or sqlite database. The UserCache is only
    /// responsible for managing persistent data related to the User model.
    /// </summary>
    public class UserCache
    {
        // --------------------------------------------------------
        // Constants related to user preferences
        // --------------------------------------------------------
        private const string USER_PREFERENCES_FILENAME = "mRides_UserPreferences";

        public const string USER_ID = "userId";
        public const string USER_PREFERENCE_ISSMOKER = "isSmoker";
        public const string USER_PREFERENCE_ISHANDICAP = "isHandicap";
        public const string USER_PREFERENCE_HASLUGGAGE = "hasLuggage";
        public const string USER_PREFERENCE_HASPET = "hasPet";
        public const string USER_PREFERENCE_GENDERPREFERENCE = "genderPreference";


        // --------------------------------------------------------
        // Single instance of the class
        // --------------------------------------------------------
        private static UserCache instance;

        /// <summary>
        /// Private constructor as this class is a Singleton.
        /// </summary>
        private UserCache() { }

        /// <summary>
        /// Singleton getter
        /// </summary>
        /// <returns>UserCache single instance of this class</returns>
        public static UserCache GetInstance()
        {
            if(instance == null)
            {
                instance = new UserCache();
            }
            return instance;
        }

        /// <summary>
        /// Saves a set of user preferences
        /// </summary>
        /// <param name="userId">User id to whom the preferences belong</param>
        /// <param name="isSmoker">User smoking preference</param>
        /// <param name="isHandicap">User physical disability preference</param>
        /// <param name="hasLuggage">User luggage preference</param>
        /// <param name="hasPet">User pet/animal preference</param>
        /// <param name="genderPreference">User gender preference</param>
        /// <returns>bool: true if the operation was successful, false if failure to write to the persistent storage occurs</returns>
        public bool SaveUserPreferences(long userId, bool isSmoker, bool isHandicap, bool hasLuggage, bool hasPet, string genderPreference)
        {
            // Get the preferences and its editor
            var preferences = Application.Context.GetSharedPreferences(USER_PREFERENCES_FILENAME, FileCreationMode.Private);
            var preferencesEditor = preferences.Edit();

            // Write the preferences of the currently logged in user
            preferencesEditor.PutString(USER_ID, userId.ToString());
            preferencesEditor.PutString(USER_PREFERENCE_ISSMOKER, isSmoker.ToString());
            preferencesEditor.PutString(USER_PREFERENCE_ISHANDICAP, isHandicap.ToString());
            preferencesEditor.PutString(USER_PREFERENCE_HASLUGGAGE, hasLuggage.ToString());
            preferencesEditor.PutString(USER_PREFERENCE_HASPET, hasPet.ToString());
            preferencesEditor.PutString(USER_PREFERENCE_GENDERPREFERENCE, genderPreference);
            return preferencesEditor.Commit();
        }


        /// <summary>
        /// Obtain the preferences saved from the persistent storage as a dictionary, in which the keys are defined
        /// by the USER_PREFERENCE constants of this class, and the values must be converted to their appropriate type.
        /// </summary>
        /// <returns>
        /// Dictionary in which the key is a string, the value is a string. The keys are defined by the USER_PREFERENCE constants of this class.
        /// An empty dictionary is returned if any of the preferences are not found.
        /// </returns>
        public Dictionary<string, string> GetSavedUserPreferences()
        {
            Dictionary<string, string> preferencesDictionary = new Dictionary<string, string>();
            string notFound = "notFound";

            var preferences = Application.Context.GetSharedPreferences(USER_PREFERENCES_FILENAME, FileCreationMode.Private);
            string userId = preferences.GetString(USER_ID, notFound);
            string isSmoker = preferences.GetString(USER_PREFERENCE_ISSMOKER, notFound);
            string isHandicap = preferences.GetString(USER_PREFERENCE_ISHANDICAP, notFound);
            string hasLuggage = preferences.GetString(USER_PREFERENCE_HASLUGGAGE, notFound);
            string hasPet = preferences.GetString(USER_PREFERENCE_HASPET, notFound);
            string genderPreference = preferences.GetString(USER_PREFERENCE_GENDERPREFERENCE, notFound);

            if(userId.Equals(notFound) ||
                isSmoker.Equals(notFound) ||
                isHandicap.Equals(notFound) ||
                hasLuggage.Equals(notFound) ||
                hasPet.Equals(notFound) || 
                genderPreference.Equals(notFound))
            {
                return preferencesDictionary;
            }

            preferencesDictionary.Add(USER_ID, userId);
            preferencesDictionary.Add(USER_PREFERENCE_ISSMOKER, isSmoker);
            preferencesDictionary.Add(USER_PREFERENCE_ISHANDICAP, isHandicap);
            preferencesDictionary.Add(USER_PREFERENCE_HASLUGGAGE, hasLuggage);
            preferencesDictionary.Add(USER_PREFERENCE_HASPET, hasPet);
            preferencesDictionary.Add(USER_PREFERENCE_GENDERPREFERENCE, genderPreference);

            return preferencesDictionary;
        }
    }
}