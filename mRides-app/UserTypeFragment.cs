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

namespace mRides_app
{
    class UserTypeFragment : DialogFragment
    {
        Button nextButton, previousButton;
        Switch userTypeSwitch;
        Spinner numberOfPeopleSpinner;
        IEditUserSelectionListener listener;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.UserType, container, false);
            nextButton = view.FindViewById<Button>(Resource.Id.Next);
            previousButton = view.FindViewById<Button>(Resource.Id.CloseButton);
            userTypeSwitch = view.FindViewById<Switch>(Resource.Id.riderOrDriverSwitch);
            numberOfPeopleSpinner = view.FindViewById<Spinner>(Resource.Id.numOfPeople);

            nextButton.Click += NextButtonClicked;
            previousButton.Click += PreviousButtonClicked;
            return view;
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            listener = (IEditUserSelectionListener)activity;
        }

        void NextButtonClicked(object sender, EventArgs e)
        {
            string userType = userTypeSwitch.Checked ? userType = "Driver" : userType = "Rider";
            int numberOfPeople = 0;
            listener.updateUserSelection(userType, numberOfPeople);
            Dismiss();
        }

        void PreviousButtonClicked(object sender, EventArgs e)
        {
            Dismiss();
        }
    }

    public interface IEditUserSelectionListener
    {
        void updateUserSelection(string type, int num);
    }
}