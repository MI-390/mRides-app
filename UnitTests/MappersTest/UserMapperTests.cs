using mRides_app.Mappers;
using mRides_app.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnitTests.Gateways;
using System.Threading;

namespace UnitTests
{
    /// <summary>
    /// Test class for UserMapper
    /// </summary>
    [TestFixture]
    public class UserMapperTests
    {
        [SetUp]
        public void Setup() { }


        [TearDown]
        public void Tear() { }

        // -------------------------------------------------------------------
        // USER RELATED TESTS
        // -------------------------------------------------------------------

        /// <summary>
        /// Test whether sending a POST request to the server to create a user is successful
        /// and a response is received.This does not test the content of the response.
        /// </summary>
        [Test]
        public void CreateUser()
        {
            User newUser = new User();
            newUser.gsd = 5012;
            newUser.hasLuggage = true;
            newUser.isHandicap = false;
            User response = UserMapper.getInstance().CreateUser(newUser);
            Assert.True(response.gsd == 5012);
            Assert.True(response.hasLuggage);
            Assert.True(!response.isHandicap);
        }

        /// <summary>
        /// Test whether we can retrieve a user by its Facebook id
        /// </summary>
        [Test]
        public void GetUserByFacebookId()
        {
            long facebookId = 10212089607586312;

            // Test when the database is empty
            User response = UserMapper.getInstance().GetUserByFacebookId(99999);
            Assert.True(response == null);

            // Create a mock user
            User newUser = new User
            {
                firstName = "Facebook",
                lastName = "User",
                gsd = 666,
                hasLuggage = true,
                isHandicap = false,
                facebookID = facebookId
            };
            UserMapper.getInstance().CreateUser(newUser);

            // Try to find that user
            User response2 = UserMapper.getInstance().GetUserByFacebookId(facebookId);
            Assert.True(response2.facebookID == facebookId);
        }

        /// <summary>
        /// Test whether we are able to successfully send a request to the server to
        /// obtain information about a user.For now assume that there is a user in the database.
        /// </summary>
        [Test]
        public void GetUser()
        {
            User response = UserMapper.getInstance().GetUser(1);
            Assert.True(response.id == 1);
        }

        /// <summary>
        /// Test whether we are able to successfully get the gender of a user
        /// </summary>
        [Test]
        public void GetGender()
        {
            UserMapper userMapper = UserMapper.getInstance();
            // Create a test user
            User aline = new User
            {
                firstName = "Test",
                lastName = "User",
                genderPreference = "male",
                gsd = 15,
                hasLuggage = false,
                isHandicap = false,
                isSmoker = false,
                hasPet = false,
                prefferedLanguage = "en-ca",
                gender = "female"
            };

            aline = userMapper.CreateUser(aline);
            string testGender = userMapper.getGender(aline.id);
            Assert.AreEqual(testGender, "female");
        }

        /// <summary>
        /// Test whether we are able to successfully set the gender of a user
        /// </summary>
        [Test]
        public void SetGender()
        {
            UserMapper userMapper = UserMapper.getInstance();
            // Create a test user
            User aline = new User
            {
                firstName = "Test",
                lastName = "User",
                genderPreference = "male",
                gsd = 15,
                hasLuggage = false,
                isHandicap = false,
                isSmoker = false,
                hasPet = false,
                prefferedLanguage = "en-ca",
                gender = "female"
            };

            aline = userMapper.CreateUser(aline);
            userMapper.setGender(aline.id, "male");
            string testGender = userMapper.getGender(aline.id);
            Assert.AreEqual(testGender, "male");
        }

        /// <summary>
        /// Test whether we are able to successfully get the GSD of a user
        /// </summary>
        [Test]
        public void GetGSD()
        {
            UserMapper userMapper = UserMapper.getInstance();
            // Create a test user
            User aline = new User
            {
                firstName = "Test",
                lastName = "User",
                genderPreference = "male",
                gsd = 15,
                hasLuggage = false,
                isHandicap = false,
                isSmoker = false,
                hasPet = false,
                prefferedLanguage = "en-ca",
                gender = "female"
            };

            aline = userMapper.CreateUser(aline);
            long testGSD = userMapper.GetGSD(aline.id);
            Assert.AreEqual(testGSD, 15);
        }

        /// <summary>
        /// Test whether we are able to successfully set the GSD of a user
        /// </summary>
        [Test]
        public void SetGSD()
        {
            UserMapper userMapper = UserMapper.getInstance();
            // Create a test user
            User aline = new User
            {
                firstName = "Test",
                lastName = "User",
                genderPreference = "male",
                gsd = 15,
                hasLuggage = false,
                isHandicap = false,
                isSmoker = false,
                hasPet = false,
                prefferedLanguage = "en-ca",
                gender = "female"
            };

            aline = userMapper.CreateUser(aline);
            userMapper.setGSD(aline.id, 150);
            long testGSD = userMapper.GetGSD(aline.id);
            Assert.AreEqual(testGSD, 150);
        }

    }
}