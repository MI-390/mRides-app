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

namespace mRides_app
{
    internal class ChatAdapter : BaseAdapter<ChatList>
    {
        private List<ChatList> items;
        private Activity context;
        public ChatAdapter(ChatListActivity context, List<ChatList> items):base()
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

        public override ChatList this[int position]
        {
            get { return items[position]; }

        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (item.user1.id == User.currentUser.id)
            {
                if (view == null) // no view to re-use, create new
                    view = context.LayoutInflater.Inflate(Resource.Layout.ChatRow, null);
                view.FindViewById<TextView>(Resource.Id.Text1).Text = item.user2.firstName+" "+item.user2.lastName;
                view.FindViewById<ImageView>(Resource.Id.Image).SetImageBitmap(UserMapper.getInstance().GetUserFacebookProfilePicture(item.user2.facebookID));
                //view.FindViewById<TextView>(Resource.Id.Text2).Text = item.SubHeading;
                //view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(item.ImageResourceId);
                return view;
            }
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.ChatRow, null);
            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.user1.firstName+" "+item.user1.lastName;
            view.FindViewById<ImageView>(Resource.Id.Image).SetImageBitmap(UserMapper.getInstance().GetUserFacebookProfilePicture(item.user1.facebookID));

            //view.FindViewById<TextView>(Resource.Id.Text2).Text = item.SubHeading;
            // view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(item.ImageResourceId);
            return view;

        }

        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }
    }
}