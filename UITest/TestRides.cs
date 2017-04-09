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
    /// Class to test the UI of the 'My Rides' activity page
    /// </summary>
    [TestFixture]
    public class TestRides
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
        /// Test method to verify that the required elements are present on the 'My Rides' page
        /// </summary>
        [Test]
        public void VerifyAllRidesElements()
        {
            app.Invoke("StartMainMenuActivity");
            app.WaitForElement(c => c.Marked("myRidesButton"));
            app.Tap(c => c.Marked("myRidesButton"));

            // Visibility of Trash-can icon
            app.WaitForElement("trashcanButton");
            app.Flash(c => c.Marked("trashcanButton"));

            // Visibility of Skip icon
            app.WaitForElement("skipButton");
            app.Flash(c => c.Marked("skipButton"));

            // Visibility of the listview for the adapter
            app.WaitForElement("list_of_rides");
            app.Flash(c => c.Marked("list_of_rides"));
        }
    }
}
