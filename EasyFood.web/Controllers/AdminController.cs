using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;
using AutoMapper;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.StoreDetails;
using Repository.ViewModels;

namespace EasyFood.web.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private HttpClient client = null;
        private string url;

        private readonly IStoreDetailService _storeService;
        private readonly StoreDetailsRepository _storeRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(UserManager<AppUser> userManager, IStoreDetailService storeService, IMapper mapper, IWebHostEnvironment webHostEnvironment, StoreDetailsRepository storeRepository)
        {
            _userManager = userManager;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
            _storeService = storeService;
            _userManager = userManager;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _storeRepository = storeRepository;
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
        [HttpGet]
        public async Task<IActionResult> ManagementSeller()
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
            string apiURL = "https://localhost:5555/Gateway/ManagementSellerService";
            List<UsersViewModel> usersViews = new List<UsersViewModel>();
            try
            {
                var respone = await client.GetAsync(apiURL);
                if(!respone.IsSuccessStatusCode)
                {
                    return View(usersViews);
                }
                var mes = await respone.Content.ReadAsStringAsync();
                usersViews = JsonSerializer.Deserialize<List<UsersViewModel>>(mes, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(usersViews);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AcceptSeller([FromBody] UsersViewModel obj) {
            var admin = await _userManager.GetUserAsync (User);
            if(admin == null || !await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
             string apiURL = $"https://localhost:5555/Gateway/ManagementSellerService/Accept-seller/{obj.Email}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent,System.Text.Encoding.UTF8, "application/json");
                var respone = await client.PostAsync(apiURL, content);
                if (respone.IsSuccessStatusCode) {
                    return Json(new { success = true, message = "Cập nhật thành công" });
                } else
                {
                    return Json(new { success = false, message = "Cập nhật thất bại" });
                }

            }
            catch (Exception)
            {

                return Json(new { success = false, message = "Lỗi kết nối API Gateway!" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RejectSeller([FromBody] UsersViewModel obj)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            string apiURL = $"https://localhost:5555/Gateway/ManagementSellerService/Reject-seller/{obj.Email}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiURL,content);
                if(response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }else
                {
                    return Json(new { success = true, message = "Cập nhật thất bại" });
                }
            }
            catch (Exception)
            {

                return Json(new { success = false, message = "Lỗi kết nối API Gateway!" });
            }
        }


        public async Task<IActionResult> Hidden([FromBody] UsersViewModel obj)
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

            string apiURL = $"https://localhost:5555/Gateway/ManagementSellerService/Admin-Hiden{obj.Email}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var respone = await client.PostAsync(apiURL, content);
                if (respone.IsSuccessStatusCode)
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
            if (admin == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            string apiURL = $"https://localhost:5555/Gateway/ManagementSellerService/Admin-Show/{obj.Email}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var respone = await client.PostAsync(apiURL, content);
                if (respone.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành Công" });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật thất bại" });
                }

            }
            catch (Exception)
            {

                return Json(new { success = false, message = "Lỗi kết nối API Gateway!" }); ;
            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateByAdmin(AdminViewModel obj)
        {
            if (string.IsNullOrEmpty(obj.Email))
            {
                return Json(new { success = false, message = "Email is required" });
            }

            var getbyEmail = await _userManager.FindByEmailAsync(obj.Email);
            if (getbyEmail == null)
            {
                return Json(new { success = false, message = "Email not found" });
            }

            var UserModel = new AdminViewModel
            {
                Email = getbyEmail.Email,
                Address = getbyEmail.Address,
                Birthday = getbyEmail.Birthday,
                PhoneNumber = getbyEmail.PhoneNumber,
                UserName = getbyEmail.UserName,
            };

            return View(UserModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateByAdmin([FromBody] AdminViewModel obj1, string id)
        {
            if (obj1 == null || string.IsNullOrEmpty(obj1.Email))
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            var admin = await _userManager.GetUserAsync(User);
            if (admin == null)
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập!" });
            }
            if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return Json(new { success = false, message = "Bạn không có quyền cập nhật!" });
            }

            string apiURL = $"https://localhost:5555/Gateway/ManagementSellerService/Admin-Update/{obj1.Email}";

            try
            {
                var jsonContent = JsonSerializer.Serialize(obj1);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PutAsync(apiURL, content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật thất bại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi kết nối API Gateway! " + ex.Message });
            }
        }

        public async Task<IActionResult> ViewAdminStore()
        {
            var stores = await _storeService.GetInactiveStoresAsync();

            if (stores == null || stores.Count == 0)
            {
                TempData["Message"] = "No inactive stores found.";
            }

            return View(stores);
        }

        public async Task<IActionResult> ViewStoreRegistration()
        {
            var stores = await _storeService.GetStoreRegistrationRequestsAsync();

            if (stores == null || stores.Count == 0)
            {
                TempData["Message"] = "No inactive stores found.";
            }

            return View(stores);
        }

        [HttpPost]
        public async Task<IActionResult> Hidden([FromBody] Guid id)
        {
            var result = await _storeService.HideStoreAsync(id);
            return Json(new { success = result });
        }

        [HttpPost]
        public async Task<IActionResult> Show([FromBody] Guid id)
        {
            var result = await _storeService.ShowStoreAsync(id);
            return Json(new { success = result });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, bool isActive)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
            {
                return Json(new { success = false, message = "Store not found" });
            }

            // Đặt trạng thái mới dựa trên hành động
            store.IsActive = isActive ? true : false;

            var result = await _storeService.UpdateStoreAsync(store);
            if (result)
            {
                string statusText = isActive ? "shown" : "hidden";
                return Json(new { success = true, message = $"Store {statusText} successfully", isActive = store.IsActive });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update store status" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AcceptStore(Guid id)
        {
            var result = await _storeService.AcceptStoreAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Store approved successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve store.";
            }

            return RedirectToAction("ViewStoreRegistration");
        }

        [HttpPost]
        public async Task<IActionResult> RejectStore(Guid id)
        {
            var result = await _storeService.RejectStoreAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Store rejected successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject store.";
            }

            return RedirectToAction("ViewStoreRegistration");
        }

        [HttpPost]
        [Route("Admin/UpdateStoreStatus")]
        public async Task<JsonResult> UpdateStoreStatus(Guid storeId, string newStatus)
        {
            bool isUpdated = await _storeService.UpdateStoreStatusAsync(storeId, newStatus);

            if (!isUpdated)
            {
                return Json(new { success = false, message = "Store not found" });
            }

            return Json(new { success = true, message = "Store status updated successfully" });
        }
    }
}

