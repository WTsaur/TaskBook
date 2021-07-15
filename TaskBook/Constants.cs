namespace TaskBook
{
    public class Constants
    {
        public static string ItemLowPriColor = "CornflowerBlue";
        public static string ItemHighPriColor = "IndianRed";
        public static string ItemDefaultColor = "SlateGray";
        public static string WebApi_Base = "http://10.0.0.128:5000/TaskLists";
        public static string WebApi_AddOrUpdate = WebApi_Base + "/AddOrUpdate";
        public static string WebApi_GetAll = WebApi_Base + "/GetAll";
        public static string WebApi_Delete = WebApi_Base + "/Delete";
        public static string WebApie_SearchLists = WebApi_Base + "/search";
    }
}
