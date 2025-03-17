using System.Net.Http.Headers;
using System.Text.Json;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;

namespace EasyFood.web.Controllers
{
    public class SellerController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;
        private readonly IStoreDetailService _storeDetailService;
        private HttpClient client = null;
        private string _url;
        public SellerController(IReviewService reviewService, UserManager<AppUser> userManager, IProductService productService, IStoreDetailService storeDetailService)
        {
            _reviewService = reviewService;
            _userManager = userManager;
            _productService = productService;
            _storeDetailService = storeDetailService;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
        }



        public async Task<IActionResult> FeedbackList()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var storeId = await _storeDetailService.FindAsync(s => s.UserID == user.Id);
            var id = storeId.ID;

            string apiUrl = $"https://localhost:5555/Gateway/ReviewService/ViewFeedbackList/{id}";
            List<ReivewViewModel> list = new List<ReivewViewModel>();

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return View(list);
                }
                var mes = await response.Content.ReadAsStringAsync();
                list = JsonSerializer.Deserialize<List<ReivewViewModel>>(mes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(list); // Trả về danh sách thay vì một object đơn

            }
            catch (Exception)
            {
                return View(list);
            }

        }

        //load du lieu len reply page
        [HttpGet]
        public async Task<IActionResult> ReplyFeedback(Guid reviewId)
        {
            // Tìm review theo ReviewId
            var review = await _reviewService.ListAsync(r => r.ID == reviewId);
            var reviewData = review.FirstOrDefault();

            if (reviewData == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đánh giá." });
            }

            // Lấy thông tin người dùng và sản phẩm liên quan
            var user = await _userManager.FindByIdAsync(reviewData.UserID);
            var product = await _productService.GetAsyncById(reviewData.ProductID);

            // Tạo ViewModel để hiển thị trong View
            var reviewModel = new ReivewViewModel
            {
                ID = reviewData.ID,
                Username = user?.UserName,
                ProductName = product?.Name,
                Rating = reviewData.Rating,
                Cmt = reviewData.Cmt,
                Datecmt = reviewData.Datecmt,
                Relay = reviewData.Relay,
                Status = reviewData.Status
            };

            return View(reviewModel);
        }

        //[HttpPost]
        //public async Task<IActionResult> UpdateByAdmin([FromBody] AdminViewModel obj1, string id)
        //{
        //    if (obj1 == null || string.IsNullOrEmpty(obj1.Email))
        //    {
        //        return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        //    }

        //    var admin = await _userManager.GetUserAsync(User);
        //    if (admin == null)
        //    {
        //        return Json(new { success = false, message = "Bạn chưa đăng nhập!" });
        //    }
        //    if (!await _userManager.IsInRoleAsync(admin, "Admin"))
        //    {
        //        return Json(new { success = false, message = "Bạn không có quyền cập nhật!" });
        //    }

        //    string apiURL = $"https://localhost:5555/Gateway/ManagementSellerService/Admin-Update/{obj1.Email}";

        //    try
        //    {
        //        var jsonContent = JsonSerializer.Serialize(obj1);
        //        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        //        var response = await client.PutAsync(apiURL, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return Json(new { success = true, message = "Cập nhật thành công!" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Cập nhật thất bại!" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Lỗi kết nối API Gateway! " + ex.Message });
        //    }
        //}

    }
}
