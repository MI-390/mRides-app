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
using Java.Lang;
using mRides_app.Models;
using mRides_app.Mappers;
using Android.Locations;

namespace mRides_app
{
    /// <summary>
    /// Adapter class for the list of rides
    /// </summary>
    public class RidesAdapter : BaseAdapter<Request>
    {
        private List<Request> requests;
        private Activity context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Current activity</param>
        /// <param name="requests">List of request objects</param>
        public RidesAdapter(UserCancelRideActivity context, List<Request> requests) : base()
        {
            this.context = context;
            this.requests = requests;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return requests.Count; }
        }

        public override Request this[int position]
        {
            get { return requests[position]; }

        }

        /// <summary>
        /// Method to get the view of one single row element of a ride
        /// </summary>
        /// <param name="position"></param>
        /// <param name="convertView"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = requests[position];
            View itemView = convertView;
            if (itemView == null) // no view to re-use, create new
                itemView = context.LayoutInflater.Inflate(Resource.Layout.List_Ride, null);

            Button trashCanButton = itemView.FindViewById<Button>(Resource.Id.trashcanButton);

            TextView riderName, location;
            CheckBox deletecheck;

            riderName = itemView.FindViewById<TextView>(Resource.Id.CancelRides_nameRider);
            location = itemView.FindViewById<TextView>(Resource.Id.CancelRides_location);
            deletecheck = itemView.FindViewById<CheckBox>(Resource.Id.CancelRides_Checkbox);
            if (item.driverID != User.currentUser.id)
            {
                riderName.Text = item.driver.firstName + " " + item.driver.lastName;
                Geocoder geocoder = new Geocoder(context);

                IList<Address> addresses = geocoder.GetFromLocation(System.Double.Parse(item.location.Split(',')[0]), System.Double.Parse(item.location.Split(',')[1]), 1);
                location.Text=addresses[0].GetAddressLine(0);
            }
            else
            {
                riderName.Text = item.driver.firstName + " " + item.driver.lastName;
                Geocoder geocoder = new Geocoder(context);

                IList<Address> addresses = geocoder.GetFromLocation(System.Double.Parse(item.location.Split(',')[0]), System.Double.Parse(item.location.Split(',')[1]), 1);
                location.Text = addresses[0].GetAddressLine(0);
            }


        

            return itemView;
        }

        /// <summary>
        /// Customized listener class for checkboxes
        /// </summary>
        private class CheckedChangeListener : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
        {
            private Activity context;

            public CheckedChangeListener(Activity activity)
            {
                context = activity;
            }

            public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
            {
                if (isChecked)
                {
                    string name = (string)buttonView.Tag;
                    string text = string.Format("{0} Checked.", name);
                    Toast.MakeText(context, text, ToastLength.Short).Show(); // for debug purpose
                }
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }
    }
}