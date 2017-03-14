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
using mRides_app.Mappers;
using mRides_app.Models;

namespace mRides_app
{
    class UserProfileFragment : DialogFragment
    {

        Button viewProfile;
        Button reviewButton;
        Button chatButton;
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
            //BUtton assignments
            viewProfile = view.FindViewById<Button>(Resource.Id.viewProfileFragmentButton);
            reviewButton = view.FindViewById<Button>(Resource.Id.reviewFragmentButton);
            chatButton = view.FindViewById<Button>(Resource.Id.chatActivityButton);
            //Click Delegation
            viewProfile.Click += ViewProfileButtonClicked;
            reviewButton.Click += ReviewButtonClicked;
            chatButton.Click += ChatButtonClicked;
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

            Bundle args = new Bundle();
            args.PutString("id", userID);
            LeaveReviewFragment dialog = new LeaveReviewFragment();
            dialog.Arguments = args;
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            dialog.Show(transaction, "Leave review fragment");
            Dismiss();
        }
        void ChatButtonClicked(object sender, EventArgs e)
        {
            Bundle args = new Bundle();
            args.PutString("ChatName",)
        }

        string createChatName()
        {
            int intUserId = Convert.ToInt32(userID);
            int currentUser = User.currentUser.id;
            if (currentUser < intUserId)
            {
                return currentUser + " & " + userID;
            }
            return userID + " & " + currentUser;
        }

    }
}