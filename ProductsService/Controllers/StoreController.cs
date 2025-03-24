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
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;

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



        [HttpGet("SearchStoreByName")]
        public async Task<IActionResult> SearchStoreByName(string searchName)
        {
            if (string.IsNullOrWhiteSpace(searchName))
                return BadRequest("Store name is required");


            

            var list = new List<StoreViewModel>();
            var store = await _storeDetailService.ListAsync(
                s => s.IsActive && s.Name.ToLower().Contains(searchName),
                orderBy: x => x.OrderByDescending(s => s.CreatedDate));
            if (!store.Any())
            {
                return NotFound("Store Not found");
            }
               
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


        [HttpGet("GetStoreDetail")]

        public async Task<IActionResult> GetStoreDetail(Guid id)
        {
            var store = new StoreDetailsViewModels();
            var List = new List<ProductsViewModel>();
            var categoryList = new List<CategoryViewModel>();

            var products = await _productService.ListAsync(u => u.IsActive && u.StoreID == id , orderBy: x => x.OrderByDescending(s => s.CreatedDate));

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
                        Img = Listimg // Gán danh sách URL hình ảnh cho sản phẩm

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



            var storeDetails = await _storeDetailService.FindAsync(s => s.ID == id);
            var userid = await _userManager.FindByIdAsync(storeDetails.UserID);
            var product = await _productService.FindAsync(p => p.StoreID == id);
            var ProductPrice = await _productVariantService.FindAsync(s => s.ProductID == product.ID);
            var category = await _categoryService.FindAsync(c => c.ID == product.CateID);
            

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

            store.ProductViewModel = List;
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
