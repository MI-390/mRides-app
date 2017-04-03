using mRides_app.Cache;
using mRides_app.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests
{
    [TestFixture]
    public class UserCacheTest
    {
        [SetUp]
        public void Setup() { }


        [TearDown]
        public void Tear() { }

        [Test]
        public void SaveUserPreferences()
        {
            UserCache userCache = UserCache.GetInstance();
            bool saveSuccess = userCache.SaveUserPreferences(999, true, false, true, false, User.PREFERENCE_GENDER_ANY);
            Assert.True(saveSuccess);
        }

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
            Assert.True(isSmoker.Equals("true"));
        }

    }
}