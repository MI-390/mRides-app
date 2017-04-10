using mRides_app.Cache;
using mRides_app.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests
{
    /// <summary>
    /// Testing class for the User Cache
    /// </summary>
    [TestFixture]
    public class UserCacheTest
    {
        [SetUp]
        public void Setup() { }


        [TearDown]
        public void Tear() { }

        /// <summary>
        /// Test method for saving the user's preferences
        /// </summary>
        [Test]
        public void SaveUserPreferences()
        {
            UserCache userCache = UserCache.GetInstance();
            bool saveSuccess = userCache.SaveUserPreferences(999, true, false, true, false, User.PREFERENCE_GENDER_ANY);
            Assert.True(saveSuccess);
        }

        /// <summary>
        /// Test method to get the current user's preferences
        /// </summary>
        [Test]
        public void GetCurrentUserPreferences()
        {
            UserCache userCache = UserCache.GetInstance();
            bool saveSuccess = userCache.SaveUserPreferences(999, true, false, true, false, User.PREFERENCE_GENDER_ANY);
            
            Dictionary<string, string> preferences = userCache.GetSavedUserPreferences();

            string userId;
            preferences.TryGetValue(UserCache.USER_ID, out userId);
            Assert.True(userId.Equals("999"));

            string isSmoker;
            preferences.TryGetValue(UserCache.USER_PREFERENCE_ISSMOKER, out isSmoker);
            Assert.True(isSmoker.Equals("True"));

            string isHandicap;
            preferences.TryGetValue(UserCache.USER_PREFERENCE_ISHANDICAP, out isHandicap);
            Assert.True(isHandicap.Equals("False"));

            string hasLuggage;
            preferences.TryGetValue(UserCache.USER_PREFERENCE_HASLUGGAGE, out hasLuggage);
            Assert.True(hasLuggage.Equals("True"));

            string hasPet;
            preferences.TryGetValue(UserCache.USER_PREFERENCE_HASPET, out hasPet);
            Assert.True(hasPet.Equals("False"));

            string genderPref;
            preferences.TryGetValue(UserCache.USER_PREFERENCE_GENDERPREFERENCE, out genderPref);
            Assert.True(genderPref.Equals(User.PREFERENCE_GENDER_ANY));
        }

    }
}