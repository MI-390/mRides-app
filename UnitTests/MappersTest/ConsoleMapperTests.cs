using mRides_app.Mappers;
using mRides_app.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnitTests.Mappers;
using static mRides_app.Models.Request;

namespace UnitTests
{
    [TestFixture]
    public class ConsoleMapperTests
    {
        [SetUp]
        public void Setup() { }


        [TearDown]
        public void Tear() { }

        // -------------------------------------------------------------------
        // CONSOLE RELATED TESTS
        // -------------------------------------------------------------------

        [Test]
        public void FindDrivers()
        {
            // Create a driver and create a request that this driver previously posted
            User driver = new User();
            User.currentUser = UserMapper.getInstance().CreateUser(driver);
            Request driverRequest = new Request(0, "45.4928064,-73.5781321", "45.4975281,-73.5789193", DateTime.Now, false, Request.TYPE_DRIVER);
            RequestMapper.getInstance().CreateRequest(driverRequest);

            // Create a rider and set it to be the current user
            // Make a request for this current user and try to find drivers matching this request
            // The above driver should be matched!
            User rider = new User();
            User.currentUser = UserMapper.getInstance().CreateUser(rider);
            Request riderRequest = new Request(0, "45.4928064,-73.5781321", "45.4975281,-73.5789193", DateTime.Now, false, Request.TYPE_RIDER);
            List<Request> requests = ConsoleMapper.getInstance().FindDrivers(riderRequest);

            // Ensure that FindDrivers updated the ID of the request we created
            Assert.True(riderRequest.ID != 0);

            // Check the requests list
            Assert.True(requests != null && requests.Count > 0);
        }

        [Test]
        public void FindRiders()
        {
            // Create a rider and create a request that the rider made in the past
            User.currentUser = UserMapper.getInstance().CreateUser(new User());
            Request riderRequest = new Request(0, "45.4928064,-73.5781321", "45.4975281,-73.5789193", DateTime.Now, false, Request.TYPE_RIDER);
            RequestMapper.getInstance().CreateRequest(riderRequest);

            // Now switch to be a driver looking for riders, having the same destination
            User.currentUser = UserMapper.getInstance().CreateUser(new User());
            Request driverRequest = new Request(0, "45.4928064,-73.5781321", "45.4975281,-73.5789193", DateTime.Now, false, Request.TYPE_DRIVER);
            driverRequest.destinationCoordinates = new List<DestinationCoordinate>
            {
                new DestinationCoordinate{coordinate = "45.4928065,-73.5781322" },
                new DestinationCoordinate{coordinate = "45.4928063,-73.5781320" },
                new DestinationCoordinate{coordinate = "45.4928065,-73.5781322" }
            };
            List<Request> response = ConsoleMapper.getInstance().FindRiders(driverRequest);

            // Ensure that the FindRiders() updated the ID of the newly created request
            Assert.True(driverRequest.ID > 0);

            // Ensure that the response received is not null and contains the rider
            Assert.True(response != null && response.Count > 0);
        }

        [Test]
        public void Confirm()
        {
            // Create a driver
            //User.currentUser = MRidesWebApi.CreateUser(new User());
            Assert.True(false);
        }
    }
}