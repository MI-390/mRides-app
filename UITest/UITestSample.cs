using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xamarin.UITest.Android;
using mRides_app;
using System.Threading;

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
                .EnableLocalScreenshots()
                .StartApp();
        }

        [Test]
        public void FacebookLogin()
        {
            app.Tap(c => c.Marked("button1"));
            Thread.Sleep(5000);
            app.Screenshot("Login");
        }
    }
}

