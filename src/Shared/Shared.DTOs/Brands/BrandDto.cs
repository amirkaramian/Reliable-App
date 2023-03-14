using System.ComponentModel.DataAnnotations;

namespace MyReliableSite.Shared.DTOs.Brands;

public class BrandDto : IDto
{
    public Guid Id { get; set; }
    public string LogoUrl { get; set; }
    public string Base64Logo { get; set; }
    public string CompanyName { get; set; }
    [Required]
    public string Address { get; set; }
    public bool Status { get; set; }
    public string Tenant { get; set; }
    public string Name { get; set; }
    public string ClientAssigned { get; set; }
    public DateTime CreatedOn { get; set; }
    public string TermsOfServiceURL { get; set; }
    public bool TermsOfServiceAgreement { get; set; }
}
