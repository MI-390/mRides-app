﻿using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xamarin.UITest.Android;

namespace UITest
{
    [TestFixture]
    public class Tests
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
                .ApkFile ("D:/Projects/mRides-app/mRides-app/bin/Release//mRides_app.mRides_app-Signed.apk")
                .EnableLocalScreenshots()
                .StartApp();
        }

        [Test]
        public void AppLaunches()
        {
            app.Screenshot("First screen.");
        }
    }
}

