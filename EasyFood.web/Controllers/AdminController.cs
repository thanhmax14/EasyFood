using System.Net.Http.Headers;
using System.Text.Json;
using BusinessLogic.Services.BalanceChanges;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;

namespace EasyFood.web.Controllers
{

    public class AdminController : Controller
    {
        private readonly IBalanceChangeService _balance; // xử lý withdaw
        private readonly UserManager<AppUser> _userManager;
        private HttpClient client = null;
        private string url;
        public AdminController(UserManager<AppUser> userManager, IBalanceChangeService balance)
        {
            _userManager = userManager;
            _balance = balance;
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
        public async Task<IActionResult> AcceptWithdraw(WithdrawAdminListViewModel model)
        {
            try
            {

                string apiUrl = $"https://localhost:5555/Gateway/WithdrawService/AcceptWithdraw/{model.ID}";

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
                    return Redirect("/Admin/WithdrawList");
                }

                return Json(dataRepone);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi kết nối API Gateway! " + ex.Message });
            }
        }






    }
}



