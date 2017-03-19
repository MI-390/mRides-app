using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xamarin.UITest.Android;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using mRides_app.Mappers;
using mRides_app.Models;

namespace UITest
{
    [TestFixture]
    public class TestUserProfile
    {
        AndroidApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            // TODO: If the Android app being tested is included in the solution then open
            // the Unit Tests window, right click Test Apps, select Add App Project
            // and select the app projects that should be tested.
            app = ConfigureApp
            .Android
            .ApkFile("D:/Projects/mRides-app/mRides-app/bin/Release//mRides_app.mRides_app-Signed.apk") //CHANGE THIS APK PATH
            .EnableLocalScreenshots().StartApp();
            UserMapper userMapper = UserMapper.getInstance();
            User user = userMapper.GetUserByFacebookId(113083069215300);
            User.currentUser = user;

        }

        [Test]
        public void IsNameVisibleOnProfile()
        {
            app.Invoke("StartActivityFour");
            app.WaitForElement("userName");
            app.Flash(c => c.Marked("userName"));
        }

        [Test]
        public void IsGenderVisibleOnProfile()
        {
            app.Invoke("StartActivityFour");
            app.WaitForElement(c => c.Marked("genderImage"));
            app.Flash(c => c.Marked("genderImage"));
        }

        [Test]
        public void UserProfilePicture()
        {
            app.Invoke("StartActivityFour");
            app.WaitForElement(c => c.Marked("userPhoto"));
            app.Flash(c => c.Marked("userPhoto"));
        }

        [Test]
        public void IsRatingsVisibleOnProfile()
        {
            app.Invoke("StartActivityFour");
            app.WaitForElement(c => c.Marked("ratingBar"));
            app.Flash("ratingBar");
        }

        [Test]
        public void isFeedbackVisibleOnProfile()
        {
            app.Invoke("StartActivityFour");
            app.WaitForElement(c => c.Marked("userProfileListView"));
            app.Flash("feedbackDateProfileFragment");
            app.Flash("feedbackFragmentUserName");
            app.Flash("feedbackFragmentRatingBar");
            app.Flash("feedbackFragmentReview");
        }

    }
}
