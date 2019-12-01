namespace AJT.API.Models
{
    public class AppSettings
    {
        public string IpStackApiKey { get; set; }
        public string BuildNumber { get; set; }
        public string AdminSafeList { get; set; }
        public EmailSenderSettings EmailSender { get; set; }
        public ApplicationInsightsSettings ApplicationInsights { get; set; }
        public AuthenticationSettings Authentication { get; set; }
        public PushBulletSettings PushBullet { get; set; }
        public SlackSettings Slack { get; set; }
        public GoogleCustomSearchSettings GoogleCustomSearch { get; set; }
        public AuthKeySettings AuthKeys { get; set; }
    }

    public class EmailSenderSettings
    {
        public string FromSenderName { get; set; }
        public string FromUserName { get; set; }
        public string Domain { get; set; }
        public string ApiKey { get; set; }
    }


    public class ApplicationInsightsSettings
    {
        public string InstrumentationKey { get; set; }
    }

    public class AuthenticationSettings
    {
        public AuthGoogle Google { get; set; }
        public MicrosoftAuth Microsoft { get; set; }
        public AuthFacebook Facebook { get; set; }

        public class AuthGoogle
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }

        public class MicrosoftAuth
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }

        public class AuthFacebook
        {
            public string AppId { get; set; }
            public string AppSecret { get; set; }
        }
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
