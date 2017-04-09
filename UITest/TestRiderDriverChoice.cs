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
    /// Class to test the UI of the Rider/Driver activity
    /// </summary>
    [TestFixture]
    public class TestRiderDriverChoice
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
        /// Method to test the selection of number of people in the spinner on the UI
        /// </summary>
        [Test]
        public void SelectNumOfPeople()
        {
            app.Invoke("StartTestFragmentsActivity");
            app.Tap(c => c.Marked("testFragment1"));
            app.WaitForElement(c => c.Marked("riderOrDriverSwitch"));
            app.Tap(c => c.Marked("riderOrDriverSwitch"));
            app.Tap(c => c.Marked("numOfPeople"));
            app.Tap("4");
            Thread.Sleep(4000); // It takes time for 4 to be tapped
            var results = app.Query(c => c.Marked("text1"));
            Assert.AreEqual("4", results[0].Text);
            Assert.AreNotEqual("5", results[0].Text);
            app.Tap("Next");
        }

        /// <summary>
        /// Method to test the selection of rider or driver on the UI
        /// </summary>
        [Test]
        public void SelectDriverOrRider()
        {
            bool isTrue = true;
            bool isFalse = false;

            bool isDriver = false;

            app.Invoke("StartTestFragmentsActivity");
            Thread.Sleep(3000);
            app.Tap(c => c.Marked("testFragment1"));
            app.Tap(c => c.Marked("riderOrDriverSwitch"));

            isDriver = app.Query(c => c.Id("riderOrDriverSwitch").Invoke("isChecked").Value<bool>()).First();

            Thread.Sleep(3000);

            Assert.AreEqual(isDriver, isFalse);

            app.Tap(c => c.Marked("riderOrDriverSwitch"));
            Thread.Sleep(3000);
            isDriver = app.Query(c => c.Id("riderOrDriverSwitch").Invoke("isChecked").Value<bool>()).First();

            Assert.AreEqual(isDriver, isTrue);

            app.Tap("Next");
        }

    }
}
