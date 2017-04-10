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

    public interface IStartDrivingModeListener
    {
        void enterDriverMode(double latitude, double longitude);
    }

    /// <summary>
    /// Fragment for the user profile view shown on the map.
    /// </summary>
    class UserProfileFragment : DialogFragment
    {

        Button viewProfile;
        Button reviewButton;
        Button chatButton;
        Button pickUpButton;
        TextView username;
        IStartDrivingModeListener listener;
        string userID;
        string location;

        /// <summary>
        /// Method that is invoked upon creation of the fragment
        /// </summary>
        /// <param name="inflater">A LayoutInflater</param>
        /// <param name="container">A ViewGroup</param>
        /// <param name="savedInstanceState">A Bundle</param>
        /// <returns>View object</returns>
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
            location = args.GetString("location");
            viewProfile = view.FindViewById<Button>(Resource.Id.viewProfileFragmentButton);
            reviewButton = view.FindViewById<Button>(Resource.Id.reviewFragmentButton);
            pickUpButton = view.FindViewById<Button>(Resource.Id.pickUpButton);
            chatButton = view.FindViewById<Button>(Resource.Id.chatActivityButton);
            viewProfile.Click += ViewProfileButtonClicked;
            reviewButton.Click += ReviewButtonClicked;
            pickUpButton.Click += PickUpButtonClicked;
            chatButton.Click += ChatButtonClicked;
            return view;
        }

        /// <summary>
        /// Method invoked when activity is attached to the fragment
        /// </summary>
        /// <param name="activity">Activity object</param>
        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            listener = (IStartDrivingModeListener)activity;
        }

        /// <summary>
        /// Method that loads user profile activity and transfers the data to it.
        /// </summary>
        /// <param name="sender">The initiator of this intent request.</param>
        /// <param name="e">The event object initiating the request.</param>
        void ViewProfileButtonClicked(object sender, EventArgs e)
        {
            Intent i = new Intent(Context, typeof(UserProfileActivity));
            i.PutExtra("id", userID);
            Context.StartActivity(i);
            Dismiss();
        }

        /// <summary>
        /// Method that is invoked when the leave a review button is clicked.
        /// </summary>
        /// <param name="sender">The initiator of this intent request.</param>
        /// <param name="e">The event object initiating the request.</param>
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

        /// <summary>
        /// Method that is invoked when the chat button is clicked.
        /// </summary>
        /// <param name="sender">The initiator of this intent request.</param>
        /// <param name="e">The event object initiating the request.</param>
        void ChatButtonClicked(object sender, EventArgs e)
        {
            Intent i = new Intent(Context, typeof(ChatActivity));
            i.PutExtra("ChatName", createChatName());
            i.PutExtra("id", userID);
            Context.StartActivity(i);
            Dismiss();
        }

        /// <summary>
        /// Method that creates the chat name.
        /// </summary>
        /// <returns>Value of the chat name</returns>
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


        /// <summary>
        /// Method that is invoked when the pick up user button is pressed.
        /// </summary>
        /// <param name="sender">The initiator of this intent request.</param>
        /// <param name="e">The event object initiating the request.</param>
        void PickUpButtonClicked(object sender, EventArgs e)
        {
            string[] splitCoordinates = location.Split(',');
            listener.enterDriverMode(Double.Parse(splitCoordinates[0]), Double.Parse(splitCoordinates[1]));
        }

    }
}