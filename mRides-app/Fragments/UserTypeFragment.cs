using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using mRides_app.Models;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace mRides_app
{
    /// <summary>
    /// Interface that will be used by MapActivity
    /// </summary>
    public interface IEditUserSelectionListener
    {
        void updateUserSelection(string type, int num);
    }

    /// <summary>
    /// Fragment class for the options of selecting whether user is a driver/rider and number of people
    /// </summary>
    public class UserTypeFragment : DialogFragment
    {
        Button previous;
        Button next;
        Switch switcher;
        Spinner spinner;
        TextView tv1;
        View view;
        Boolean driver; // Keep track if user selected 'Driver' or ' Rider'
        int num = 1; // Keep track of the number selected by the user in the drop-down list
        IEditUserSelectionListener listener;

        /// <summary>
        /// Method for the creation of the fragment's view
        /// </summary>
        /// <param name="inflater"></param>
        /// <param name="container"></param>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            view = inflater.Inflate(Resource.Layout.UserTypeFragment, container, false);

            previous = view.FindViewById<Button>(Resource.Id.CloseButton);
            next = view.FindViewById<Button>(Resource.Id.Next);
            switcher = view.FindViewById<Switch>(Resource.Id.riderOrDriverSwitch);
            spinner = view.FindViewById<Spinner>(Resource.Id.numOfPeople);
            tv1 = view.FindViewById<TextView>(Resource.Id.textView3);

            Dialog.SetCanceledOnTouchOutside(false);

            // Set the toggle to be at the position where the user has last placed
            if (User.currentUser != null)
            {
                if (User.currentUser.currentType == "rider")
                {
                    switcher.Checked = false;
                    driver = false;
                    next.SetBackgroundResource(Resource.Drawable.green_button);
                }
                else
                {
                    next.SetBackgroundResource(Resource.Drawable.red_button);
                    switcher.Checked = true;
                    driver = true;
                    tv1.Visibility = ViewStates.Gone;
                    spinner.Visibility = ViewStates.Gone;
                }
            }

            switcher.CheckedChange += SwitchDriverOrRider;

            // Create an adapter for the spinner
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(view.Context,
                Resource.Array.numbers_array,
                Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            spinner.ItemSelected += SpinnerItemSelected;
            previous.Click += PreviousButtonClicked;
            next.Click += NextButtonClicked;
            return view;
        }

        /// <summary>
        /// Method that is called when an activity is attached to the fragment.
        /// </summary>
        /// <param name="activity">The activity where the fragment was opened</param>
        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            listener = (IEditUserSelectionListener)activity;
        }


        /// <summary>
        /// Method to toggle between rider and driver when the user clicks on the switch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SwitchDriverOrRider(object sender, EventArgs e)
        {
            if (!driver)
            {
                next.SetBackgroundResource(Resource.Drawable.red_button);
                Log.Debug("Driver", "Driver mode");
                tv1.Visibility = ViewStates.Gone;
                spinner.Visibility = ViewStates.Gone;
            }
            else
            {
                next.SetBackgroundResource(Resource.Drawable.green_button);
                Log.Debug("Rider", "Rider mode");
                string numRiders = GetString(Resource.String.numberOfRiders);
                tv1.Visibility = ViewStates.Visible;
                spinner.Visibility = ViewStates.Visible;
            }

            driver = !driver;
        }

        /// <summary>
        /// Method that will store the user's selected number from the spinner
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">An event argument</param>
        void SpinnerItemSelected(object sender, EventArgs e)
        {
            num = Int32.Parse(spinner.SelectedItem.ToString());
        }

        /// <summary>
        /// Method that will set the user type of the user after clicking the next button
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">An event argument</param>
        void NextButtonClicked(object sender, EventArgs e)
        {
            string userType = "";
            string usr_driver = mRides_app.Models.Request.TYPE_DRIVER;
            string usr_rider = mRides_app.Models.Request.TYPE_RIDER;

            if (driver)
            {
                userType = usr_driver;
            }
            else
            {
                userType = usr_rider;
            }

            listener.updateUserSelection(userType, num);
            Dismiss();
        }

        /// <summary>
        /// Method that will close the fragment when the previous button is clicked
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">An event argument</param>
        void PreviousButtonClicked(object sender, EventArgs e)
        {
            Dismiss();
            Toast.MakeText(Activity, "Choose a destination", ToastLength.Short).Show();
        }
    }
}