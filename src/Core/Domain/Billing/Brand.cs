using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.Departments;

namespace MyReliableSite.Domain.Billing;

public class Brand : AuditableEntity, IMustHaveTenant
{
    public string Name { get; set; }
    public string ClientAssigned { get; set; }
    public string LogoUrl { get; set; }
    public string CompanyName { get; set; }
    public bool Status { get; set; }
    public bool IsDefault { get; set; }
    public string TermsOfServiceURL { get; set; }
    public bool TermsOfServiceAgreement { get; set; } = true;
    public string Tenant { get; set; }
    public string Address { get; set; }
    public Guid SmtpConfigurationId { get; set; }
    public ICollection<SmtpConfiguration> SmtpConfiguration { get; set; }
    public List<BrandSmtpConfiguration> BrandSmtpConfigurations { get; set; }
    public List<Department> Departments { get; set; }

    public Brand()
    {
    }

    public Brand(string logoUrl, string companyName, bool status, string name, string clientAssigned, bool isDefault, string termsOfServiceURL, bool termsOfServiceAgreement, string address)
    {
        LogoUrl = logoUrl;
        CompanyName = companyName;
        Status = status;
        Name = name;
        ClientAssigned = clientAssigned;
        IsDefault = isDefault;
        TermsOfServiceURL = termsOfServiceURL;
        TermsOfServiceAgreement = termsOfServiceAgreement;
        Address = address;
    }

    public Brand Update(string logoUrl, string companyName, bool status, string name, string clientAssigned, bool isDefault, string termsOfServiceURL, bool termsOfServiceAgreement, string address)
    {
        if (!string.IsNullOrWhiteSpace(logoUrl) && !string.Equals(LogoUrl, logoUrl, StringComparison.InvariantCultureIgnoreCase)) LogoUrl = logoUrl;
        if (!string.IsNullOrWhiteSpace(companyName) && !string.Equals(CompanyName, companyName, StringComparison.InvariantCultureIgnoreCase)) CompanyName = companyName;
        if (!string.IsNullOrWhiteSpace(name) && !string.Equals(Name, name, StringComparison.InvariantCultureIgnoreCase)) Name = name;
        if (!string.IsNullOrWhiteSpace(clientAssigned) && !string.Equals(ClientAssigned, clientAssigned, StringComparison.InvariantCultureIgnoreCase)) ClientAssigned = clientAssigned;
        if (Status != status) Status = status;
        if (IsDefault != isDefault) IsDefault = isDefault;
        if (TermsOfServiceAgreement != termsOfServiceAgreement) TermsOfServiceAgreement = termsOfServiceAgreement;
        if (!string.IsNullOrWhiteSpace(termsOfServiceURL) && !string.Equals(TermsOfServiceURL, termsOfServiceURL, StringComparison.InvariantCultureIgnoreCase)) TermsOfServiceURL = termsOfServiceURL;
        if (!string.IsNullOrWhiteSpace(address) && !string.Equals(Address, address, StringComparison.InvariantCultureIgnoreCase)) Address = address;

        return this;
    }
}