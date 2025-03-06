using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.ViewModels;

namespace ProductsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;

     
        public ProductsController(IProductService productService, IProductImageService productImageService)
        {
            _productService = productService;
            _productImageService = productImageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = new List<ProductsViewModel>();
            var products = await _productService.ListAsync(u => u.IsActive, orderBy: x => x.OrderByDescending(s => s.CreatedDate));

            if (products.Any())
            {
                foreach (var item in products)
                {
                    // Tạo danh sách hình ảnh riêng cho từng sản phẩm
                    var imgList = await _productImageService.ListAsync(i => i.ProductID == item.ID);
                    var Listimg = imgList.Select(i => i.ImageUrl).ToList(); // Lấy danh sách ImageUrl

                    list.Add(new ProductsViewModel
                    {
                        CateID = item.CateID,
                        CreatedDate = item.CreatedDate,
                        ID = item.ID,
                        IsActive = item.IsActive,
                        IsOnSale = item.IsOnSale,
                        LongDescription = item.LongDescription,
                        ManufactureDate = item.ManufactureDate,
                        ModifiedDate = item.ModifiedDate,
                        Name = item.Name,
                        ShortDescription = item.ShortDescription,
                        StoreID = item.StoreID,
                        Img = Listimg // Gán danh sách ảnh cho sản phẩm
                    });
                }
                return Ok(list);
            }
            return BadRequest(false);
        }

    }
}
