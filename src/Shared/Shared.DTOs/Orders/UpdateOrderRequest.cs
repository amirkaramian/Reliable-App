namespace MyReliableSite.Shared.DTOs.Orders;
public class UpdateOrderRequest : IMustBeValid
{
    public string Notes { get; set; }
    public OrderStatus Status { get; set; }
    public List<string> AdminAssignedId { get; set; }
}