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
    /// <summary>
    /// Implementation of the Chat List Adapter class
    /// </summary>
    internal class ChatListAdapter : BaseAdapter<ChatList>
    {
        private List<ChatList> items;
        private Activity context;

        /// <summary>
        /// Constructor of ChatListAdapter
        /// </summary>
        /// <param name="context">An instance of chatListActivity</param>
        /// <param name="items">A list of ChatList</param>
        public ChatListAdapter(ChatListActivity context, List<ChatList> items):base()
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
        /// Method to get the number of items in the list
        /// </summary>
        public override int Count
        {
            get { return items.Count; }
        }

        public override ChatList this[int position]
        {
            get { return items[position]; }

        }

        /// <summary>
        /// Method that is returns the view of the adapter
        /// </summary>
        /// <param name="position">Position of the view</param>
        /// <param name="convertView">A View object</param>
        /// <param name="parent">The parent View object</param>
        /// <returns>View object</returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (item.user1.id == User.currentUser.id)
            {
                if (view == null) // no view to re-use, create new
                    view = context.LayoutInflater.Inflate(Resource.Layout.ChatRow, null);
                view.FindViewById<TextView>(Resource.Id.chat_name).Text = item.user2.firstName+" "+item.user2.lastName;
                view.FindViewById<ImageView>(Resource.Id.Image).SetImageBitmap(UserMapper.getInstance().GetUserFacebookProfilePicture(item.user2.facebookID));
                return view;
            }
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.ChatRow, null);
            view.FindViewById<TextView>(Resource.Id.chat_name).Text = item.user1.firstName+" "+item.user1.lastName;
            view.FindViewById<ImageView>(Resource.Id.Image).SetImageBitmap(UserMapper.getInstance().GetUserFacebookProfilePicture(item.user1.facebookID));

            return view;

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