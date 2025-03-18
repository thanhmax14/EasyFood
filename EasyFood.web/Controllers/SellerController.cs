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


        public async Task<IActionResult> ReplyFeedback(string id)
        {
            try
            {
                // Kiểm tra id có hợp lệ không
                if (!Guid.TryParse(id, out Guid reviewId))
                {
                    return Json(new { success = false, message = "ID không hợp lệ." });
                }

                // Tìm review theo ReviewId
                var review = await _reviewService.FindAsync(r => r.ID == reviewId);

                if (review == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đánh giá." });
                }

                // Lấy thông tin người dùng
                var user = await _userManager.FindByIdAsync(review.UserID);
                if (user == null)
                {
                    return Json(new { success = false, message = "Người dùng không tồn tại." });
                }

                // Lấy thông tin sản phẩm
                var product = await _productService.GetAsyncById(review.ProductID);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                // Tạo ViewModel để hiển thị trong View
                var reviewModel = new ReivewViewModel
                {
                    ID = review.ID,
                    Username = user.UserName,
                    ProductName = product.Name,
                    Rating = review.Rating,
                    Cmt = review.Cmt,
                    Datecmt = review.Datecmt,
                    Relay = review.Relay,
                    Status = review.Status,
                    UserID = review.UserID,
                    ProductID = review.ProductID
                };

                return View(reviewModel);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để debug sau này
                Console.WriteLine($"Lỗi: {ex.Message}");

                // Trả về lỗi JSON để tránh chết chương trình
                return Json(new { success = false, message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ReplyFeedback(ReivewViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Relay))
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            try
            {

                string apiUrl = $"https://localhost:5555/Gateway/ReviewService/UpdateReply/{model.ID}";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");


                var response = await client.PutAsync(apiUrl, content);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var mes = await response.Content.ReadAsStringAsync();
                var dataRepone = JsonSerializer.Deserialize<ErroMess>(mes, options);
                if (response.IsSuccessStatusCode)
                {
                    return Redirect("/Seller/FeedbackList");
                }
                return Json(dataRepone);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi kết nối API Gateway! " + ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Show(string id)
        {
            var model = new ReivewViewModel
            {
                Status = true,
                UserID = id,
                ID = Guid.Parse(id),
                Cmt = "1",
                Datecmt = DateTime.Now,
                Relay = "1",
                DateRelay = DateTime.Now,
                Rating = 5,

            };


            string apiUrl = $"https://localhost:5555/Gateway/ReviewService/ShowFeedback/{id}";
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");


                var response = await client.PutAsync(apiUrl, content);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var mes = await response.Content.ReadAsStringAsync();
                var dataRepone = JsonSerializer.Deserialize<ErroMess>(mes, options);
                if (response.IsSuccessStatusCode)
                {
                    return Json(dataRepone);
                }
                return Json(dataRepone);
            }
            catch (Exception)
            {

                return StatusCode(500, new ErroMess { success = false, msg = "Lỗi kết nối API Gateway!" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> HiddenFeedback([FromBody] ReivewViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Relay))
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }


            string apiUrl = $"https://localhost:5555/Gateway/ReviewService/HiddenFeedback/{model.ID}";
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");


                var response = await client.PutAsync(apiUrl, content);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var mes = await response.Content.ReadAsStringAsync();
                var dataRepone = JsonSerializer.Deserialize<ErroMess>(mes, options);
                if (response.IsSuccessStatusCode)
                {
                    return Json(dataRepone);
                }
                return Json(dataRepone);
            }
            catch (Exception)
            {

                return StatusCode(500, new ErroMess { success = false, msg = "Lỗi kết nối API Gateway!" });
            }
        }
    }
}
