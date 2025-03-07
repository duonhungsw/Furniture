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
    [HttpPatch]
    public async Task<IActionResult> UpdateCartItemByStatus([FromRoute] CartItemDto model)
    {
        var result = await cartApi.UpdateCartItemByStatus(model.Id);
        if (result)
        {
            return Ok(true);
        }
        return BadRequest("Cập nhật thất bại!");
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteCartItem([FromRoute] CartItemDto model)
    {
        try
        {

            bool result = await cartApi.DeleteCartItem(model.Id);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm để xóa." });
            }

            return Ok(new { message = "Xóa thành công!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi xóa sản phẩm: {ex.Message}");
            return StatusCode(500, new { message = "Lỗi server khi xóa sản phẩm.", error = ex.Message });
        }
    }


}

