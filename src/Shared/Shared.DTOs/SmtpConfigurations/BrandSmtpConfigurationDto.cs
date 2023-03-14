namespace MyReliableSite.Shared.DTOs.SmtpConfigurations;

public class BrandSmtpConfigurationDto
{
    public Guid Id { get; set; }
    public Guid BrandId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid SmtpConfigurationId { get; set; }
}