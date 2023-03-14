namespace MyReliableSite.Shared.DTOs.Orders;
public class OrderProductLineItemsDto : IDto
{
    public Guid Id { get; set; }
    public string LineItem { get; set; }
    public decimal Price { get; set; }
    public Guid ProductLineItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? OrderId { get; set; }
}
