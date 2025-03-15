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


    }
}
