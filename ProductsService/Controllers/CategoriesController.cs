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

        [HttpGet("GetAllProductOfCategory")]

        public async Task<IActionResult> GetAllProductOfCategory(Guid categoryId)
        {
            var store = new List<StoreDetailsViewModels>();
            var List = new List<ProductsViewModel>();
            var categoryList = new CategoryViewModel();


            var products = await _productService.ListAsync(u => u.IsActive && u.CateID == categoryId, orderBy: x => x.OrderByDescending(s => s.CreatedDate));

            foreach (var item in products)
            {
                var price = await _productVariantService.FindAsync(s => s.ProductID == item.ID && s.IsActive == true);
                var storeName = await _storeDetailService.FindAsync(x => x.ID == item.StoreID);
                var categoryName = await _categoryService.FindAsync(c => c.ID == item.CateID);
                var imgList = await _productImageService.ListAsync(i => i.ProductID == item.ID);

                var Listimg = imgList.Select(i => i.ImageUrl).ToList();

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
                        Img = Listimg

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
                        Img = Listimg // Gán danh sách URL hình ảnh cho sản phẩm

                    });
                }

            }
            var category = await _categoryService.FindAsync(s => s.ID == categoryId);
           

            categoryList.ID = category.ID;
            categoryList.Name = category.Name;
            categoryList.Img = category.Img;
            categoryList.Number = category.Number;
            categoryList.Commission = category.Commission;
            categoryList.CreatedDate = category.CreatedDate;
            categoryList.ModifiedDate = category.ModifiedDate;
            categoryList.ProductViewModel = List;
            categoryList.StoreDetailViewModel = store;

            if (category == null)
            {
                return BadRequest("Category not found");
            }
            else
            {
                return Ok(categoryList);
            }



        }

    }


}
