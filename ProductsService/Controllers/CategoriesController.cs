using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.ViewModels;

namespace ProductsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;
        private readonly IProductVariantService _productVariantService;
        private readonly ICategoryService _categoryService;
        private readonly IStoreDetailService _storeDetailService;

        public CategoriesController(IProductService productService, IProductImageService productImageService, IProductVariantService productVariantService, ICategoryService categoryService, IStoreDetailService storeDetailService)
        {
            _productService = productService;
            _productImageService = productImageService;
            _productVariantService = productVariantService;
            _categoryService = categoryService;
            _storeDetailService = storeDetailService;
        }


        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAllCategory()
        {
            var list = new List<CategoryViewModel>();
            var category = await _categoryService.ListAsync(orderBy: c => c.OrderByDescending(c => c.CreatedDate));
          
            if (category.Any())
            {
                foreach (var item in category)
                {
                    list.Add(new CategoryViewModel
                    {
                        ID = item.ID,
                        Name = item.Name,
                        CreatedDate = item.CreatedDate,
                        Commission = item.Commission,
                        Img = item.Img,
                        ModifiedDate = item.ModifiedDate,
                        Number = item.Number,

                    });
                   
                }
                return Ok(list);

            }
            return BadRequest(false);
        }
        [HttpGet("SearchCategoryByName")]
        public async Task<IActionResult> SearchCategoryByName(string searchName)
        {
            if (string.IsNullOrWhiteSpace(searchName))
                return BadRequest("Product name is required");

            var list = new List<CategoryViewModel>();
            var category = await _categoryService.ListAsync(
                u => u.Name.ToLower().Contains(searchName),
                orderBy: x => x.OrderByDescending(s => s.CreatedDate)
            );

            if (!category.Any())
                return NotFound("No Category found");

            if (category.Any())
            {
                foreach (var item in category)
                {
                    list.Add(new CategoryViewModel
                    {
                        ID = item.ID,
                        Name = item.Name,
                        CreatedDate = item.CreatedDate,
                        Commission = item.Commission,
                        Img = item.Img,
                        ModifiedDate = item.ModifiedDate,
                        Number = item.Number,

                    });

                }
                return Ok(list);

            }
            return BadRequest(false);
        }

        [HttpGet("GetAllProductOfCategory")]

        public async Task<IActionResult> GetAllProductOfCategory(Guid id)
        {
            var CategoryList = new CategoryDetailsViewModel();
            var List = new List<ProductsViewModel>();
            var StoreList = new List<StoreDetailsViewModels>();
            var products = await _productService.ListAsync(u => u.IsActive && u.CateID == id, orderBy: x => x.OrderByDescending(s => s.CreatedDate));

            foreach (var item in products)
            {
                var price = await _productVariantService.FindAsync(s => s.ProductID == item.ID && s.IsActive == true);
                var storeName = await _storeDetailService.FindAsync(x => x.ID == item.StoreID);
                var categoryName = await _categoryService.FindAsync(c => c.ID == item.CateID);
                var imgList = await _productImageService.ListAsync(i => i.ProductID == item.ID);
                var ListImg = imgList.Select(o => o.ImageUrl).ToList();

                if (price != null)
                {
                    List.Add(new ProductsViewModel
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
                        Img = ListImg
                    });

                }
                else
                {
                    List.Add(new ProductsViewModel
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
                        Img = ListImg // Gán danh sách URL hình ảnh cho sản phẩm

                    });
                }
        }
            
            var category = await _categoryService.FindAsync(c => c.ID == id);

            CategoryList.ID = category.ID;
            CategoryList.Number = category.Number;
            CategoryList.Commission = category.Commission;
            CategoryList.CreatedDate = category.CreatedDate;
            CategoryList.ModifiedDate = category.CreatedDate;
            CategoryList.Img = category.Img;
            CategoryList.ProductViewModel = List;
            CategoryList.StoreDetailViewModel = StoreList;

            if (category == null)
            {
                return BadRequest("Category Not Found");
            }
            else
            {
                return Ok(category);
            }


        }

    }


}
