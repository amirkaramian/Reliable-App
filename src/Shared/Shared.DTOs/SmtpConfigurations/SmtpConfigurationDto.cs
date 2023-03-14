namespace MyReliableSite.Shared.DTOs.SmtpConfigurations;

public class SmtpConfigurationDto : IDto
{
    public Guid Id { get; set; }
    public List<BrandSmtpConfigurationDto> BrandSmtpConfigurations { get; set; }
    public int Port { get; set; }
    public bool HttpsProtocol { get; set; }
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string FromName { get; set; }
    public string FromEmail { get; set; }
    public string Signature { get; set; }
    public string CssStyle { get; set; }
    public string HeaderContent { get; set; }
    public string FooterContent { get; set; }
    public string CompanyAddress { get; set; }
    public string Bcc { get; set; }
    public string Tenant { get; set; }
}
