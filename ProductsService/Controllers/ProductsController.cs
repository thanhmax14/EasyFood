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
        private readonly IProductVariantService _productVariantService;
        private readonly ICategoryService _categoryService;
        private readonly IStoreDetailService _storeDetailService;

        public ProductsController(IProductService productService, IProductImageService productImageService, IProductVariantService productVariantService, ICategoryService categoryService, IStoreDetailService storeDetailService)
        {
            _productService = productService;
            _productImageService = productImageService;
            _productVariantService = productVariantService;
            _categoryService = categoryService;
            _storeDetailService = storeDetailService;
        }

        [HttpGet("GetAllProducts")]

        public async Task<IActionResult> GetAll()
        {
            var list = new List<ProductsViewModel>();
            var products = await _productService.ListAsync(u => u.IsActive, orderBy: x => x.OrderByDescending(s => s.CreatedDate));

            if (products.Any())
            {
                foreach (var item in products)
                {

                    var price = await _productVariantService.FindAsync(s => s.ProductID == item.ID && s.IsActive == true);

                    if (price != null)
                    {
                        var storeName = await _storeDetailService.FindAsync(x => x.ID == item.StoreID);
                        var categoryName = await _categoryService.FindAsync(c => c.ID == item.CateID);
                        // Tạo danh sách hình ảnh riêng cho từng sản phẩm
                        var imgList = await _productImageService.ListAsync(i => i.ProductID == item.ID);

                        var Listimg = imgList.Select(i => i.ImageUrl).ToList();
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
                            StoreId = item.StoreID,
                            Img = Listimg // Gán danh sách URL hình ảnh cho sản phẩm

                        });
                    }
                    else
                    {
                        var storeName = await _storeDetailService.FindAsync(x => x.ID == item.StoreID);
                        var categoryName = await _categoryService.FindAsync(c => c.ID == item.CateID);
                        // Tạo danh sách hình ảnh riêng cho từng sản phẩm
                        var imgList = await _productImageService.ListAsync(i => i.ProductID == item.ID);

                        var Listimg = imgList.Select(i => i.ImageUrl).ToList();
                        list.Add(new ProductsViewModel
                        {
                            CategoryName = categoryName.Name,
                            StoreName = storeName.Name,
                            Price = 0,
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
                            StoreId = item.StoreID,
                            Img = Listimg // Gán danh sách URL hình ảnh cho sản phẩm

                        });
                    }


                }
                return Ok(list);
            }
            return BadRequest(false);
        }
        [HttpGet("GetProductDetails")]
        public async Task<IActionResult> ProductDetails(Guid id)
        {
            
            var producct = new ProductDetailsViewModel();
            var romType = await this._productImageService.ListAsync(u => u.ProductID == id);
            if (romType.Any())
            {
                foreach (var item in romType)
                {
                    producct.Img.Add(item.ImageUrl);
                }
                
            }
            
            var imgList = await _productImageService.ListAsync(i => i.ProductID == producct.ID);
            var Listimg = imgList.Select(i => i.ImageUrl).ToList(); // Lấy danh sách ImageUrl
            var productDetail = await _productService.FindAsync(x => x.ID == id);



            var price = await _productVariantService.FindAsync(s => s.ProductID == id && s.IsActive == true);

            if (price != null)
            {

                var ProductSize = await _productVariantService.FindAsync(s => s.ProductID == id);

                var getFullsize = await this._productVariantService.ListAsync(s => s.ProductID == id && s.IsActive == true);

                foreach(var item in getFullsize)
                {
                    producct.size.Add(item.Size);

                }


                var storeName = await _storeDetailService.FindAsync(t => t.ID == productDetail.StoreID);
                var categoryName = await _categoryService.FindAsync(c => c.ID == productDetail.CateID);

                producct.ID = productDetail.ID;
                producct.Name = productDetail.Name;
                producct.StoreName = storeName.Name;
                producct.CategoryName = categoryName.Name;
                producct.Price = price.Price;
                producct.ShortDescription = productDetail.ShortDescription;
                producct.LongDescription = productDetail.LongDescription;
                producct.CreatedDate = productDetail.CreatedDate;
                producct.Stocks = ProductSize.Stock;
              
            }
            else
            {
                var storeName = await _storeDetailService.FindAsync(t => t.ID == productDetail.StoreID);
                var categoryName = await _categoryService.FindAsync(c => c.ID == productDetail.CateID);

                producct.ID = productDetail.ID;
                producct.Name = productDetail.Name;
                producct.StoreName = storeName.Name;
                producct.CategoryName = categoryName.Name;
                producct.Price = 0;
                producct.ShortDescription = productDetail.ShortDescription;
                producct.LongDescription = productDetail.LongDescription;
                producct.CreatedDate = productDetail.CreatedDate;
               
            }



            if (productDetail == null)
            {
                return BadRequest("Not found Product");

            }
            else
            {
                return Ok(producct);
            }
        }
       

    }
}
