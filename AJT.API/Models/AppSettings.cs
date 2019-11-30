using System.Collections.Generic;

namespace AJT.API.Models
{
    public class AppSettings
    {
        public PushBulletSettings PushBullet { get; set; }
        public SlackSettings Slack { get; set; }
        public GoogleCustomSearchSettings GoogleCustomSearch { get; set; }
        public AuthKeySettings AuthKeys { get; set; }
        public string IpStackApiKey { get; set; }
        public string BuildNumber { get; set; }
        public string AdminSafeList { get; set; }
        public ApplicationInsightsSettings ApplicationInsights { get; set; }
    }

    public class ApplicationInsightsSettings
    {
        public string InstrumentationKey { get; set; }
    }

    public class PushBulletSettings
    {
        public string ApiKey { get; set; }
        public string EncryptionKey { get; set; }
    }
    public class SlackSettings
    {
        public string ApiToken { get; set; }
        public string SigningSecret { get; set; }
        public string VerificationToken { get; set; }
    }

    public class GoogleCustomSearchSettings
    {
        public string ApiKey { get; set; }
        public string MarvelCx { get; set; }
        public string DcComicsCx { get; set; }
    }

    public class AuthKeySettings
    {
        public string Default { get; set; }
        public string AppVeyor { get; set; }
    }
}
