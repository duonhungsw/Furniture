namespace Furniture.Core;

public class CartUpdateQuantityDto
{
    public Guid CartId { get; set; }
    public int Quantity {  get; set; }
}
