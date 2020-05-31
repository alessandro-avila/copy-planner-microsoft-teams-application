namespace TeamsAppLib.Settings
{
    public static class Constants
    {
        /* 
         * Messages (Info).
         */
        public static readonly string MESSAGE_INFO_PRESSCONNECTBUTTON = "Press the Connect button to connect to Office 365.";
        public static readonly string MESSAGE_INFO_LOADING = "Loading...";
        public static readonly string MESSAGE_INFO_COPY = "Copying...";

        /* 
         * Messages (Error).
         */
        public static readonly string MESSAGE_ERROR_GENERIC = "An error has occurred:";
        public static readonly string MESSAGE_ERROR_APPNOTREGISTERED = "It looks like this app is not registered with Office 365, because a client id wasn't found.";
        public static readonly string MESSAGE_ERROR_CANNOTCONNECTO365 = "Oops! We couldn't connect to Office 365. Check your debug output for errors.";

        /*
         * Messages (Warning).
         */
        public static readonly string MESSAGE_WARNING_NULLARGUMENTS = "Null arguments.";

        /*
         * Messages (Teams-specific).
         */
        public static readonly string MESSAGE_TEAMS_LOADED = "Teams Loaded. Select a Team.";
        public static readonly string MESSAGE_PLANNER_SELECT = "Please, select a Source Planner and a Destination Planner.";
        public static readonly string MESSAGE_PLANNER_LOADED = "Planners loaded. Select a Planner Source and a Planner Destination.";
        public static readonly string MESSAGE_PLANNER_COPIED = "Copy completed. Enjoy!";

        /*
         * Log.
         */
        public static readonly string LOG_LISTENERNAME = "teamsapplog";
        public static readonly string LOG_FILEPATH = @".\";
        public static readonly string LOG_FILENAME = "teamsapp.log";

        /*
         * Exceptions.
         */
        public static readonly string EXCEPTION_HTTPREQUEST = "Error in sending the request.";

        /*
         * HTTP Client Request / Response.
         */
        public static readonly string HTTP_REQUEST_SENDING = "Request: sending.";
        public static readonly string HTTP_REQUEST_SENT = "Request: sent.";
        public static readonly string HTTP_REQUEST_SUCCESS = "Request: success.";
    }
}
