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
    /// <summary>
    /// Class to test the UI of the UserProfile activity
    /// </summary>
    [TestFixture]
    public class TestUserProfile
    {
        AndroidApp app;

        /// <summary>
        /// Method that is invoked before the test. A pre-existing user 'David' is used for testing purpose.
        /// </summary>
        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp
            .Android
            .ApkFile("D:/Projects/mRides-app/mRides-app/bin/Release//mRides_app.mRides_app-Signed.apk")
            .EnableLocalScreenshots().StartApp();
        }

        /// <summary>
        /// Test method to verify that the required elements are present on the profile, such as the user name,
        /// the gender, the profile picture, the rating bar, the GSD
        /// </summary>
        [Test]
        public void VerifyAllUserProfileElements()
        {
            app.Invoke("StartMainMenuActivity");
            app.WaitForElement(c => c.Marked("myProfileButton"));
            app.Tap(c => c.Marked("myProfileButton"));
            
            // Visibility of user name
            app.WaitForElement("userName");
            app.Flash(c => c.Marked("userName"));

            // Visibility of gender
            app.WaitForElement(c => c.Marked("genderImage"));
            app.Flash(c => c.Marked("genderImage"));

            // Visibility of profile picture
            app.WaitForElement(c => c.Marked("userPhoto"));
            app.Flash(c => c.Marked("userPhoto"));

            // Visibility of the rating bar
            app.WaitForElement(c => c.Marked("ratingBar"));
            app.Flash("ratingBar");

            // Visibility of the GSD (Good Samaritan Dollar)
            app.WaitForElement("userProfileGSD");
            app.Flash(c => c.Marked("userProfileGSD"));
        }

        /// <summary>
        /// Test method to verify the feedbacks are on the user profile
        /// </summary>
        [Test]
        public void VerifyFeedbackOnProfile()
        {
            app.Invoke("StartMainMenuActivity");
            app.WaitForElement(c => c.Marked("myProfileButton"));
            app.Tap(c => c.Marked("myProfileButton"));

            app.WaitForElement(c => c.Marked("userProfileListView"));
            app.Flash("feedbackDateProfileFragment");
            app.Flash("feedbackFragmentUserName");
            app.Flash("feedbackFragmentRatingBar");
            app.Flash("feedbackFragmentReview");
        }

    }
}
