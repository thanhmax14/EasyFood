using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;
using System.Net.Http.Headers;
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


    }
}
