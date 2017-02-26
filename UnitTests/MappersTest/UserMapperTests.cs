﻿using mRides_app.Mappers;
using mRides_app.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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
            UserMapper userMapper = UserMapper.getInstance();

            // Set a current user
            User.currentUser = userMapper.GetUser(6);

            // TODO: come up with tests that ensure that such rides exist
            userMapper.LeaveReview(1, 1, 4, "Some test reviews");
            Assert.True(true);
        }
    }
}