namespace MyReliableSite.Shared.DTOs.Categories;

public class GenerateRandomCategoryRequest : IMustBeValid
{
    public int NSeed { get; set; }
}