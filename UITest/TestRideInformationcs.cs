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
    public class TestRideInformation
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
        public void SelectPreferences()
        {
            app.Invoke("StartActivityOne");
            app.Tap(c => c.Marked("radioButtonNonSmoker"));
            app.Tap(c => c.Marked("radioButtonNoLuggage"));
            app.Tap(c => c.Marked("radioButtonNoHandicap"));
            app.Tap(c => c.Marked("radioButtonNoPet"));
            app.Tap(c => c.Marked("text1"));
            app.TapCoordinates(721, 1200);
            app.Tap(c => c.Marked("buttonBack"));
            app.WaitForElement(c => c.Marked("loginButton"));
        }

        [Test]
        public void SelectNumOfPeopleOrSeats()
        {
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment1"));
            app.Tap(c => c.Marked("numOfPeople"));
            app.TapCoordinates(926, 1050);
            app.Tap(c => c.Marked("riderOrDriverSwitch"));
            app.Tap(c => c.Marked("numOfPeople"));
            app.TapCoordinates(926, 1550);
            app.Tap("Next");
        }

        [Test]
        public void SelectDriverOrRider()
        {
            app.Invoke("StartActivityThree");
            Thread.Sleep(3000);
            app.Tap(c => c.Marked("testFragment1"));
            app.Tap(c => c.Marked("riderOrDriverSwitch"));
            app.Flash(c => c.Marked("driver1"));
            app.Tap(c => c.Marked("riderOrDriverSwitch"));
            app.Tap(c => c.Marked("rider1"));
            app.Tap("Next");
        }

    }
}
