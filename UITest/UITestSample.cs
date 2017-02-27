using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xamarin.UITest.Android;
using mRides_app;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace UITest
{
    // This contains a test case for UI testing and can be run on Test Cloud.
    // You might want to comment out the other unit test classes since it makes the Test Cloud runs forever.
    // Steps:
    // 1. Log into the Xamarin Test Cloud using the trial account
    // 2. In Visual Studio, right click 'UITest' project -> click on 'Run in Test Cloud...'
    // Popup should appear, which will open the browser so you can select the devices you want to test the app on
    // 3. Wait for the test to finish
    //
    // To run all the tests in Visual Studio, open the app with the emulator in 'Release' mode
    // Click on 'Test' tab -> 'Run' -> 'All tests'
    [TestFixture]
    public class UITestSample
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
// TODO: Update this path to point to your Android app and uncomment the
// code if the app is not included in the solution.
.ApkFile("D:/Projects/mi-390/mRides-app/bin/Release//mRides_app.mRides_app-Signed.apk") //CHANGE THIS APK PATH
.EnableLocalScreenshots().StartApp();
        }

        [Test]
        public void FacebookLogin()
        {
            // Start app only once, so that further test can continue
            app.Tap(c => c.Marked("loginButton"));
            Thread.Sleep(3000);
            app.Screenshot("Login");
            app.TapCoordinates(549, 900);
            app.EnterText("cvnewggbsc_1487629189@tfbnw.net");
            app.ScrollDownTo(c => c.Css("input#u_0_2"));
            app.EnterText(c => c.Css("input#u_0_2"), "mi-390");
            app.PressEnter();
        }

        [Test]
        public void SelectPreferences()
        {
            app.WaitForElement(c => c.Marked("loginButton"));
            app.Invoke("StartActivityOne");
            app.Tap(c => c.Marked("radioButtonNonSmoker"));
            app.Tap(c => c.Marked("radioButtonNoLuggage"));
            app.Tap(c => c.Marked("radioButtonNoHandicap"));
            app.Tap(c => c.Marked("radioButtonNoPet"));
            app.Tap(c => c.Marked("text1"));
            app.TapCoordinates(721, 1200);
            app.Tap(c => c.Marked("buttonDone"));
        }

        [Test]
        public void SelectDriverOrRider()
        {
            app.WaitForElement(c => c.Marked("loginButton"));
            app.Invoke("StartActivityThree");
            Thread.Sleep(3000);
            app.Tap(c => c.Marked("testFragment1"));
            app.Tap(c => c.Marked("riderOrDriverSwitch"));
            app.Tap("Next");
        }

        [Test]
        public void SelectNumOfPeopleOrSeats()
        {
            app.WaitForElement(c => c.Marked("loginButton"));
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment1"));
            app.Tap(c => c.Marked("numOfPeople"));
            app.TapCoordinates(926, 550);
            app.Tap(c => c.Marked("riderOrDriverSwitch"));
            app.Tap(c => c.Marked("numOfPeople"));
            app.TapCoordinates(926, 550);
            app.Tap("Next");
        }

        [Test]
        public void GiveFeedbackToUser()
        {
            app.WaitForElement(c => c.Marked("loginButton"));
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment2"));
            app.EnterText(c => c.Marked("driverReviewEdit"), "Best driver!");
            app.Tap(c => c.Marked("ratingBarDriver"));
            app.Tap("submitFeedback1");
        }

        [Test]
        public void isNameVisibleOnProfile()
        {
            app.WaitForElement(c => c.Marked("loginButton"));
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment3"));
            app.Query(c => c.Marked("userName"));
        }

        [Test]
        public void isGenderVisibleOnProfile()
        {
            app.WaitForElement(c => c.Marked("loginButton"));
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment3"));
            app.Query(c => c.Marked("genderImage"));
        }

        [Test]
        public void isProfilePhotoVisibleOnProfile()
        {
            app.WaitForElement(c => c.Marked("loginButton"));
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment3"));
            app.Query(c => c.Marked("userPhoto"));
        }

        [Test]
        public void isRatingsVisibleOnProfile()
        {
            app.WaitForElement(c => c.Marked("loginButton"));
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment3"));
            app.Query(c => c.Marked("ratingBar"));
        }

        [Test]
        public void isFeedbackVisibleOnProfile()
        {
            app.WaitForElement(c => c.Marked("loginButton"));
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment3"));
            app.Query(c => c.Marked("userProfileListView"));
            app.Query("feedbackFragmentReview");
        }

        [Test]
        public void openMap()
        {
            app.Tap(c => c.Marked("loginButton"));
            Thread.Sleep(3000);
            app.Screenshot("Login");
            app.TapCoordinates(549, 900);
            app.EnterText("cvnewggbsc_1487629189@tfbnw.net");
            app.ScrollDownTo(c => c.Css("input#u_0_2"));
            app.EnterText(c => c.Css("input#u_0_2"), "mi-390");
            app.PressEnter();
        }

    }
}

