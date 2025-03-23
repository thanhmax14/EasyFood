using BusinessLogic.Services.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SellerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("bystore/{storeId}")]
        public IActionResult GetProductsByStore(Guid storeId)
        {
            var products = _productService.GetProductsByStoreId(storeId);
            if (products == null || !products.Any())
            {
                return NotFound(new { message = "No products found for this store." });
            }
            return Ok(products);
        }
    }
}
