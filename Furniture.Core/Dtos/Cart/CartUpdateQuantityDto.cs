namespace Furniture.Core.Dtos.Cart;

public class CartUpdateQuantityDto
{
    public Guid CartId { get; set; }
    public int Quantity {  get; set; }
}
