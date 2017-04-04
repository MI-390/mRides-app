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
    // This contains a test case for UI testing and can be run on Test Cloud.
    // Steps:
    // 1. Log into the Xamarin Test Cloud using the trial account
    // 2. In Visual Studio, right click 'UITest' project -> click on 'Run in Test Cloud...'
    // Popup should appear, which will open the browser so you can select the devices you want to test the app on
    // 3. Wait for the test to finish
    //
    // To run all the tests in Visual Studio, open the app with the emulator in 'Release' mode
    // Click on 'Test' tab -> 'Run' -> 'All tests'
    [TestFixture]
    public class TestFacebookAuthentication
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
            // Set up the user to David
            UserMapper userMapper = UserMapper.getInstance();
            User user = userMapper.GetUserByFacebookId(113083069215300);
            User.currentUser = user;
        }

        [Test]
        public void FacebookLogin()
        {
            app.WaitForElement(c => c.Marked("loginButton"));
            app.Tap(c => c.Marked("loginButton"));
            Thread.Sleep(3000);
            app.Screenshot("Login");
            app.TapCoordinates(549, 700);
            Thread.Sleep(3000);
            app.EnterText("cvnewggbsc_1487629189@tfbnw.net");
            app.ScrollDownTo(c => c.Css("input#u_0_2"));
            app.EnterText(c => c.Css("input#u_0_2"), "mi-390");
            app.PressEnter();
            app.WaitForElement(c => c.Marked("place_autocomplete_search_input"));
        }
    }
}

