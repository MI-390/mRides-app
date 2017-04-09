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
    internal class RequestListAdapter : BaseAdapter<Request>
    {
        private List<Request> items;
        private Activity context;
        public RequestListAdapter(RequestListActivity context, List<Request> items):base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override Request this[int position]
        {
            get { return items[position]; }

        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            Geocoder geocoder = new Geocoder(context);
            if (item.driverID != null)
            {
                if (view == null) // no view to re-use, create new
                    view = context.LayoutInflater.Inflate(Resource.Layout.RequestRow, null);


                view.FindViewById<TextView>(Resource.Id.ride_id).Text = item.ID.ToString();

                IList<Address> addresses = geocoder.GetFromLocation(System.Double.Parse(item.location.Split(',')[0]), System.Double.Parse(item.location.Split(',')[1]), 1);
                view.FindViewById<TextView>(Resource.Id.from).Text = "From: " +addresses[0].GetAddressLine(0);

                addresses = geocoder.GetFromLocation(System.Double.Parse(item.destination.Split(',')[0]), System.Double.Parse(item.destination.Split(',')[1]), 1);
                view.FindViewById<TextView>(Resource.Id.to).Text = "To: "+ addresses[0].GetAddressLine(0);

                return view;
            }
            else return view;
            

        }

        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }
    }
}