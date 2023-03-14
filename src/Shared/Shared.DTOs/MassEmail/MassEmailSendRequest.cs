using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.MassEmail;

public class MassEmailSendRequest
{
    public List<Guid> ProductIds { get; set; }
    public List<Guid> ClientIds { get; set; }
    public string HeaderContent { get; set; }
    public string FooterConent { get; set; }
    public string SignatureContent { get; set; }
    public string EmailBody { get; set; }
    public int NumberOfEmails { get; set; }
    public int IntervalInSeconds { get; set; }
    public Guid SmtpConfigId { get; set; }
    public string Name { get; set; }
    public string EmailAddress { get; set; }
    public string CompanyAddress { get; set; }
    public string CssStyle { get; set; }
}
