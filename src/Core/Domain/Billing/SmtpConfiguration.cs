using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class SmtpConfiguration : AuditableEntity, IMustHaveTenant
{
    public Guid BrandId { get; set; }
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
    public ICollection<Brand> Brand { get; set; }
    public List<BrandSmtpConfiguration> BrandSmtpConfigurations { get; set; }
    public SmtpConfiguration()
    {
    }

    public SmtpConfiguration(int port, bool httpsProtocol, string host, string username, string password, string fromName, string fromEmail, string signature, string cssStyle, string headerContent, string footerContent, string companyAddress, string bcc)
    {
        BrandSmtpConfigurations = new List<BrandSmtpConfiguration>();
        Port = port;
        HttpsProtocol = httpsProtocol;
        Host = host;
        Username = username;
        Password = password;
        FromName = fromName;
        FromEmail = fromEmail;
        Signature = signature;
        CssStyle = cssStyle;
        HeaderContent = headerContent;
        FooterContent = footerContent;
        CompanyAddress = companyAddress;
        Bcc = bcc;
    }

    public SmtpConfiguration Update(int port, bool? httpsProtocol, string host, string username, string password, string fromName, string fromEmail, string signature, string cssStyle, string headerContent, string footerContent, string companyAddress, string bcc)
    {
        // if (brandId != Guid.Empty && !string.Equals(BrandId.ToString(), brandId.ToString(), StringComparison.CurrentCultureIgnoreCase)) BrandId = brandId;
        if (port > 0 && Port != port) Port = port;
        if (httpsProtocol.HasValue && HttpsProtocol != httpsProtocol) HttpsProtocol = httpsProtocol.Value;
        if (!string.IsNullOrWhiteSpace(host) && !string.Equals(Host, host, StringComparison.InvariantCultureIgnoreCase)) Host = host;
        if (!string.IsNullOrWhiteSpace(username) && !string.Equals(Username, username, StringComparison.InvariantCultureIgnoreCase)) Username = username;
        if (!string.IsNullOrWhiteSpace(password) && !string.Equals(Password, password, StringComparison.InvariantCultureIgnoreCase)) Password = password;

        if (!string.IsNullOrWhiteSpace(fromName) && !string.Equals(FromName, fromName, StringComparison.InvariantCultureIgnoreCase)) FromName = fromName;
        if (!string.IsNullOrWhiteSpace(fromEmail) && !string.Equals(FromEmail, fromEmail, StringComparison.InvariantCultureIgnoreCase)) FromEmail = fromEmail;
        if (!string.IsNullOrWhiteSpace(signature) && !string.Equals(Signature, signature, StringComparison.InvariantCultureIgnoreCase)) Signature = signature;
        if (!string.IsNullOrWhiteSpace(cssStyle) && !string.Equals(CssStyle, cssStyle, StringComparison.InvariantCultureIgnoreCase)) CssStyle = cssStyle;
        if (!string.IsNullOrWhiteSpace(headerContent) && !string.Equals(HeaderContent, headerContent, StringComparison.InvariantCultureIgnoreCase)) HeaderContent = headerContent;
        if (!string.IsNullOrWhiteSpace(footerContent) && !string.Equals(FooterContent, footerContent, StringComparison.InvariantCultureIgnoreCase)) FooterContent = footerContent;
        if (!string.IsNullOrWhiteSpace(companyAddress) && !string.Equals(CompanyAddress, companyAddress, StringComparison.InvariantCultureIgnoreCase)) CompanyAddress = companyAddress;
        if (!string.IsNullOrWhiteSpace(bcc) && !string.Equals(Bcc, bcc, StringComparison.InvariantCultureIgnoreCase)) Bcc = bcc;
        return this;
    }
}