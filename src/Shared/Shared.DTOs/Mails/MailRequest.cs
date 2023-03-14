namespace MyReliableSite.Shared.DTOs.General.Requests;

public class MailRequest
{
    public List<string> To { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }

    public string From { get; set; }

    public int? AmountLimit { get; set; }

    public int? MinutesToWait { get; set; }
}