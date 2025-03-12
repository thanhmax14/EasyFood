using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.ProductVariantVariants;
using BusinessLogic.Services.StoreDetail;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;

namespace ProductsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;
        private readonly IProductVariantService  _productVariantService;
        private readonly ICategoryService _categoryService;
        private readonly IStoreDetailService  _storeDetailService;

        public ProductsController(IProductService productService, IProductImageService productImageService, IProductVariantService productVariantService, ICategoryService categoryService, IStoreDetailService storeDetailService)
        {
            _productService = productService;
            _productImageService = productImageService;
            _productVariantService = productVariantService;
            _categoryService = categoryService;
            _storeDetailService = storeDetailService;
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
                    
                    var price = await _productVariantService.FindAsync(s => s.ProductID == item.ID);
                    var storeName = await _storeDetailService.FindAsync(x => x.ID == item.StoreID);
                    var categoryName = await _categoryService.FindAsync(c => c.ID == item.CateID);
                    // Tạo danh sách hình ảnh riêng cho từng sản phẩm
                    var imgList = await _productImageService.ListAsync(i => i.ProductID == item.ID);
                    var Listimg = imgList.Select(i => i.ImageUrl).ToList(); // Lấy danh sách ImageUrl

                    list.Add(new ProductsViewModel
                    {
                        CategoryName = categoryName.Name,
                        StoreName = storeName.Name,
                        Price = price.Price,
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
