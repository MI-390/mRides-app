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

namespace mRides_app
{
    public interface IEditUserSelectionListener
    {
        void updateUserSelection(string type, int num);
    }

    public class UserTypeFragment : DialogFragment
    {
        Button previous;
        Button next;
        Switch switcher;
        Spinner spinner;
        TextView tv1;
        Boolean driver = false; // Keep track if user selected 'Driver' or ' Rider'
        String num = ""; // Keep track of the number selected by the user in the drop-down list

        // Static method to create a new instance of this fragment
        public static UserTypeFragment NewInstance(Bundle bundle)
        {
            UserTypeFragment fragment = new UserTypeFragment();
            fragment.Arguments = bundle;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.UserTypeFragment, container, false);
            previous = view.FindViewById<Button>(Resource.Id.CloseButton);
            next = view.FindViewById<Button>(Resource.Id.Next);
            switcher = view.FindViewById<Switch>(Resource.Id.riderOrDriverSwitch);
            spinner = view.FindViewById<Spinner>(Resource.Id.numOfPeople);
            tv1 = view.FindViewById<TextView>(Resource.Id.textView3);

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


        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            listener = (IEditUserSelectionListener)activity;
        }

        // Toggle between rider and driver
        void SwitchDriverOrRider(object sender, EventArgs e)
        {
            if (!driver)
            {
                Log.Debug("Driver", "I am a driver woohoo");
                // Open a session as a Driver?
                tv1.SetText("How many available seats\ndo you have?", TextView.BufferType.Normal);
            }
            else
            {
                Log.Debug("Rider", "I am a rider woohoo");
                // Open a session as a Rider
                tv1.SetText("How many are you?", TextView.BufferType.Normal);
            }

            driver = !driver;
        }
        // Store the number selected from the drop-down list
        void SpinnerItemSelected(object sender, EventArgs e)
        {
            num = spinner.SelectedItem.ToString();
        }
        // Load a new activity and transfer the data to the new one
        void NextButtonClicked(object sender, EventArgs e)
        {
            if (driver)
            {
                Intent myIntent1 = new Intent(view.Context, typeof(DriverMode));
                myIntent1.PutExtra("numOfSeats", num);
                view.Context.StartActivity(myIntent1);
            }
            else
            {
                Intent myIntent2 = new Intent(view.Context, typeof(RiderMode));
                myIntent2.PutExtra("numOfPeople", num);
                view.Context.StartActivity(myIntent2);
            }
        }

        // Close dialog fragment when clicking 'Previous' button
        void PreviousButtonClicked(object sender, EventArgs e)
        {
            Dismiss();
            Toast.MakeText(Activity, "Choose your destination", ToastLength.Short).Show();
        }
    }
}