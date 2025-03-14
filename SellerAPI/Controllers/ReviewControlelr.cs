using BusinessLogic.Services.Products;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;

namespace SellerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewControlelr : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;
        private readonly IStoreDetailService _storeDetailService;

        public ReviewControlelr(IReviewService reviewService, UserManager<AppUser> userManager, IProductService productService, IStoreDetailService storeDetailService)
        {
            _reviewService = reviewService;
            _userManager = userManager;
            _productService = productService;
            _storeDetailService = storeDetailService;
        }

        [HttpGet("{storeId}")]
        public async Task<IActionResult> GetByStoreId(Guid storeId)
        {
            // Lấy danh sách sản phẩm thuộc StoreId
            var products = await _productService.ListAsync(p => p.StoreID == storeId);
            var productIds = products.Select(p => p.ID).ToList();

            // Lấy danh sách review dựa vào ProductId
            var reviews = await _reviewService.ListAsync(r => productIds.Contains(r.ProductID));

            var result = new List<ReivewViewModel>();

            foreach (var review in reviews)
            {
                var user = await _userManager.FindByIdAsync(review.UserID);
                var product = products.FirstOrDefault(p => p.ID == review.ProductID);

                var reviewModel = new ReivewViewModel
                {
                    Cmt = review.Cmt,
                    Relay = review.Relay,
                    Datecmt = review.Datecmt,
                    Status = review.Status,
                    Rating = review.Rating,
                    Username = user?.UserName,
                    ProductName = product?.Name,
                    StoreId = storeId
                };

                result.Add(reviewModel);
            }

            return Ok(result);
        }

    }
}
