using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using mRides_app.Models;
using mRides_app.Mappers;

namespace UnitTests.Mappers
{
    /// <summary>
    /// The ride mapper class provides an interface for its users to perform operations on the
    /// </summary>
    public class RideMapper : AbstractMapper
    {
        private RideMapper() { }

        private static RideMapper _instance;
        public static RideMapper getInstance()
        {
            if (_instance == null)
            {
                _instance = new RideMapper();
            }
            return _instance;
        }
    }
}
