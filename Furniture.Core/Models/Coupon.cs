using System.ComponentModel.DataAnnotations;

namespace Furniture.Core.Models;

public class Coupon : BaseEntity
{
    [Required(ErrorMessage = "Coupon name not required")]
    public required string CouponName { get; set; }
    [Required(ErrorMessage = "Discount not required")]
    public required string Discount { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
