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

namespace mRides_app
{
    [Activity(Label = "UserTypeFragmentTest")]
    public class TestFragments : Activity, IEditUserSelectionListener
    {

        string userType;
        int numberOfPeople;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TestFragments);

            Button f1 = FindViewById<Button>(Resource.Id.testFragment1);
            Button f2 = FindViewById<Button>(Resource.Id.testFragment2);

            f1.Click += (sender, args) =>
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                UserTypeFragment dialog = new UserTypeFragment();
                dialog.Show(transaction, "User type fragment");
            };

            f2.Click += (sender, args) =>
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                DriverReview dialog = new DriverReview();
                dialog.Show(transaction, "Driver Review Fragment");
            };
        }

        public void updateUserSelection(string type, int number)
        {
            userType = type;
            numberOfPeople = number;
            Toast.MakeText(ApplicationContext, "User type: " + userType + " Number of people: " + numberOfPeople, ToastLength.Long).Show();
        }
    }
}