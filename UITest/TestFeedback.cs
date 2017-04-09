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
    /// Class to test the UI of the LeaveReview activity
    /// </summary>
    [TestFixture]
    public class TestFeedback
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
        /// Method to test that required elements are visibible on the feedback UI
        /// </summary>
        [Test]
        public void VerifyAllFeedbackElements()
        {
            app.Invoke("StartTestFragmentsActivity");
            app.Tap(c => c.Marked("testFragment2"));

            // Visibility of the rating bar
            app.WaitForElement(c => c.Marked("reviewRatingBar1"));
            app.Flash(c => c.Marked("reviewRatingBar1"));

            // Visibility of the comment line
            app.WaitForElement(c => c.Marked("reviewEdit1"));
        }

        /// <summary>
        /// Method to test leaving a feedback on the UI
        /// </summary>
        [Test]
        public void GiveFeedbackToUser()
        {
            app.Invoke("StartTestFragmentsActivity");
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
