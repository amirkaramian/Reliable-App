using MyReliableSite.Shared.DTOs.ArticleFeedbacks;
using MyReliableSite.Shared.DTOs.Categories;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Tickets;
using System.Globalization;

namespace MyReliableSite.Shared.DTOs.Notifications;

public class NotificationDto : IDto
{
    public Guid Id { get; set; }
    public Guid? ToUserId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationStatus Status { get; set; }
    public NotificationTargetUserTypes TargetUserTypes { get; set; }
    public DateTime SentAt { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string Tenant { get; set; }
    public bool? IsRead { get; set; }
    public string FullName { get; set; }
    public string UserImage { get; set; }
    public Guid? NotifyModelId { get; set; }
    public List<TicketDto> Tickets { get; set; }
    public List<OrderDto> Orders { get; set; }
    public List<BillDto> Bills { get; set; }
    public List<ProductDto> Products { get; set; }
    public List<ArticleFeedbackDto> ArticleFeedbacks { get; set; }
    public List<CategoryDto> Categories { get; set; }
}

public class File
{
    public string Name { get; set; }
    public string Size { get; set; }
    public string Link { get; set; }
}

public class Images
{
    public string Image1 { get; set; }
    public string Image2 { get; set; }
    public string image3 { get; set; }
}

public class TaskItem
{
    public string TaskTitle { get; set; }
    public string TaskCategory { get; set; }
    public Images Images { get; set; }
    public string Status { get; set; }
}