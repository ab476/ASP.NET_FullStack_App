namespace Common.Configurations;

public interface IEmailSettings
{
    string Password { get; set; }
    string SenderEmail { get; set; }
    string SenderName { get; set; }
    int SmtpPort { get; set; }
    string SmtpServer { get; set; }
}

public class EmailSettings : IEmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}