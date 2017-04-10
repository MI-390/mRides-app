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
    /// Class to test the MainMenu activity. This class contains only test methods to open each item of the menu
    /// </summary>
    [TestFixture]
    public class TestMainMenu
    {
        AndroidApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp
            .Android
            .ApkFile("D:/Projects/mRides-app/mRides-app/bin/Release//mRides_app.mRides_app-Signed.apk")
            .EnableLocalScreenshots().StartApp();
        }

        /// <summary>
        /// Method to test the 'Create Ride' item in the menu
        /// </summary>
        [Test]
        public void SelectCreateRideItem()
        {
            app.Invoke("StartMainMenuActivity");
            app.WaitForElement(c => c.Marked("createRideButton"));
            app.Tap(c => c.Marked("createRideButton"));
        }

        /// <summary>
        /// Method to test the 'Preferences' item in the menu
        /// </summary>
        [Test]
        public void SelectPreferencesItem()
        {
            app.Invoke("StartMainMenuActivity");
            app.WaitForElement(c => c.Marked("preferencesButton"));
            app.Tap(c => c.Marked("preferencesButton"));
        }

        /// <summary>
        /// Method to test the 'My Rides' item in the menu
        /// </summary>
        [Test]
        public void SelectMyRidesItem()
        {
            app.Invoke("StartMainMenuActivity");
            app.WaitForElement(c => c.Marked("myRidesButton"));
            app.Tap(c => c.Marked("myRidesButton"));
        }

        /// <summary>
        /// Method to test the 'Chat' item in the menu
        /// </summary>
        [Test]
        public void SelectChatItem()
        {
            app.Invoke("StartMainMenuActivity");
            app.WaitForElement(c => c.Marked("chatButton"));
            app.Tap(c => c.Marked("chatButton"));
        }

        /// <summary>
        /// Method to test the 'My Profile' item in the menu
        /// </summary>
        [Test]
        public void SelectMyProfileItem()
        {
            app.Invoke("StartMainMenuActivity");
            app.WaitForElement(c => c.Marked("myProfileButton"));
            app.Tap(c => c.Marked("myProfileButton"));
        }

    }
}
