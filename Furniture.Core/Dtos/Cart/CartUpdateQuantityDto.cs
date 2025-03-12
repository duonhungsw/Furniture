namespace Furniture.Core;

public class CartUpdateQuantityDto
{
    public Guid CartItemId { get; set; }
    public int Quantity {  get; set; }
}
