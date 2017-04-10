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
    /// <summary>
    /// Activity class used for UI testing purpose to open fragments
    /// </summary>
    [Activity(Label = "TestFragments", MainLauncher = true)]
    public class TestFragments : Activity, IEditUserSelectionListener
    {
        string userType;
        int numberOfPeople;

        /// <summary>
        /// Method that is invoked upon creation of the activity
        /// </summary>
        /// <param name="savedInstanceState"></param>
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
                Bundle bundleArgs = new Bundle();
                bundleArgs.PutString("id", "8");
                LeaveReviewFragment dialog = new LeaveReviewFragment();
                dialog.Arguments = bundleArgs;
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                dialog.Show(transaction, "Leave review fragment");
            };
        }

        /// <summary>
        /// Method that displays a message showing the usertype and number of people selected
        /// from the UserTypeFragment
        /// </summary>
        /// <param name="type"></param>
        /// <param name="number"></param>
        public void updateUserSelection(string type, int number)
        {
            userType = type;
            numberOfPeople = number;
            Toast.MakeText(ApplicationContext, "User type: " + userType + " Number of people: " + numberOfPeople, ToastLength.Long).Show();
        }
    }
}