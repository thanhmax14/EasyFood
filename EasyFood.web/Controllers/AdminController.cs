using System.Net.Http.Headers;
using System.Text.Json;
using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.StoreDetails;
using Repository.ViewModels;

namespace EasyFood.web.Controllers
{

    public class AdminController : Controller
    {
        private readonly IBalanceChangeService _balance; // xử lý withdaw
        private readonly UserManager<AppUser> _userManager;
        private HttpClient client = null;
        private string url;
        private readonly IStoreDetailService _storeService;
        private readonly StoreDetailsRepository _storeRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICategoryService _categoryService;

        public AdminController(UserManager<AppUser> userManager, IStoreDetailService storeService, IMapper mapper, IWebHostEnvironment webHostEnvironment, StoreDetailsRepository storeRepository, IBalanceChangeService balance, ICategoryService categoryService)
        {
            _userManager = userManager;
            _balance = balance;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
            _storeService = storeService;
            _userManager = userManager;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _storeRepository = storeRepository;
            _categoryService = categoryService;
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
            if (admin == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            string apiURL = "https://localhost:5555/Gateway/ManagementSellerService";
            List<UsersViewModel> usersViews = new List<UsersViewModel>();
            try
            {
                var respone = await client.GetAsync(apiURL);
                if (!respone.IsSuccessStatusCode)
                {
                    return View(usersViews);
                }
                var mes = await respone.Content.ReadAsStringAsync();
                usersViews = JsonSerializer.Deserialize<List<UsersViewModel>>(mes, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                usersViews = usersViews.OrderByDescending(x => x.RequestSeller == "1").ToList();

                return View(usersViews);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AcceptSeller([FromBody] UsersViewModel obj)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            string apiURL = $"https://localhost:5555/Gateway/ManagementSellerService/Accept-seller/{obj.Email}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var respone = await client.PostAsync(apiURL, content);
                if (respone.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                else
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
                var response = await client.PostAsync(apiURL, content);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                else
                {
                    return Json(new { success = true, message = "Cập nhật thất bại" });
                }
            }
            catch (Exception)
            {

                return Json(new { success = false, message = "Lỗi kết nối API Gateway!" });
            }
        }
     
        public async Task<IActionResult> HiddenAccount([FromBody] UsersViewModel obj)
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

            string apiURL = $"https://localhost:5555/Gateway/ManagementSellerService/Admin-Hiden/{obj.Email}";
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
        public async Task<IActionResult> ShowAccount([FromBody] UsersViewModel obj)
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
        [Route("Admin/UpdateStoreStatus/{storeId}/{newStatus}")]
        public async Task<JsonResult> UpdateStoreStatus(Guid storeId, string newStatus)
        {
            bool isUpdated = await _storeService.UpdateStoreStatusAsync(storeId, newStatus);

            if (!isUpdated)
            {
                return Json(new { success = false, message = "Store not found" });
            }

            return Json(new { success = true, message = "Store status updated successfully", newStatus });
        }

        public async Task<IActionResult> WithdrawList()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string apiUrl = "https://localhost:5555/Gateway/WithdrawService/GetWithdraw";
            List<WithdrawAdminListViewModel> withdrawList = new List<WithdrawAdminListViewModel>();

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return View(withdrawList);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                withdrawList = JsonSerializer.Deserialize<List<WithdrawAdminListViewModel>>(responseContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(withdrawList);
            }
            catch (Exception)
            {
                return View(withdrawList);
            }
        }
        public async Task<IActionResult> WithdrawDetails(string id)
        {
            try
            {
                // Kiểm tra ID có hợp lệ không
                if (!Guid.TryParse(id, out Guid withdrawId))
                {
                    return Json(new ErroMess { success = false, msg = "ID không hợp lệ." });
                }

                // Tìm thông tin rút tiền theo ID
                var withdraw = await _balance.FindAsync(w => w.ID == withdrawId);

                if (withdraw == null)
                {
                    return Json(new ErroMess { success = false, msg = "Không tìm thấy yêu cầu rút tiền." });
                }

                // Lấy thông tin người dùng
                var user = await _userManager.FindByIdAsync(withdraw.UserID);
                if (user == null)
                {
                    return Json(new ErroMess { success = false, msg = "Người dùng không tồn tại." });
                }

                // Tạo ViewModel để hiển thị trong View
                var withdrawModel = new WithdrawAdminListViewModel
                {
                    ID = withdraw.ID,
                    UserName = user.UserName,
                    MoneyChange = withdraw.MoneyChange,
                    StartTime = withdraw.StartTime,
                    DueTime = withdraw.DueTime,
                    Description = withdraw.Description,
                    Status = withdraw.Status,
                    Method = withdraw.Method,
                    UserID = withdraw.UserID
                };

                return View(withdrawModel);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để debug sau này
                Console.WriteLine($"Lỗi: {ex.Message}");

                // Trả về lỗi JSON để tránh chết chương trình
                return Json(new ErroMess { success = false, msg = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AcceptWithdraw(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid guidId))
                return BadRequest(new ErroMess { msg = "ID không hợp lệ!" });
            var flag = await _balance.FindAsync(p => p.ID == guidId);
            if (flag == null)
                return NotFound(new ErroMess { msg = "Không tìm thấy yêu cầu rút tiền!" });
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new ErroMess { msg = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            this.url = $"https://localhost:5555/Gateway/WithdrawService/AcceptWithdraw";
            try
            {
                var response = await client.GetAsync($"{this.url}/{id}");
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
                return Json(new { success = false, msg = "Lỗi không xác định, vui lòng thử lại." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectWithdraw(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid guidId))
                return BadRequest(new ErroMess { msg = "ID không hợp lệ!" });
            var flag = await _balance.FindAsync(p => p.ID == guidId);
            if (flag == null)
                return NotFound(new ErroMess { msg = "Không tìm thấy yêu cầu rút tiền!" });
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new ErroMess { msg = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            this.url = "https://localhost:5555/Gateway/WithdrawService/RejectWithdraw";
            try
            {
                var response = await client.GetAsync($"{this.url}/{id}");
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
                return Json(new { success = false, msg = "Lỗi không xác định, vui lòng thử lại." });
            }

        }

        [HttpPost]
        [Route("Admin/UpdateStoreIsActive")]
        public async Task<JsonResult> UpdateStoreIsActive(Guid storeId, bool isActive)
        {
            bool isUpdated = await _storeService.UpdateStoreIsActiveAsync(storeId, isActive);

            if (!isUpdated)
            {
                return Json(new { success = false, message = "Store not found" });
            }

            return Json(new { success = true, message = "Store status updated successfully", isActive });
        }

        public async Task<IActionResult> ViewCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                _categoryService.CreateCategory(model);
                return RedirectToAction("ViewCategories", "Admin");
            }

            return View(model);
        }

        [HttpGet("Admin/UpdateCategory/{id}")]
        public IActionResult UpdateCategory(Guid id)
        {
            var model = _categoryService.GetCategoryForUpdate(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateCategory(CategoryUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _categoryService.UpdateCategory(model);
                return RedirectToAction("ViewCategories", "Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}



