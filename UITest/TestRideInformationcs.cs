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
            User.currentUser.currentType = "rider";

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

        [Test]
        public void SelectNumOfPeople()
        {
            app.Invoke("StartActivityThree");
            app.Tap(c => c.Marked("testFragment1"));
            app.Tap(c => c.Marked("numOfPeople"));
            app.Tap("4");
            Thread.Sleep(4000); // It takes time for 4 to be tapped
            var results = app.Query(c => c.Marked("text1"));
            Assert.AreEqual("4", results[0].Text);
            Assert.AreNotEqual("5", results[0].Text);
            app.Tap("Next");
        }

        [Test]
        public void SelectDriverOrRider()
        {
            bool isTrue = true;
            bool isFalse = false;

            bool isDriver = false;

            app.Invoke("StartActivityThree");
            Thread.Sleep(3000);
            app.Tap(c => c.Marked("testFragment1"));
            app.Tap(c => c.Marked("riderOrDriverSwitch"));

            isDriver = app.Query(c => c.Id("riderOrDriverSwitch").Invoke("isChecked").Value<bool>()).First();

            Thread.Sleep(3000);

            Assert.AreEqual(isDriver, isTrue);

            app.Tap(c => c.Marked("riderOrDriverSwitch"));
            Thread.Sleep(3000);
            isDriver = app.Query(c => c.Id("riderOrDriverSwitch").Invoke("isChecked").Value<bool>()).First();

            Assert.AreEqual(isDriver, isFalse);

            app.Tap("Next");
        }

    }
}
