using MyReliableSite.Shared.DTOs.Storage;
using Newtonsoft.Json;

namespace MyReliableSite.Shared.DTOs.WHMCS;

public class ImportWHMSCRequest : IMustBeValid
{
    public WHMCSFileType WHMCSFileType { get; set; }
    public string ClientId { get; set; }
    public FileUploadRequest JsonFile { get; set; }
    public string Content { get; set; }
}

public class RowValidationErrorDto
{
    public RowValidationErrorDto()
    {
        Errors = new List<ValidationErrorDto>();
    }

    public List<ValidationErrorDto> Errors { get; set; }

    public int Row
    {
        get;
        set;
    }

    public string Content
    {
        get;
        set;
    }
}

public class ValidationErrorDto
{
    public ValidationErrorDto()
    {
    }

    public string Message
    {
        get;
        set;
    }

    /// <summary>
    /// The position of the character the error occurred at.
    /// </summary>
    public int AtCharacter
    {
        get;
        set;
    }

    /// <summary>
    /// The column the error occurred in.
    /// </summary>
    public int Column
    {
        get;
        internal set;
    }
}

public class ImportWHMSCResponse
{
    public bool Success { get; set; }
    public List<RowValidationErrorDto> RowValidationErrors { get; set; }
    public string Content { get; set; }
}

public class WHMSCClientDto
{
    public string Tenant { get; set; }
    public int ID { get; set; }
    [JsonProperty("First Name")]
    public string FirstName { get; set; }
    [JsonProperty("Last Name")]
    public string LastName { get; set; }
    [JsonProperty("Company Name")]
    public string CompanyName { get; set; }
    [JsonProperty("Email")]
    public string Email { get; set; }
    [JsonProperty("Address1")]
    public string Address1 { get; set; }
    [JsonProperty("Address2")]
    public string Address2 { get; set; }
    [JsonProperty("City")]
    public string City { get; set; }
    [JsonProperty("State")]
    public string State { get; set; }
    [JsonProperty("Postcode")]
    public string Postcode { get; set; }
    [JsonProperty("Country")]
    public string Country { get; set; }
    [JsonProperty("Phone Number")]
    public string PhoneNumber { get; set; }
    [JsonProperty("Currency")]
    public string Currency { get; set; }
    [JsonProperty("Client Group ID")]
    public string ClientGroupID { get; set; }
    [JsonProperty("Credit")]
    public decimal Credit { get; set; }
    [JsonProperty("Creation Date")]
    public DateTime CreationDate { get; set; }
    [JsonProperty("Notes")]
    public string Notes { get; set; }
    [JsonProperty("Status")]
    public string Status { get; set; }
}

/*
public class WHMSCClient
{
}

public class WHMSCDomain
{
}

public class WHMSCInvoice
{
}

public class WHMSCTransactions
{
}

public class WHMSCServices
{
}*/

public enum WHMCSFileType
{
    Clients,
    Domains,
    Invoices,
    Transactions,
    Services
}
