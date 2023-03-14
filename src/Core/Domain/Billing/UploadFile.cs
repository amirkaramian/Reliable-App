using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class UploadFile : AuditableEntity, IMustHaveTenant
{
    public string FileName { get; set; }
    public Guid CustomerId { get; set; }
    public string FilePath { get; set; }
    public string FileType { get; set; }
    public DateTime DateUploaded { get; set; }
    public string Tenant { get; set; }

    public UploadFile(string fileName, string filePath, string fileType, in Guid customerId, DateTime dateUploaded)
    {
        FileName = fileName;
        FilePath = filePath;
        FileType = fileType;
        DateUploaded = dateUploaded;
        CustomerId = customerId;
    }

    protected UploadFile()
    {
    }

    public UploadFile Update(string fileName, string filePath, string fileType, in Guid customerId, DateTime dateUploaded)
    {
        if (fileName != null && !FileName.NullToString().Equals(fileName)) FileName = fileName;
        if (filePath != null && !FilePath.NullToString().Equals(filePath)) FilePath = filePath;
        if (DateUploaded != dateUploaded) DateUploaded = dateUploaded;
        if (customerId != Guid.Empty && !CustomerId.NullToString().Equals(customerId)) CustomerId = customerId;
        if (fileType != null && !FileType.NullToString().Equals(fileType)) FileType = fileType;
        return this;
    }
}
