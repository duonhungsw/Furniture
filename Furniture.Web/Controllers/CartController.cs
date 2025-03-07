using Furniture.Web.Services;
namespace Furniture.Web.Controllers;

public class CartController(ICartApi cartApi) : Controller
{
    [HttpGet]
    public async Task<IActionResult> ShoppingCart()
    {
        var response = await cartApi.GetCarts();

        if (!response.IsSuccessStatusCode || response.Content == null)
        {
            return View(new List<CartItemDto>()); // Nếu lỗi, trả về danh sách rỗng
        }

        return View(response.Content); // Chỉ truyền danh sách CartItemDto vào View
    }
    [HttpPatch]
    public async Task<IActionResult> UpdateCartItemByQuantity([FromBody] CartUpdateQuantityDto model)
    {
        var response = await cartApi.UpdateCartItemByQuantity(model);

        if (!response)
        {
            return BadRequest("Cập nhật thất bại.");
        }

        return Ok(new { success = true, message = "Cập nhật thành công!" });
    }

}

