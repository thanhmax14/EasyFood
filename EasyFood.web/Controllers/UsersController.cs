using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;

namespace EasyFood.web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private HttpClient client = null;
        private string _url;
        private readonly IBalanceChangeService _balance;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _product;
        public readonly ICartService _cart;
        public readonly IProductVariantService _productWarian;
        private readonly IStoreDetailService _storeDetailService;
        private readonly IReviewService _reviewService;



        public UsersController(UserManager<AppUser> userManager, IBalanceChangeService balance, IHttpContextAccessor httpContextAccessor,
            IProductService product, ICartService cart, IProductVariantService productWarian, IStoreDetailService storeDetail, IReviewService reviewService)
        {
            _userManager = userManager;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
            _balance = balance;
            _httpContextAccessor = httpContextAccessor;
            _product = product;
            _cart = cart;
            _productWarian = productWarian;
            _storeDetailService = storeDetail;
            _reviewService = reviewService;
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
            //string urlFeedback = $"https://localhost:5555/Gateway/ReviewService/GetReviewByUserId/{userId}";
            try
            {
                var response = await client.GetAsync(apiUrl);
                var responceBalance = await this.client.GetAsync($"{urlBalace}/{user.Id}");
                //var feedbackTask = await client.GetAsync(urlFeedback);
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
                list.BalanceUser = await this._balance.GetBalance(user.Id);

                //var feedbackJson = await feedbackTask.Content.ReadAsStringAsync();

                //var feedbackOptions = new JsonSerializerOptions
                //{
                //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                //    PropertyNameCaseInsensitive = true
                //};
                //list.Reivew = JsonSerializer.Deserialize<List<ReivewViewModel>>(feedbackJson, feedbackOptions);

                return View(list);
            }
            catch (Exception)
            {
                return View(list);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] IndexUserViewModels obj)
        {
            if (obj.userView == null)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "You are not logged in." });
            }

            string apiUrl = $"https://localhost:5555/Gateway/UsersService/{user.Id}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Profile updated successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Phone number is already registered by another user!" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "API Gateway connection error!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterSeller(IndexUserViewModels model)
        {
            if (model.userView == null)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "You are not logged in." });
            }

            string apiUrl = $"https://localhost:5555/Gateway/UsersService/register-seller/{user.Id}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Registration successful!" });
                }
                else
                {
                    return Json(new { success = false, message = "Please update all required information before registering as a seller!" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "API Gateway connection error!" });
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
                return Json(new { success = false, msg = "Lỗi không xác định, vui lòng thử lại." });
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
                if (!responceBalance.IsSuccessStatusCode)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(long number, string code, string numAccount, string nameAcc)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new ErroMess { msg = "Bạn chưa đăng nhập!!" });
            }
            if (number < 500000)
                return Json(new ErroMess { msg = "Rút tối thiểu 500,000 VND" });
            if (number % 1 != 0)
                return Json(new ErroMess { msg = "Số tiền phải là số nguyên, không được có phần thập phân." });

            if (string.IsNullOrWhiteSpace(code)) return Json(new ErroMess { msg = "Vui lòng chọn lại số tài khoản" });
            if (string.IsNullOrWhiteSpace(numAccount)) return Json(new ErroMess { msg = "Vui lòng nhập số tài khoản" });
            if (string.IsNullOrWhiteSpace(nameAcc)) return Json(new ErroMess { msg = "Vui lòng nhập tên tài khoản" });

            try
            {
                var messErro = new ErroMess();
                this._url = "https://api.vietqr.io/v2/banks";
                var responce = await this.client.GetAsync(_url);

                responce.EnsureSuccessStatusCode();

                string json = await responce.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("data", out JsonElement banks))
                    {
                        foreach (JsonElement bank in banks.EnumerateArray())
                        {
                            if (bank.TryGetProperty("code", out JsonElement codeElement) &&
                                codeElement.GetString().Equals(code, StringComparison.OrdinalIgnoreCase))
                            {
                                var temModels = new WithdrawViewModels
                                {
                                    UserID = user.Id,
                                    AccountName = nameAcc,
                                    accountNumber = numAccount,
                                    amount = number,
                                    BankName = bank.GetProperty("shortName").GetString()
                                };

                                this._url = $"https://localhost:5555/Gateway/WalletService/WithdrawPayment";
                                string jsonData = JsonSerializer.Serialize(temModels);
                                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                                var response = await client.PostAsync($"{this._url}", content);
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                    PropertyNameCaseInsensitive = true
                                };
                                var mes = await response.Content.ReadAsStringAsync();
                                messErro = JsonSerializer.Deserialize<ErroMess>(mes, options);
                                if (response.IsSuccessStatusCode)
                                {
                                    return Json(messErro);
                                }
                                return Json(messErro);

                            }
                        }
                    }
                    return Json(new ErroMess { msg = "Đã xảy ra lỗi, hãy thử lại hoặc liên hệ admin!!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new ErroMess { msg = "Đã xảy ra lỗi, hãy thử lại hoặc liên hệ admin!!" });
            }


        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy([FromForm] List<Guid> productIds)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new ErroMess { msg = "Bạn chưa đăng nhập!!" });
            }

            if (productIds == null || !productIds.Any())
            {
                return Json(new ErroMess { msg = "Vui lòng chọn sản phẩm cần mua!" });
            }
            foreach (var id in productIds)
            {
                var product = await _product.GetAsyncById(id);
                if (product == null)
                {
                    return Json(new ErroMess { msg = "Sản phẩm mua không tồn tại!" });
                }
                var checkcart = await this._cart.FindAsync(u => u.UserID == user.Id && u.ProductID == id);
                if (checkcart == null)
                {
                    return Json(new ErroMess { msg = "Sản phẩm mua không tồn tại trong giỏ hàng!" });
                }
                var getQuatity = await this._productWarian.FindAsync(u => u.ProductID == id);
                if (checkcart.Quantity > getQuatity.Stock)
                {
                    return Json(new ErroMess { msg = "Số lượng sản phẩm mua vượt quá số lượng tồn kho!" });
                }

            }








            return Ok(new { message = "Danh sách sản phẩm đã được xử lý.", selectedProducts = productIds });
        }


        //public async Task<IActionResult> FeedbackListByUserId()
        //{
        //    // Kiểm tra người dùng có đăng nhập hay không
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }

        //    // Lấy ID của user đăng nhập
        //    string userId = user.Id;
        //    var list = new IndexUserViewModels();

        //    // Gọi API Gateway để lấy thông tin user theo ID
        //    string apiUrl = $"https://localhost:5555/Gateway/UsersService/View-Profile/{userId}";
        //    string urlBalance = $"https://localhost:5555/Gateway/WalletService/GetWallet/{userId}";
        //    string urlFeedback = $"https://localhost:5555/Gateway/ReviewService/GetReviewByUserId/{userId}";

        //    try
        //    {
        //        // Gọi API song song để tối ưu hiệu suất
        //        var userTask = client.GetAsync(apiUrl);
        //        var balanceTask = client.GetAsync(urlBalance);
        //        var feedbackTask = client.GetAsync(urlFeedback);

        //        await Task.WhenAll(userTask, balanceTask, feedbackTask);

        //        var responseUser = userTask.Result;
        //        var responseBalance = balanceTask.Result;
        //        var responseFeedback = feedbackTask.Result;

        //        if (!responseUser.IsSuccessStatusCode || !responseBalance.IsSuccessStatusCode || !responseFeedback.IsSuccessStatusCode)
        //        {
        //            return View(list);
        //        }

        //        // Xử lý API User
        //        //var userJson = await responseUser.Content.ReadAsStringAsync();
        //        //list.userView = JsonSerializer.Deserialize<UsersViewModel>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //        // Xử lý API Balance
        //        //var balanceJson = await responseBalance.Content.ReadAsStringAsync();
        //        //var balanceOptions = new JsonSerializerOptions
        //        //{
        //        //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //        //    PropertyNameCaseInsensitive = true
        //        //};
        //        //list.Balance = JsonSerializer.Deserialize<List<BalanceListViewModels>>(balanceJson, balanceOptions);
        //        //list.BalanceUser = await _balance.GetBalance(user.Id);

        //        // Xử lý API Feedback
        //        var feedbackJson = await responseFeedback.Content.ReadAsStringAsync();
        //        //list.Reivew = JsonSerializer.Deserialize<List<ReivewViewModel>>(feedbackJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //        var feedbackOptions = new JsonSerializerOptions
        //        {
        //            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //            PropertyNameCaseInsensitive = true
        //        };
        //        list.Reivew = JsonSerializer.Deserialize<List<ReivewViewModel>>(feedbackJson, feedbackOptions);
        //        return View(list);
        //    }
        //    catch (Exception)
        //    {
        //        return View(list);
        //    }
        //}
    }
}
