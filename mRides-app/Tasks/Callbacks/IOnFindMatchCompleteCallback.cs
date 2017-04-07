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

namespace mRides_app.Tasks.Callbacks
{
    public interface IOnFindMatchCompleteCallback
    {
        void OnFindMatchComplete(List<Request> requests);
    }
}