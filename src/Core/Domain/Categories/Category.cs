using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.KnowledgeBase;

namespace MyReliableSite.Domain.Categories;

public class Category : AuditableEntity, IMustHaveTenant
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid? ParentCategoryId { get; set; }
    public string CategoryIcon { get; set; }
    public CategoryType CategoryType { get; set; }
    public virtual Category ParentCategory { get; set; }
    public ICollection<ArticleCategory> ArticleCategories { get; set; }

    public string Tenant { get; set; }

    public Category(string name, string description, Guid? parentCategoryId, CategoryType categoryType, string categoryIcon)
    {
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
        CategoryType = categoryType;
        CategoryIcon = categoryIcon;
    }

    public Category Update(string name, string description, Guid? parentCategoryId, string categoryIcon)
    {
        if (name != null && !Name.NullToString().Equals(name)) Name = name;
        if (categoryIcon != null && !CategoryIcon.NullToString().Equals(categoryIcon)) CategoryIcon = categoryIcon;
        if (parentCategoryId != null && ParentCategoryId != parentCategoryId) ParentCategoryId = parentCategoryId;
        if (description != null && !Description.NullToString().Equals(description)) Description = description;
        return this;
    }
}

public enum CategoryType
{
    ProductServices,
    Articles
}