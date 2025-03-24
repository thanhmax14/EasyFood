using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;

using BusinessLogic.Services.Reviews;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repository.ViewModels;
using Models;

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
        private readonly IReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;


       

        public ProductsController(IProductService productService, IProductImageService productImageService, IProductVariantService productVariantService, ICategoryService categoryService, IStoreDetailService storeDetailService, IReviewService reviewService, UserManager<AppUser> userManager)
        {
            _productService = productService;
            _productImageService = productImageService;
            _productVariantService = productVariantService;
            _categoryService = categoryService;
            _storeDetailService = storeDetailService;
            _reviewService = reviewService;
            _userManager = userManager;
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

        [HttpGet("GetAllComment/{id}")]
        public async Task<IActionResult> getAllComment(Guid id)
        {
            // Tìm sản phẩm có ID được truyền vào
            var product = (await _productService.ListAsync()).FirstOrDefault(p => p.ID == id);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            // Lấy danh sách cửa hàng theo StoreID của sản phẩm
            var storeDetails = await _storeDetailService.ListAsync();
            var store = storeDetails.FirstOrDefault(x => x.ID == product.StoreID);
            var storeName = store?.Name ?? "Unknown Store"; // Nếu không tìm thấy cửa hàng, đặt là "Unknown Store"

            // Lấy danh sách review của sản phẩm
            var reviews = await _reviewService.ListAsync(x => x.ProductID == id);

            // Danh sách bình luận (mặc định rỗng)
            var commentList = new List<CommentViewModels>();

            if (reviews.Any())
            {
                // Lấy danh sách User từ review
                var userIds = reviews.Select(x => x.UserID).Distinct().ToList();
                var users = _userManager.Users.Where(x => userIds.Contains(x.Id)).ToList();
                var userDict = users.ToDictionary(u => u.Id, u => u.UserName);

                // Tạo danh sách CommentViewModels
                commentList = reviews.Select(review => new CommentViewModels
                {
                    storeName = storeName,
                    Username = userDict.ContainsKey(review.UserID) ? userDict[review.UserID] : "Unknown",
                    Cmt = review.Cmt,
                    Datecmt = review.Datecmt,
                    Relay = review.Relay,
                    DateRelay = review.DateRelay,
                    Status = review.Status,
                    Rating = review.Rating
                }).ToList();
            }

            return Ok(commentList);
        }




    }
}
