using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace EasyFood.web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private HttpClient client = null;
        private string _url;

        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
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
            var userViewModel = new UsersViewModel();

            // Gọi API Gateway để lấy thông tin user theo ID
            string apiUrl = $"https://localhost:5555/Gateway/UsersService/View-Profile/{userId}";

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return View(userViewModel);
                }

                var mes = await response.Content.ReadAsStringAsync();
                userViewModel = JsonSerializer.Deserialize<UsersViewModel>(mes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(userViewModel); // Trả về thông tin userViewModel cho View
            }
            catch (Exception)
            {
                return View(userViewModel);
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


    }
}
