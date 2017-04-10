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
using mRides_app.MessagingService;
using mRides_app.Models;
using mRides_app.Mappers;
using Android.Locations;

namespace mRides_app
{
    /// <summary>
    /// Adapter class for the list of requests
    /// </summary>
    internal class RequestListAdapter : BaseAdapter<Request>
    {
        private List<Request> items;
        private Activity context;

        /// <summary>
        /// Constructor for RequestListAdapter
        /// </summary>
        /// <param name="context">Current activity</param>
        /// <param name="requests">List of request objects</param>
        public RequestListAdapter(RequestListActivity context, List<Request> items):base()
        {
            this.context = context;
            this.items = items;
        }

        /// <summary>
        /// Method to get ID of the item.
        /// </summary>
        /// <returns>ID of item</returns>
        public override long GetItemId(int position)
        {
            return position;
        }

        /// <summary>
        /// Method to get the number of requests in the list.
        /// </summary>
        public override int Count
        {
            get { return items.Count; }
        }

        public override Request this[int position]
        {
            get { return items[position]; }

        }

        /// <summary>
        /// Method to get the view of one single row element of a request
        /// </summary>
        /// <param name="position">Position of the view</param>
        /// <param name="convertView">A View object</param>
        /// <param name="parent">The parent View object</param>
        /// <returns>View object</returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            Geocoder geocoder = new Geocoder(context);
            if (item.driverID != null)
            {
                if (view == null) // no view to re-use, create new
                    view = context.LayoutInflater.Inflate(Resource.Layout.RequestRow, null);

                view.FindViewById<TextView>(Resource.Id.driver_name).Text = item.driver.firstName+" "+item.driver.lastName;
                view.FindViewById<TextView>(Resource.Id.ride_id).Text = item.ID.ToString();

                IList<Address> addresses = geocoder.GetFromLocation(System.Double.Parse(item.location.Split(',')[0]), System.Double.Parse(item.location.Split(',')[1]), 1);
                view.FindViewById<TextView>(Resource.Id.from).Text = "From: " +addresses[0].GetAddressLine(0);

                addresses = geocoder.GetFromLocation(System.Double.Parse(item.destination.Split(',')[0]), System.Double.Parse(item.destination.Split(',')[1]), 1);
                view.FindViewById<TextView>(Resource.Id.to).Text = "To: "+ addresses[0].GetAddressLine(0);

                return view;
            }
            else return view;

        }

        /// <summary>
        /// Method to get an item given its position in the list.
        /// </summary>
        /// <param name="position">Position of item in list</param>
        /// <returns>Position of item</returns>
        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }
    }
}