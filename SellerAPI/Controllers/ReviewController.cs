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
                    Cmt = review.Comment,
                    Relay = review.Reply,
                    Datecmt = review.CommentDate,
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
                Cmt = reviewData.Comment,
                Datecmt = reviewData.CommentDate,
                Relay = reviewData.Reply,
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
                review.Reply = obj.Relay;
                review.ReplyDate = DateTime.Now;

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



                review.Status = false;// từ ẩn -> hiện

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



                review.Status = true; // từ hiện -> ẩn

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


        //check trùng bằng existingReview
        //[HttpPost("CreateReview")]
        //public async Task<IActionResult> CreateReview([FromBody] ReivewViewModel model)
        //{
        //    // Kiểm tra dữ liệu đầu vào
        //    if (model == null || string.IsNullOrWhiteSpace(model.Cmt))
        //    {
        //        return BadRequest(new { message = "Nội dung bình luận không được để trống!" });
        //    }
        //    // Kiểm tra xem sản phẩm có tồn tại không
        //    var product = await _productService.GetAsyncById(model.ProductID);
        //    if (product == null)
        //    {
        //        return NotFound(new { message = "Sản phẩm không tồn tại!" });
        //    }
        //    // Kiểm tra xem User đã đánh giá sản phẩm này chưa
        //    var existingReview = await _reviewService.FindAsync(r => r.UserID == model.UserID && r.ProductID == model.ProductID);
        //    if (existingReview != null)
        //    {
        //        return BadRequest(new { message = "Người dùng đã đánh giá sản phẩm này rồi!" });
        //    }
        //    // Tạo review mới
        //    var newReview = new Review
        //    {
        //        ID = Guid.NewGuid(),
        //        Cmt = model.Cmt,
        //        Datecmt = DateTime.UtcNow,
        //        Relay = model.Relay,
        //        DateRelay = model.DateRelay ?? DateTime.UtcNow,
        //        Status = model.Status,
        //        Rating = model.Rating,
        //        UserID = model.UserID,
        //        ProductID = model.ProductID
        //    };
        //    // Lưu vào database
        //    await _reviewService.AddAsync(newReview);
        //    return Ok(new { message = "Thêm đánh giá thành công!", review = newReview });
        //}

        //add khong can check
        [HttpPost("CreateReview")]
        public async Task<IActionResult> CreateReview([FromBody] ReivewViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Cmt))
            {
                return BadRequest(new { message = "Nội dung bình luận không được để trống!" });
            }
            // Kiểm tra Rating không được để trống và phải nằm trong khoảng hợp lệ
            if (model.Rating <= 0 || model.Rating > 5)
            {
                return BadRequest(new { message = "Điểm đánh giá không hợp lệ! Giá trị phải từ 1 đến 5." });
            }
            var product = await _productService.GetAsyncById(model.ProductID);
            if (product == null)
            {
                return NotFound(new { message = "Sản phẩm không tồn tại!" });
            }

            var newReview = new Review
            {
                ID = Guid.NewGuid(),
                Comment = model.Cmt,
                CommentDate = DateTime.UtcNow,
                //Relay = model.Relay,
                //DateRelay = model.DateRelay ?? DateTime.UtcNow,
                Status = model.Status = false, //hiện
                Rating = model.Rating,
                UserID = model.UserID,
                ProductID = model.ProductID
            };

            try
            {
                await _reviewService.AddAsync(newReview);
                await _reviewService.SaveChangesAsync();
                return Ok(new { message = "Thêm đánh giá thành công!", review = newReview });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Thêm đánh giá thất bại!", error = ex.Message });
            }
        }

        [HttpGet("GetReviewByUserId/{userId}")]
        public async Task<IActionResult> GetReviewByUserId(string userId)
        {
            var list = new List<ReivewViewModel>();

            // Kiểm tra userId hợp lệ
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new ErroMess { msg = "Vui lòng nhập userID" });
            }

            try
            {
                // Kiểm tra user có tồn tại không
                var getUser = await _userManager.FindByIdAsync(userId);
                if (getUser == null)
                {
                    return BadRequest(new ErroMess { msg = "Người dùng không tồn tại trong hệ thống" });
                }

                // Lấy danh sách review theo UserID, sắp xếp giảm dần theo Datecmt
                var reviews = await _reviewService.ListAsync(
                    r => r.UserID == userId,
                    orderBy: q => q.OrderByDescending(r => r.CommentDate) // Ưu tiên đánh giá mới nhất
                );

                if (!reviews.Any())
                {
                    return NotFound(new ErroMess { msg = "Không có đánh giá nào từ người dùng này!" });
                }

                // Lấy danh sách productId từ review
                var productIds = reviews.Select(r => r.ProductID).Distinct().ToList();
                var products = await _productService.ListAsync(p => productIds.Contains(p.ID));

                // Lấy danh sách storeId từ product
                var storeIds = products.Select(p => p.StoreID).Distinct().ToList();
                var stores = await _storeDetailService.ListAsync(s => storeIds.Contains(s.ID));

                // Tạo danh sách kết quả
                int count = 0;
                foreach (var review in reviews)
                {
                    count++;
                    var product = products.FirstOrDefault(p => p.ID == review.ProductID);
                    var store = stores.FirstOrDefault(s => s.ID == product?.StoreID);

                    list.Add(new ReivewViewModel
                    {
                        //No = count, // Đánh số thứ tự như GetWallet
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
                    });
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { msg = ex.Message });
            }
        }

    }
}
