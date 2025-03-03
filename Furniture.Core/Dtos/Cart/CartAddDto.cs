using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furniture.Core.Dtos.Cart
{
    public class CartAddDto
    {
        public Guid ProductId { get; set; }
        public int Quantity {  get; set; }
    }
}
