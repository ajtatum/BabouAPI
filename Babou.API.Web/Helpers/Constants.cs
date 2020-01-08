namespace Babou.API.Web.Helpers
{
    public struct Constants
    {
        public struct Roles
        {
            public const string Admin = "Admin";
            public const string Member = "Member";
        }

        public struct Applications
        {
            public const int PushBullet = 1;
        }

        public struct ContentTypes
        {
            public const string ApplicationJson = "application/json";
            public const string TextPlain = "text/plain";
        }
    }
}
