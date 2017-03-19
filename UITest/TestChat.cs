﻿using System;
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
    public class TestChat
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
            //UserMapper userMapper = UserMapper.getInstance();
            //User user = userMapper.GetUserByFacebookId(113083069215300);
            //User.currentUser = user;

        }

        [Test]
        public void AreChatElementVisible()
        {
            app.Invoke("StartActivityFive");
            app.WaitForElement(c => c.Marked("message_user"));
            app.Flash(c => c.Marked("message_user"));
            app.Flash(c => c.Marked("message_time"));
            app.Flash(c => c.Marked("message_text"));
        }

        [Test]
        public void IsTextVisible()
        {
            app.Invoke("StartActivityFive");
            app.WaitForElement(c => c.Marked("message_text"));
            string msgToEnter = "Hi, how are you doing?";
            app.Tap(c => c.Marked("ChatMsg"));
            app.EnterText(msgToEnter);
            app.Tap(c => c.Marked("sendMsgButton"));
            string enteredMessage = "";
            Assert.AreEqual(enteredMessage, msgToEnter);  
        }
    }
}