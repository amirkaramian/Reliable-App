using MyReliableSite.Shared.DTOs.Storage;

namespace MyReliableSite.Shared.DTOs.Brands;

public class CreateBrandRequest : BrandDto, IMustBeValid
{
    public FileUploadRequest Image { get; set; }
    public bool IsDefault { get; set; }
}
