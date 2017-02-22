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
    public class UserTypeFragment : DialogFragment
    {
        //public override void OnCreate(Bundle savedInstanceState)
        //{
        //    base.OnCreate(savedInstanceState);

        //    // Create your fragment here
        //}

        public static UserTypeFragment NewInstance(Bundle bundle)
        {
            UserTypeFragment fragment = new UserTypeFragment();
            fragment.Arguments = bundle;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Boolean driver = false;
            String num="";
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.UserTypeFragment, container, false);
            Button previous = view.FindViewById<Button>(Resource.Id.CloseButton);
            Button next = view.FindViewById<Button>(Resource.Id.Next);
            Switch switcher = view.FindViewById<Switch>(Resource.Id.riderOrDriverSwitch);
            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.numOfPeople);
            Button button2 = view.FindViewById<Button>(Resource.Id.Next);
            TextView tv1 = view.FindViewById<TextView>(Resource.Id.numOfPeopleText);
            bool check = true;
            switcher.CheckedChange += delegate
            {
                if (check)
                {
                    Log.Debug("Driver", "I am a driver woohoo");
                    // Open a session as a Driver?
                    tv1.SetText("How many available seats\ndo you have?", TextView.BufferType.Normal);
                    driver = true;
                }
                else
                {
                    Log.Debug("Rider", "I am a rider woohoo");
                    // Open a session as a Rider
                    tv1.SetText("How many are you?", TextView.BufferType.Normal);
                    driver = false;
                }

                check = !check;
            };

            // Create an adapter
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(view.Context,
                Resource.Array.numbers_array,
                Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            // Store the number selected
            spinner.ItemSelected += delegate
            {
                num = spinner.SelectedItem.ToString();
            };


            // Close dialog fragment
            previous.Click += delegate
            {
                Dismiss();
                Toast.MakeText(Activity, "Choose your destination", ToastLength.Short).Show();
            };

            // Click next
            next.Click += delegate
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
            };
            return view;
        }
    }
}