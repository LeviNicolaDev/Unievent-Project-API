namespace Unievent.Application.Configurations.Email
{
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
        public string BaseUrl { get; set; } = string.Empty;
        public string ConfirmationBaseUrl { get; set; } = string.Empty;
        public string WebLoginUrl { get; set; } = string.Empty;
        public string MobileLoginDeepLink { get; set; } = string.Empty;
    }
}
