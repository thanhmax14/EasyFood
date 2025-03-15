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
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;
        private readonly IStoreDetailService _storeDetailService;

        public ReviewController(IReviewService reviewService, UserManager<AppUser> userManager, IProductService productService, IStoreDetailService storeDetailService)
        {
            _reviewService = reviewService;
            _userManager = userManager;
            _productService = productService;
            _storeDetailService = storeDetailService;
        }

        /// <summary>
        /// Lấy feedback theo StoreID của role seller
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [HttpGet("ViewFeedbackList/{storeId}")]
        public async Task<IActionResult> GetReviewByStoreId(Guid storeId)
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

        /// <summary>
        /// Lấy feedback theo UserID của role user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReviewByUserId(string userId)
        {
            // Lấy danh sách review theo UserID
            var reviews = await _reviewService.ListAsync(r => r.UserID == userId);

            if (reviews == null || !reviews.Any())
            {
                return NotFound("Không có đánh giá nào của người dùng này.");
            }

            var productIds = reviews.Select(r => r.ProductID).ToList();

            // Lấy danh sách sản phẩm tương ứng với ProductID trong review
            var products = await _productService.ListAsync(p => productIds.Contains(p.ID));

            var result = new List<ReivewViewModel>();

            foreach (var review in reviews)
            {
                var product = products.FirstOrDefault(p => p.ID == review.ProductID);

                var reviewModel = new ReivewViewModel
                {
                    Cmt = review.Cmt,
                    Relay = review.Relay,
                    Datecmt = review.Datecmt,
                    Status = review.Status,
                    Rating = review.Rating,
                    Username = userId.ToString(), // Nếu cần lấy UserName, bạn có thể gọi lại _userManager
                    ProductName = product?.Name ?? "Không xác định",
                    StoreId = product?.StoreID ?? Guid.Empty // Lấy StoreID từ Product
                };

                result.Add(reviewModel);
            }

            return Ok(result);
        }

    }
}
