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
    /// Class to test the UI of the Preferences activity
    /// </summary>
    [TestFixture]
    public class TestPreferences
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
        /// A test method for the selection of preferences on the UI
        /// </summary>
        [Test]
        public void SelectPreferences()
        {
            app.Invoke("StartMainMenuActivity");
            app.WaitForElement(c => c.Marked("preferencesButton"));
            app.Tap(c => c.Marked("preferencesButton"));
            app.WaitForElement(c => c.Marked("radioButtonNonSmoker"));

            app.Tap(c => c.Marked("radioButtonNonSmoker"));
            app.Tap(c => c.Marked("radioButtonNoLuggage"));
            app.Tap(c => c.Marked("radioButtonNoHandicap"));
            app.Tap(c => c.Marked("radioButtonNoPet"));
            app.Tap(c => c.Marked("text1"));
            app.Tap("Female");
            Thread.Sleep(4000);
            var results = app.Query(c => c.Marked("text1"));
            // These should be false
            bool valueOfSmokerRadio = app.Query(c => c.Id("radioButtonSmoker").Invoke("isChecked").Value<bool>()).First();
            bool valueOfLuggageRadio = app.Query(c => c.Id("radioButtonLuggage").Invoke("isChecked").Value<bool>()).First();
            bool valueOfHandicapRadio = app.Query(c => c.Id("radioButtonHandicap").Invoke("isChecked").Value<bool>()).First();
            bool valueOfPetRadio = app.Query(c => c.Id("radioButtonPet").Invoke("isChecked").Value<bool>()).First();

            // These should be true, as they are checked
            bool valueOfNonSmokerRadio = app.Query(c => c.Id("radioButtonNonSmoker").Invoke("isChecked").Value<bool>()).First();
            bool valueOfNoLuggageRadio = app.Query(c => c.Id("radioButtonNoLuggage").Invoke("isChecked").Value<bool>()).First();
            bool valueOfNoHandicapRadio = app.Query(c => c.Id("radioButtonNoHandicap").Invoke("isChecked").Value<bool>()).First();
            bool valueOfNoPetRadio = app.Query(c => c.Id("radioButtonNoPet").Invoke("isChecked").Value<bool>()).First();

            bool isTrue = true;
            bool isFalse = false;

            Assert.AreEqual(valueOfSmokerRadio, isFalse);
            Assert.AreEqual(valueOfLuggageRadio, isFalse);
            Assert.AreEqual(valueOfHandicapRadio, isFalse);
            Assert.AreEqual(valueOfPetRadio, isFalse);

            Assert.AreEqual(valueOfNonSmokerRadio, isTrue);
            Assert.AreEqual(valueOfNoLuggageRadio, isTrue);
            Assert.AreEqual(valueOfNoHandicapRadio, isTrue);
            Assert.AreEqual(valueOfNoPetRadio, isTrue);

            Assert.AreNotEqual(valueOfSmokerRadio, isTrue); // Just to see that the opposite also works

            Assert.AreEqual("Female", results[0].Text);
        }

    }
}
