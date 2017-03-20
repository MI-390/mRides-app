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
    public class TestFeedback
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
            .ApkFile("D:/Projects/mRides/mRides-app/bin/Release//mRides_app.mRides_app-Signed.apk") //CHANGE THIS APK PATH
            .EnableLocalScreenshots().StartApp();
            UserMapper userMapper = UserMapper.getInstance();
            User user = userMapper.GetUserByFacebookId(113083069215300);
            User.currentUser = user;

        }

        [Test]
        public void IsRatingBarVisible()
        {
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment2"));
            app.WaitForElement(c => c.Marked("reviewRatingBar1"));
            app.Flash(c => c.Marked("reviewRatingBar1"));
        }

        [Test]
        public void IsReviewLineVisible()
        {
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment2"));
            app.WaitForElement(c => c.Marked("reviewEdit1"));
        }

        [Test]
        public void GiveFeedbackToUser()
        {
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment2"));
            app.Tap(c => c.Marked("reviewRatingBar1"));
            string uiTest = "Best driver! (UI Test)";
            app.EnterText(c => c.Marked("reviewEdit1"), uiTest);
            var results = app.Query(c => c.Marked("reviewEdit1"));
            app.Tap("submitFeedback1");
            Assert.AreEqual(uiTest, results[0].Text);
            Assert.AreNotEqual("Not working", results[0].Text);
        }

    }
}
