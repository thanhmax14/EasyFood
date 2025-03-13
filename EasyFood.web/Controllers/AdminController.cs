using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace EasyFood.web.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private HttpClient client = null;
        private string url;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
        }

        public async Task<IActionResult> Index()
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            string apiURL = "https://localhost:5555/Gateway/ManagementSellerService";
            List<UsersViewModel> userViewModel = new List<UsersViewModel>(); // ✅ Danh sách người dùng

            try
            {
                var response = await client.GetAsync(apiURL);
                if (!response.IsSuccessStatusCode)
                {
                    return View(userViewModel); // Nếu lỗi, trả về danh sách rỗng
                }

                var mes = await response.Content.ReadAsStringAsync();
                userViewModel = JsonSerializer.Deserialize<List<UsersViewModel>>(mes, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(userViewModel); // ✅ Trả về danh sách UsersViewModel dưới dạng JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }
        public async Task<IActionResult> Hidden([FromBody] UsersViewModel obj)
        {
            var admin = await _userManager.GetUserAsync(User);
            if(admin == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if(!await _userManager.IsInRoleAsync(admin,"Admin")) {
                return RedirectToAction("Login", "Home");
            }
         
            string apiURL = $"https://localhost:5555/Gateway/ManagementSellerService/Admin-Hiden/{obj.Email}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var respone = await client.PostAsync(apiURL, content);  
                if(respone.IsSuccessStatusCode)
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
        public async Task<IActionResult> Show([FromBody] UsersViewModel obj)
        {
            var admin = await _userManager.GetUserAsync(User);
            if(admin == null)
            {
                return RedirectToAction("Login", "Home");
            } 
            if(!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            string apiURL = $"https://localhost:5555/Gateway/ManagementSellerService/Admin-Show/{obj.Email}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var respone = await client.PostAsync(apiURL,content);
                if(respone.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành Công" });
                } else
                {
                    return Json(new { success = false, message = "Cập nhật thất bại" });
                }

            }
            catch (Exception)
            {

                return Json(new { success = false, message = "Lỗi kết nối API Gateway!" }); ;
            }
        }
        public async Task<IActionResult> UpdateByAdmin([FromBody] UsersViewModel obj)
        {
            var admin = await _userManager.GetUserAsync (User);
            if(admin == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if(!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            string apiURL = $"https://localhost:5555/Gateway/ManagementSellerService/Admin-Update/{obj.Email}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiURL,content);  
                if(response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành công" });
                } else
                {
                    return Json(new { success = true, message = "Cập nhật thất bại" });
                }
            }
            catch (Exception)
            {

                return Json(new { success = false, message = "Lỗi kết nối API Gateway!" }); ;
            }
        }
        }

    }
}
