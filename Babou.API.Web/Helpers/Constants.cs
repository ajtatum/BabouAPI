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

        public struct ShortDomainUrls
        {
            /// <summary>
            /// https://s.babou.io/
            /// </summary>
            public const string BabouIo = "https://s.babou.io/";
            /// <summary>
            /// https://mrvl.co/
            /// </summary>
            public const string MrvlCo = "https://mrvl.co/";
            /// <summary>
            /// https://babou.io/go/
            /// </summary>
            public const string BabouIoGo = "https://babou.io/go/";
            /// <summary>
            /// https://ajt.io/go/
            /// </summary>
            public const string AjtGo = "https://ajt.io/go/";
        }
    }
}
