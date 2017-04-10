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

namespace UITests
{
    /// <summary>
    /// Class to test the UI of the map. Note that not everything can be tested automatically since
    /// Google Maps API were used and elements on the map cannot be accessed via UI tests calls
    /// </summary>
    [TestFixture]
    public class TestMap
    {
        AndroidApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp
            .Android
            .ApkFile("D:/Projects/mRides-app/mRides-app/bin/Release//mRides_app.mRides_app-Signed.apk") //CHANGE THIS APK PATH
            .EnableLocalScreenshots().StartApp();
        }

        /// <summary>
        /// Method to test that drive mode screen can be opened successfully
        /// </summary>
        [Test]
        public void EnterDriveMode()
        {
            app.Invoke("StartEnterDrivingMode");
        }

        /// <summary>
        /// Method to test that Map activity can be opened successfully
        /// </summary>
        [Test]
        public void OpenMap()
        {
            app.Invoke("StartMainMenuActivity");
            app.WaitForElement(c => c.Marked("createRideButton"));
            app.Tap(c => c.Marked("createRideButton"));
            app.WaitForElement(c => c.Marked("alertTitle"));
            app.Tap(c => c.Marked("button1"));
            app.WaitForElement(c => c.Marked("place_autocomplete_search_button"));
        }

    }
}
