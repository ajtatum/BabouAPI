using System.Collections.Generic;

namespace AJT.API.Web.Models
{
    public class AppSettings
    {
        public string AdminSafeList { get; set; }
        public AzureSettings Azure { get; set; }
        public ApplicationInsightsSettings ApplicationInsights { get; set; }
        public AuthenticationSettings Authentication { get; set; }
        public string BaseShortenedDefaultUrl { get; set; }
        public List<string> BaseShortenedUrls { get; set; }
        public string BuildNumber { get; set; }
        public EmailSenderSettings EmailSender { get; set; }
        public string IpStackApiKey { get; set; }
    }

    public class AzureSettings
    {
        public KeyVaultSettings KeyVault { get; set; }
        public StorageSettings Storage { get; set; }

        public class KeyVaultSettings
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string DataProtectionSecret { get; set; }
            public string EncryptionKey { get; set; }
        }

        public class StorageSettings
        {
            public string ConnectionString { get; set; }
            public string DataProtectionContainer { get; set; }
        }
    }

    public class ApplicationInsightsSettings
    {
        public string InstrumentationKey { get; set; }
    }

    public class AuthenticationSettings
    {
        public AuthFacebook Facebook { get; set; }
        public AuthGoogle Google { get; set; }
        public AuthMicrosoft Microsoft { get; set; }
        public AuthTwitter Twitter { get; set; }

        public class AuthFacebook
        {
            public string AppId { get; set; }
            public string AppSecret { get; set; }
        }

        public class AuthGoogle
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }

        public class AuthMicrosoft
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
        public class AuthTwitter
        {
            public string ConsumerApiKey { get; set; }
            public string ConsumerSecret { get; set; }
        }

    }

    public class EmailSenderSettings
    {
        public string ApiKey { get; set; }
        public string Domain { get; set; }
        public string FromSenderName { get; set; }
        public string FromUserName { get; set; }
        public string ToEmail { get; set; }
    }
}
