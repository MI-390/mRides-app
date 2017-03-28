namespace mRides_app.Constants
{
    /**
     * This class contains constant strings used for the data passed in the intent
     * between activities.
     */
    public static class IntentExtraNames
    {
        public static string UserId { get { return "UserId"; } }
        public static string UserFacebookId { get { return "UserId"; } }
        public static string UserFacebookFirstName { get { return "UserFacebookFirstName"; } }
        public static string UserFacebookLastName { get { return "UserFacebookLastName"; } }
        public static string UserFacebookGender { get { return "UserFacebookGender"; } }
        public static string PreviousActivity { get { return "PreviousActivity"; } }
        
        public static string RouteCoordinatesJson { get { return "RouteCoordinatesJson"; } }
        public static string RequestType { get { return "RequestType"; } }
    }
}