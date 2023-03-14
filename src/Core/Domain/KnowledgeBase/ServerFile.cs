using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.KnowledgeBase;

public class ServerFile : AuditableEntity
{
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public string Base64Data { get; set; }

    public ServerFile(string fileName, string fileExtension, string base64Data)
    {
        FileName = fileName;
        FileExtension = fileExtension;
        Base64Data = base64Data;
    }
}
