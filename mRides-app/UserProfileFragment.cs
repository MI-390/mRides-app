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
    class UserProfileFragment : DialogFragment
    {

        Button viewProfile;
        Button reviewButton;
        TextView username;
        string userID;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            Bundle args = Arguments;
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.UserProfileFragment, container, false);
            username = view.FindViewById<TextView>(Resource.Id.userProfileFragmentName);
            username.Text = args.GetString("name");
            userID = (args.GetString("id"));
            viewProfile = view.FindViewById<Button>(Resource.Id.viewProfileFragmentButton);
            reviewButton = view.FindViewById<Button>(Resource.Id.reviewFragmentButton);
            viewProfile.Click += ViewProfileButtonClicked;
           //reviewButton.Click += ReviewButtonClicked;
            return view;
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
        }

        // Load a new activity and transfer the data to the new one
        void ViewProfileButtonClicked(object sender, EventArgs e)
        {
            Intent i = new Intent(Context, typeof(UserProfileActivity));
            i.PutExtra("id", userID);
            Context.StartActivity(i);
            Dismiss();
        }

        void ReviewButtonClicked(object sender, EventArgs e)
        {
            //Intent i = new Intent(Context, typeof(Review));
            //i.PutExtra("id", userID);
            //Context.StartActivity(i);
        }

    }
}