using mRides_app.Mappers;
using mRides_app.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnitTests.Mappers;
using System.Threading;

namespace UnitTests
{
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

        /**
         * Test whether sending a POST request to the server to create a user is successful
         * and a response is received. This does not test the content of the response.
         */
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

        /**
         * Test whether we can retrieve a user by its Facebook id
         */
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

        /**
         * Test whether we are able to successfully send a request to the server to
         * obtain information about a user. For now assume that there is a user in the database.
         */
        [Test]
        public void GetUser()
        {
            User response = UserMapper.getInstance().GetUser(1);
            Assert.True(response.id == 1);
        }

        /**
         * Test whether we are able to successfully get the reviews of a user
         */
        [Test]
        public void GetReviews()
        {
            UserMapper userMapper = UserMapper.getInstance();
            List<Feedback> reviews = userMapper.GetReviews(2);

            // TODO: Make a better test to ensure that this user actually
            // has a review.
            Assert.True(reviews.Count > 0);
        }

        /**
         * Test whether we are able to leave a review
         */
        [Test]
        public void LeaveReview()
        {
            // Get user mapper
            UserMapper userMapper = UserMapper.getInstance();

            // Create a driver
            User driver = new User
            {
                firstName = "Driver",
                lastName = "Driver",
                genderPreference = "male",
                gsd = 0,
                hasLuggage = false,
                isHandicap = false,
                isSmoker = false,
                hasPet = false,
                prefferedLanguage = "en-ca"
            };
            driver = userMapper.CreateUser(driver);

            // Create a rider
            User rider = new User
            {
                firstName = "Rider",
                lastName = "Rider",
                genderPreference = "male",
                gsd = 0,
                hasLuggage = false,
                isHandicap = false,
                isSmoker = false,
                hasPet = false,
                prefferedLanguage = "en-ca"
            };
            rider = userMapper.CreateUser(rider);

            // Create a ride
            User.currentUser = driver;
            RideMapper rideMapper = RideMapper.getInstance();
            Ride ride = new Ride
            {
                destination = "",
                location = "",
                dateTime = DateTime.Now,
                isWeekly = false,
                Driver = driver,
                DriverID = driver.id,
                type = "driver"
            };
            ride = rideMapper.CreateRide(ride);

            // Add the rider to the ride
            User.currentUser = rider;
            ride = rideMapper.AddRiderToRide(ride.ID);


            // Test the leave review by the driver
            User.currentUser = driver;
            userMapper.LeaveReview(ride.ID, rider.id, 4, "Review from driver " + driver.id);
            List<Feedback> riderReviews = userMapper.GetReviews(rider.id);
            bool successDriverToRiderFeedback = false;
            foreach (Feedback f in riderReviews)
            {
                if (f.givenBy.id == driver.id && f.stars == 4 && f.feedbackText.Equals("Review from driver " + driver.id))
                {
                    successDriverToRiderFeedback = true;
                    break;
                }
            }
            Assert.True(successDriverToRiderFeedback);

            // Test the leave review by the rider
            User.currentUser = rider;
            userMapper.LeaveReview(ride.ID, driver.id, 5, "Review from rider " + rider.id);
            List<Feedback> driverReviews = userMapper.GetReviews(driver.id);
            bool successRiderToDriverFeedback = false;
            foreach (Feedback f in driverReviews)
            {
                if (f.givenBy.id == rider.id && f.stars == 5 && f.feedbackText.Equals("Review from rider " + rider.id))
                {
                    successRiderToDriverFeedback = true;
                    break;
                }
            }
            Assert.True(successRiderToDriverFeedback);
        }

        /**
        * Test whether we are able to successfully get the gender of a user
        */
        [Test]
        public void GetGender()
        {
            UserMapper userMapper = UserMapper.getInstance();
            // Create a test user
            User aline = new User
            {
                id = 7777,
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

            userMapper.CreateUser(aline);
            string testGender = userMapper.getGender(7777);
            Assert.AreEqual(testGender, "female");
        }

        /**
        * Test whether we are able to successfully set the gender of a user
        */
        [Test]
        public void SetGender()
        {
            UserMapper userMapper = UserMapper.getInstance();
            // Create a test user
            User aline = new User
            {
                id = 6666,
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

            userMapper.CreateUser(aline);
            userMapper.setGender(6666, "male");
            string testGender = userMapper.getGender(6666);
            Assert.AreEqual(testGender, "male");
        }

        /**
        * Test whether we are able to successfully get the GSD of a user
        */
        [Test]
        public void GetGSD()
        {
            UserMapper userMapper = UserMapper.getInstance();
            // Create a test user
            User aline = new User
            {
                id = 88888,
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

            userMapper.CreateUser(aline);
            //Thread.Sleep(5000);
            long testGSD = userMapper.GetGSD(88888);
            Assert.AreEqual(testGSD, 15);
        }

        /**
        * Test whether we are able to successfully set the GSD of a user
        */
        [Test]
        public void SetGSD()
        {
            UserMapper userMapper = UserMapper.getInstance();
            // Create a test user
            User aline = new User
            {
                id = 9999,
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

            userMapper.CreateUser(aline);
            //Thread.Sleep(5000);
            userMapper.setGSD(9999, 150);
            long testGSD = userMapper.GetGSD(9999);
            Assert.AreEqual(testGSD, 150);
        }


    }
}