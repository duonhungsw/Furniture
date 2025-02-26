namespace Furniture.Core.Dtos.Cart;

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? UrlImage { get; set; }
    public string? ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public int ProductQuatity { get; set; }
    public bool Status {  get; set; }

}
