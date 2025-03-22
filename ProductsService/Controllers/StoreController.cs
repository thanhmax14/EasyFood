using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariantVariants;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;
using System.Net.WebSockets;

namespace ProductsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly StoreDetailService _storeDetailService;
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;
        private readonly ProductVariantService _productVariantService;
        private readonly ProductImageService _productImageService;
        private readonly UserManager<AppUser> _userManager;


        public StoreController(StoreDetailService storeDetailService, CategoryService categoryService, ProductService productService, ProductVariantService productVariantService, ProductImageService productImageService, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _productImageService = productImageService;
           _productService = productService;
            _productVariantService = productVariantService;
            _categoryService = categoryService;
            _storeDetailService = storeDetailService;
        }

        [HttpGet("GetAllStores")]
        public async Task<IActionResult> GetallStore()
        {

            var list = new List<StoreViewModel>();
            var store = await _storeDetailService.ListAsync(s => s.IsActive, orderBy: x => x.OrderByDescending(s => s.CreatedDate));
            if (store.Any())
            {
                foreach (var item in store)
                {
                    list.Add(new StoreViewModel
                    {
                        Name = item.Name,
                        LongDescriptions = item.LongDescriptions,
                        Address = item.Address,
                        Phone = item.Phone,
                        ShortDescriptions = item.ShortDescriptions,
                        CreatedDate = item.CreatedDate,
                        ID = item.ID,
                        Img = item.Img,
                        IsActive = item.IsActive,
                        ModifiedDate = item.ModifiedDate,
                        Status = item.Status,   
                    });
                }
                return Ok(list);
            }
            return BadRequest(false);
        }


        [HttpGet("GetStoreDetail")]

        public async Task<IActionResult> GetStoreDetail(Guid id)
        {
            var store = new StoreDetailsViewModels();
            var productList = new List<ProductListViewModel>();
            var categoryList = new List<CategoryViewModel>();


            var storeDetails = await _storeDetailService.FindAsync(s => s.ID == id);
            var userid = await _userManager.FindByIdAsync(storeDetails.UserID);
            var product = await _productService.FindAsync(p => p.StoreID == id);
            var ProductPrice = await _productVariantService.FindAsync(s => s.ProductID == product.ID);
            var category = await _categoryService.FindAsync(c => c.ID == product.CateID);
            var imgList = await _productImageService.ListAsync(i => i.ProductID == product.ID);
            var Listimg = imgList.Select(i => i.ImageUrl).ToList();

            store.UserID = storeDetails.UserID;
            store.UserName = userid?.UserName;
            store.ID = storeDetails.ID;
            store.Name = storeDetails.Name;
            store.Address = storeDetails.Address;
            store.Phone = storeDetails.Phone;
            store.ShortDescriptions = storeDetails.ShortDescriptions;
            store.CreatedDate = storeDetails.CreatedDate;
            store.Img = storeDetails.Img;
            store.LongDescriptions = storeDetails.LongDescriptions;

            store.ProductViewModel = productList;
            store.CategoryViewModels = categoryList;

            if (storeDetails == null)
            {
                return BadRequest("Store not Found");
            }
            else
            {

                return Ok(store);

            }
        }


    }
}
