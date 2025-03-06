namespace Furniture.Web.Models
{
    public class ProductPagingModel
    {
        public List<ProductDto> Items { set; get; }
        public int PageIndex { set; get; }
        public int PageSize { set; get; }
        public int TotalPages { set; get; }
    }
}
