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
                    ID = review.ID,
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

        [HttpGet("ViewFeedbackById/{reviewId}")]
        public async Task<IActionResult> GetReviewByReviewId(Guid reviewId)
        {
            // Tìm review theo ReviewId
            var review = await _reviewService.ListAsync(r => r.ID == reviewId);
            var reviewData = review.FirstOrDefault(); // Lấy 1 review

            if (reviewData == null)
            {
                return NotFound(new { message = "Không tìm thấy đánh giá." });
            }

            // Lấy thông tin người dùng và sản phẩm liên quan
            var user = await _userManager.FindByIdAsync(reviewData.UserID);
            var product = await _productService.GetAsyncById(reviewData.ProductID);

            // Tạo ViewModel trả về
            var reviewModel = new ReivewViewModel
            {
                ID = reviewData.ID,
                Username = user?.UserName,
                ProductName = product?.Name,
                Rating = reviewData.Rating,
                Cmt = reviewData.Cmt,
                Datecmt = reviewData.Datecmt,
                Relay = reviewData.Relay,
                Status = reviewData.Status,
            };

            return Ok(reviewModel);
        }


        [HttpPut("UpdateReply/{id}")]
        public async Task<IActionResult> UpdateReply(Guid id, [FromBody] ReivewViewModel obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.Relay))
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            try
            {
                var review = await _reviewService.GetAsyncById(id);

                if (review == null)
                {
                    return NotFound(new ErroMess { success = false, msg = "khong tim thay danh gia" });
                }

                // Cập nhật phản hồi
                review.Relay = obj.Relay;
                review.DateRelay = DateTime.Now;

                // Lưu thay đổi
                await _reviewService.UpdateAsync(review);
                await _reviewService.SaveChangesAsync();


                return Ok(
                    new ErroMess { success = true, msg = "Cập nhật phản hồi thành công" }
                    );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { msg = ex.Message });
            }


        }



        [HttpPut("ShowFeedback/{id}")]
        public async Task<IActionResult> ShowFeedback(Guid id)
        {
            try
            {
                var review = await _reviewService.GetAsyncById(id);

                if (review == null)
                {
                    return NotFound(new ErroMess { success = false, msg = "khong tim thay danh gia" });
                }


                review.Status = false;
                await _reviewService.UpdateAsync(review);
                await _reviewService.SaveChangesAsync();

                return Ok(
                    new ErroMess { success = true, msg = "Cập nhật phản hồi thành công" }
                    );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { msg = ex.Message });
            }
        }
        [HttpPut("HiddenFeedback/{id}")]
        public async Task<IActionResult> HiddenFeedback(Guid id)
        {

            try
            {
                var review = await _reviewService.GetAsyncById(id);

                if (review == null)
                {
                    return NotFound(new ErroMess { success = false, msg = "khong tim thay danh gia" });
                }


                review.Status = true;
                await _reviewService.UpdateAsync(review);
                await _reviewService.SaveChangesAsync();

                return Ok(
                    new ErroMess { success = true, msg = "Cập nhật phản hồi thành công" }
                    );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { msg = ex.Message });
            }
        }

        [HttpGet("ViewFeedbackListByUser/{userId}")]
        public async Task<IActionResult> GetReviewByUserId(string userId)
        {
            // Lấy danh sách review dựa vào UserID
            var reviews = await _reviewService.ListAsync(r => r.UserID == userId);

            if (!reviews.Any())
            {
                return NotFound(new { message = "Không có đánh giá nào từ người dùng này!" });
            }

            // Lấy danh sách productId từ review
            var productIds = reviews.Select(r => r.ProductID).Distinct().ToList();

            // Lấy thông tin sản phẩm từ danh sách productId
            var products = await _productService.ListAsync(p => productIds.Contains(p.ID));

            // Lấy danh sách storeId từ product
            var storeIds = products.Select(p => p.StoreID).Distinct().ToList();

            // Lấy thông tin store từ danh sách storeId
            var stores = await _storeDetailService.ListAsync(s => storeIds.Contains(s.ID));

            // Xây dựng kết quả
            var result = reviews.Select(review =>
            {
                var product = products.FirstOrDefault(p => p.ID == review.ProductID);
                var store = stores.FirstOrDefault(s => s.ID == product?.StoreID);

                return new ReivewViewModel
                {
                    ID = review.ID,
                    Cmt = review.Cmt,
                    Relay = review.Relay,
                    Datecmt = review.Datecmt,
                    Status = review.Status,
                    Rating = review.Rating,
                    UserID = review.UserID,
                    ProductID = review.ProductID,
                    ProductName = product?.Name, // Lấy tên sản phẩm
                    StoreId = product?.StoreID ?? Guid.Empty, // Lấy StoreID
                    StoreName = store?.Name // Lấy StoreName
                };
            }).ToList();

            return Ok(result);
        }
    }
}
