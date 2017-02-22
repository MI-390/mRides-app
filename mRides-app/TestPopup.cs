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
    [Activity(Label = "mRides", MainLauncher = true)]
    public class AlertScreen : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Alert);

            var simpleButton = FindViewById<Button>(Resource.Id.simpleButton);

            simpleButton.Click += (sender, args) =>
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();
                //Remove fragment else it will crash as it is already added to backstack
                Fragment prev = FragmentManager.FindFragmentByTag("dialog");
                if (prev != null)
                {
                    ft.Remove(prev);
                }

                ft.AddToBackStack(null);

                // Create and show the dialog.
                UserTypeFragment newFragment = UserTypeFragment.NewInstance(null);
                //Add fragment
                newFragment.Show(ft, "dialog");
            };
        }

    }
}