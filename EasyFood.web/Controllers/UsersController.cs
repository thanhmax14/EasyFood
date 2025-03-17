using BusinessLogic.Services.BalanceChanges;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace EasyFood.web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private HttpClient client = null;
        private string _url;
        private readonly IBalanceChangeService _balance;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(UserManager<AppUser> userManager, IBalanceChangeService balance, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
            _balance = balance;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            // Kiểm tra người dùng có đăng nhập hay không
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Lấy ID của user đăng nhập
            string userId = user.Id;
            var list = new IndexUserViewModels();

            // Gọi API Gateway để lấy thông tin user theo ID
            string apiUrl = $"https://localhost:5555/Gateway/UsersService/View-Profile/{userId}";
            var urlBalace = $"https://localhost:5555/Gateway/WalletService/GetWallet";
            try
            {
                var response = await client.GetAsync(apiUrl);
                var responceBalance = await this.client.GetAsync($"{urlBalace}/{user.Id}");
                if (!response.IsSuccessStatusCode || !responceBalance.IsSuccessStatusCode)
                {
                    return View(list);
                }
                var mes = await response.Content.ReadAsStringAsync();
                list.userView = JsonSerializer.Deserialize<UsersViewModel>(mes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var messBalance = await responceBalance.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var dataRepone = JsonSerializer.Deserialize<List<BalanceListViewModels>>(messBalance, options);
                list.Balance = dataRepone;


                return View(list); 
            }
            catch (Exception)
            {
                return View(list);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] UsersViewModel obj)
        {
            if (obj == null)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });
            }

            string apiUrl = $"https://localhost:5555/Gateway/UsersService/{user.Id}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật thất bại!" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Lỗi kết nối API Gateway!" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RegisterSeller(UsersViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Ban chua dang nhap" });
            }
            string apiUrl = $"https://localhost:5555/Gateway/UsersService/register-seller/{user.Id}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành công!" });

                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật thất bại!" });

                }
            }
            catch (Exception)
            {

                return Json(new { success = false, message = "Lỗi kết nối API Gateway!" }); ;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBalance(long number)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            if (number < 100000)
            {
                return Json(new ErroMess { msg = "Nạp tối thiểu 100,000 VND" });
            }
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            this._url = "https://localhost:5555/Gateway/WalletService/CreatePayment";
            var temdata = new DepositViewModel
            {
                number = number,
                CalleURL = $"{baseUrl}/home/invoice",
                ReturnUrl = $"{baseUrl}/home/invoice",
                UserID = user.Id
            };

            string json = JsonSerializer.Serialize(temdata);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
           
            try
            {
                var response = await client.PostAsync($"{this._url}", content);
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
            catch (Exception ex)
            {
                return Json(new { success=false, msg = "Lỗi không xác định, vui lòng thử lại." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBalance()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("Phai Dang Nhap");
            }
            var urlBalace = $"https://localhost:5555/Gateway/WalletService/GetWallet";
            try
            {
       
                var responceBalance = await this.client.GetAsync($"{urlBalace}/{user.Id}");
                if  (!responceBalance.IsSuccessStatusCode)
                {
                    return BadRequest("Lỗi Hệ Thống");
                }
                var messBalance = await responceBalance.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var dataRepone = JsonSerializer.Deserialize<List<BalanceListViewModels>>(messBalance, options);
           
                return Json(dataRepone);
            }
            catch (Exception)
            {
                return BadRequest("Lỗi Hệ Thống");
            }
        }


    }
}
