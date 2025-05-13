using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.Wishlists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Models;
using Net.payOS;
using Repository.ViewModels;

namespace EasyFood.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private HttpClient client = null;
        private string _url;
        private readonly ICartService _cart;
        private readonly IWishlistServices _wishlist;
        private readonly IProductService _product;
        private readonly IProductImageService _productimg;
        private readonly IProductVariantService _productvarian;
        private readonly IStoreDetailService _storeDetailService;
        private readonly IBalanceChangeService _balance;
        private readonly IOrdersServices _order;
        private readonly ICategoryService _categoryService;

        private readonly PayOS _payos;



        public HomeController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ICategoryService categoryService, IStoreDetailService storeDetailService, IEmailSender emailSender, ICartService cart, IWishlistServices wishlist, IProductService product
, IProductImageService productimg, IProductVariantService productvarian, IBalanceChangeService balance, IOrdersServices order, PayOS payos)
        {
            _categoryService = categoryService;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
            _cart = cart;
            _wishlist = wishlist;
            _product = product;
            _productimg = productimg;
            _productvarian = productvarian;
            _storeDetailService = storeDetailService;
            _balance = balance;
            _order = order;
            _payos = payos;
        }




        public async Task<IActionResult> LearnMore()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            var list = new List<ProductsViewModel>();
            this._url = "https://localhost:5555/Gateway/ProductsService/GetAllProducts";
            try
            {
                var response = await client.GetAsync(this._url);
                if (!response.IsSuccessStatusCode)
                {
                    return View(list);
                }

                var mes = await response.Content.ReadAsStringAsync();
                list = JsonSerializer.Deserialize<List<ProductsViewModel>>(mes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(list);
            }
            catch (Exception ex)
            {
                return View(list);
            }
        }
        [HttpGet]
        public IActionResult Login(string ReturnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) && ReturnUrl != Url.Action("Login", "Home")
                && ReturnUrl != Url.Action("Register", "Home") && ReturnUrl != Url.Action("Forgot", "Home")
                && ReturnUrl != Url.Action("ResetPassword", "Home")
                )
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, bool? rememberMe, string ReturnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) && ReturnUrl != Url.Action("Login", "Home")
                && ReturnUrl != Url.Action("Register", "Home") && ReturnUrl != Url.Action("Forgot", "Home")
                && ReturnUrl != Url.Action("ResetPassword", "Home")
                )
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }
            this._url = "https://localhost:5555/Gateway/authenticate/login";
            var temdata = new
            {
                Username = $"{username}",
                Password = $"{password}"
            };

            string json = JsonSerializer.Serialize(temdata);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            try
            {
                var response = await client.PostAsync($"{this._url}", content);
                var mes = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(mes);
                var root = doc.RootElement;
                var status = root.GetProperty("status").GetString();
                var msg = root.GetProperty("msg").GetString();

                if (response.IsSuccessStatusCode)
                {


                    var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
                    var roles = await _userManager.GetRolesAsync(user);


                    await _signInManager.SignInAsync(user, isPersistent: rememberMe ?? false);
                    if (roles.Contains("Admin"))
                    {
                        return Json(new
                        {
                            status,
                            msg,
                            redirectUrl = "/admin"
                        });
                    }
                    return Json(new
                    {
                        status,
                        msg,
                        redirectUrl = ViewData["ReturnUrl"]?.ToString()
                    });
                }
                return Json(new { status, msg });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = "Lỗi không xác định, vui lòng thử lại." });
            }
        }

        [HttpGet]
        public IActionResult Register(string ReturnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = User.Identity.Name;
                return RedirectToAction("Index", "Home");
            }
            // Xử lý ReturnUrl tương tự GET
            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) && ReturnUrl != Url.Action("Login", "Home")
                && ReturnUrl != Url.Action("Register", "Home") && ReturnUrl != Url.Action("Forgot", "Home")
                && ReturnUrl != Url.Action("ResetPassword", "Home")
                )
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (ModelState.ContainsKey("Username") && ModelState["Username"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["Username"].Errors[0].ErrorMessage });
                }

                if (ModelState.ContainsKey("Email") && ModelState["Email"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["Email"].Errors[0].ErrorMessage });
                }

                if (ModelState.ContainsKey("Password") && ModelState["Password"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["Password"].Errors[0].ErrorMessage });
                }

                if (ModelState.ContainsKey("repassword") && ModelState["repassword"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["repassword"].Errors[0].ErrorMessage });
                }

                return Json(new { status = "error", msg = "Dữ liệu không hợp lệ" });
            }

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl != Url.Action("Login", "Home")
                && model.ReturnUrl != Url.Action("Register", "Home") && model.ReturnUrl != Url.Action("Forgot", "Home")
                && model.ReturnUrl != Url.Action("ResetPassword", "Home")
                )
            {
                ViewData["ReturnUrl"] = model.ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }

            this._url = "https://localhost:5555/Gateway/authenticate/register";
            var temp = new RegisterViewModel
            {
                Email = model.Email,
                Password = model.Password,
                repassword = model.repassword,
                Username = model.Username,
                ReturnUrl = "",
            };
            string json = JsonSerializer.Serialize(temp);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync($"{this._url}", content);
                var mes = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(mes);
                var root = doc.RootElement;
                var status = root.GetProperty("status").GetString();
                var msg = root.GetProperty("msg").GetString();

                if (response.IsSuccessStatusCode)
                {
                    var user = await _userManager.FindByNameAsync(model.Username) ?? await _userManager.FindByEmailAsync(model.Username);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Home",
                       new { userId = user.Id, token = token }, Request.Scheme);

                    await _emailSender.SendEmailAsync(user.Email, "Xác nhận email",
                        $"Vui lòng nhấp vào liên kết để xác nhận email: {confirmationLink}");

                    return Json(new
                    {
                        status,
                        msg,
                        redirectUrl = ViewData["ReturnUrl"]?.ToString()
                    });
                }
                return Json(new { status, msg });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = "Lỗi không xác định, vui lòng thử lại." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ResendConfirmationEmail(string username)
        {
            this._url = "https://localhost:5555/Gateway/authenticate/ResendEmail";
            var temp = new { username };
            var content = new StringContent($"\"{username}\"", Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync(this._url, content);
                var mes = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(mes);
                var root = doc.RootElement;
                var status = root.GetProperty("status").GetString();
                var msg = root.GetProperty("msg").GetString();

                return Json(new { status, msg });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { status = "error", msg = "Lỗi không xác định, vui lòng thử lại." });
            }
        }

        public async Task<IActionResult> ListProducts(string searchName, decimal? minPrice = null, decimal? maxPrice = null, int filterCount = 0)
        {
            var list = new List<ProductsViewModel>();
            string url = "https://localhost:5555/Gateway/ProductsService/GetAllProducts"; // Chỉ gọi GetAllProducts

            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.MinPrice = minPrice ?? 0;
                    ViewBag.MaxPrice = maxPrice ?? 2000;
                    ViewBag.FilterCount = filterCount;
                    ViewBag.ErrorMessage = "Không thể tải danh sách sản phẩm từ API.";
                    return View(list); // Danh sách rỗng nếu lỗi
                }

                var mes = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(mes))
                {
                    ViewBag.MinPrice = minPrice ?? 0;
                    ViewBag.MaxPrice = maxPrice ?? 2000;
                    ViewBag.FilterCount = filterCount;
                    ViewBag.ErrorMessage = "API trả về dữ liệu rỗng.";
                    return View(list); // Danh sách rỗng nếu rỗng
                }

                // Lấy toàn bộ danh sách từ API
                list = JsonSerializer.Deserialize<List<ProductsViewModel>>(mes, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                });

                // Lọc danh sách theo searchName (nếu có)
                if (!string.IsNullOrEmpty(searchName))
                {
                    list = list.Where(p => p.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Lọc danh sách theo giá (nếu có minPrice hoặc maxPrice)
                if (minPrice.HasValue || maxPrice.HasValue)
                {
                    decimal min = minPrice ?? 0;
                    decimal max = maxPrice ?? decimal.MaxValue; // Nếu không có maxPrice, lấy giá trị lớn nhất
                    list = list.Where(p => p.Price >= min && p.Price <= max).ToList();
                    filterCount++; // Tăng số lần lọc
                }

                // Cập nhật ViewBag để đồng bộ slider và đếm số lần lọc
                ViewBag.MinPrice = minPrice ?? 0;
                ViewBag.MaxPrice = maxPrice ?? 2000;
                ViewBag.FilterCount = filterCount;

                return View(list); // Trả về danh sách đã lọc
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ListProducts: {ex.Message}\n{ex.StackTrace}");
                ViewBag.MinPrice = minPrice ?? 0;
                ViewBag.MaxPrice = maxPrice ?? 2000;
                ViewBag.FilterCount = filterCount;
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải danh sách sản phẩm.";
                return View(list);
            }
        }


        public async Task<IActionResult> GetAllCategory(string searchName)

        {
            var list = new List<CategoryViewModel>();
            this._url = "https://localhost:5555/Gateway/CategoryService/GetAllCategory";
            if (!string.IsNullOrEmpty(searchName))
            {
                this._url = $"https://localhost:5555/Gateway/CategoryService/SearchCategoryByName?searchName={searchName}";
            }
            try
            {
                var response = await client.GetAsync(this._url);
                if (!response.IsSuccessStatusCode)
                {
                    return View(list);
                }

                var mes = await response.Content.ReadAsStringAsync();
                list = JsonSerializer.Deserialize<List<CategoryViewModel>>(mes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(list);
            }
            catch (Exception ex)
            {
                return View(list);
            }
        }



        public async Task<IActionResult> GetAllProductOfCategory(Guid iD)
        {
            var FindProduct = await _categoryService.FindAsync(x => x.ID == iD);
            if (FindProduct != null)
            {
                
                var list = new CategoryDetailsViewModel();
                this._url = $"https://localhost:5555/Gateway/CategoryService/GetAllProductOfCategory?id={iD}";


                try
                {
                    var response = await client.GetAsync(this._url);
                    if (!response.IsSuccessStatusCode)
                    {
                        return View(list);
                    }

                    var mes = await response.Content.ReadAsStringAsync();
                    list = JsonSerializer.Deserialize<CategoryDetailsViewModel>(mes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return View(list);
                }
                catch (Exception ex)
                {
                    return View(list);
                }
            }
            return NotFound();
        }


        public async Task<IActionResult> ProductDetail(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var FindProduct = await _product.FindAsync(x => x.ID == id);
            if (FindProduct == null)
            {
                return NotFound();
            }

            var list = new ProductDetailsViewModel();
            this._url = $"https://localhost:5555/Gateway/ProductsService/GetProductDetails?id={id}";
            string commentApi = $"https://localhost:5555/Gateway/ProductsService/GetAllComment/{id}";

            try
            {
                // Gọi API lấy thông tin sản phẩm
                var response = await client.GetAsync(this._url);
                if (response.IsSuccessStatusCode)
                {
                    var mes = await response.Content.ReadAsStringAsync();
                    list = JsonSerializer.Deserialize<ProductDetailsViewModel>(mes, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                // Gọi API lấy danh sách bình luận
                var commentResponse = await client.GetAsync(commentApi);
                if (commentResponse.IsSuccessStatusCode)
                {
                    var commentMes = await commentResponse.Content.ReadAsStringAsync();
                    var comments = JsonSerializer.Deserialize<List<CommentViewModels>>(commentMes, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    list.Comments = comments ?? new List<CommentViewModels>();
                }

                return View(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }



        public async Task<IActionResult> GetAllStore(string searchName)
        {
            var list = new List<StoreViewModel>();
            this._url = "https://localhost:5555/Gateway/StoreDetailService/GetAllStores";
            if (!string.IsNullOrEmpty(searchName))
            {
                this._url = $"https://localhost:5555/Gateway/StoreDetailService/SearchStoreByName?searchName={searchName}";
            }
            try
            {
                var response = await client.GetAsync(this._url);
                if (!response.IsSuccessStatusCode)
                {
                    return View(list);
                }
                var mes = await response.Content.ReadAsStringAsync();
                list = JsonSerializer.Deserialize<List<StoreViewModel>>(mes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(list);
            }
            catch (Exception ex)
            {
                {
                    return View(list);
                }
            }
        }



        public async Task<IActionResult> GetStoreDetail(Guid iD)
        {

            var FindStore = await _storeDetailService.FindAsync(x => x.ID == iD);

            if (FindStore != null)
            {
                var list = new StoreDetailsViewModels();
                this._url = $"https://localhost:5555/Gateway/StoreDetailService/GetStoreDetail?id={iD}";

                try
                {
                    var response = await client.GetAsync(this._url);
                    if (!response.IsSuccessStatusCode)
                    {
                        return View(list);
                    }

                    var mes = await response.Content.ReadAsStringAsync();
                    list = JsonSerializer.Deserialize<StoreDetailsViewModels>(mes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return View(list);
                }
                catch (Exception ex)
                {
                    return View(list);
                }
            }





            return NotFound();
        }


        [HttpGet]
        public IActionResult Forgot()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgot(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { status = "error", msg = "Email không hợp lệ" });
            }

            this._url = "https://localhost:5555/Gateway/authenticate/Forgot";
            var content = new StringContent($"\"{email}\"", Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(this._url, content);
                var mes = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(mes);
                var root = doc.RootElement;
                var status = root.GetProperty("status").GetString();
                var msg = root.GetProperty("msg").GetString();
                if (response.IsSuccessStatusCode)
                {
                    var user = await _userManager.FindByNameAsync(email) ?? await _userManager.FindByEmailAsync(email);
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ResetPassword", "Home", new { token = token, email = email }, protocol: HttpContext.Request.Scheme);
                    var subject = "Đặt lại mật khẩu của bạn";
                    var body = $"Vui lòng nhấp vào đường dẫn dưới đây để đặt lại mật khẩu của bạn: <br/><a href='{resetLink}'>Đặt lại mật khẩu</a>";
                    await _emailSender.SendEmailAsync(email, subject, body);
                    return Json(new { status, msg });
                }
                return Json(new { status, msg });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { status = "error", msg = "Lỗi không xác định, vui lòng thử lại." });
            }
        }


        public async Task<IActionResult> Logout()
        {

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var getUser = await this._userManager.FindByIdAsync(userId);
                getUser.LastAccess = DateTime.Now;
                await this._userManager.UpdateAsync(getUser);
            }
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return BadRequest("Invalid request");
            }

            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (ModelState.ContainsKey("Password") && ModelState["Password"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["Password"].Errors[0].ErrorMessage });
                }

                if (ModelState.ContainsKey("ConfirmPassword") && ModelState["ConfirmPassword"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["ConfirmPassword"].Errors[0].ErrorMessage });
                }
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Json(new { status = "error", msg = "Email không tồn tại trong hệ thống." });
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return Json(new { status = "success", msg = "Mật khẩu đã được thay đổi thành công." });
            }

            var firstResultError = result.Errors.FirstOrDefault()?.Description;
            if (firstResultError == "Invalid token.")
            {
                firstResultError = "Link cập nhật mật khẩu đã hết hạn!!";
            }

            return Json(new { status = "error", msg = firstResultError ?? "Cập nhật mật khẩu thất bại." });

        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Xác nhận email thành công.");
            }

            return BadRequest("Xác nhận email không thành công.");
        }


        /* public async Task<IActionResult> Cart()
         {
             var getCart = this._cart.GetCartFromSession();
             var listtem = new List<CartViewModels>();

             if (getCart.Any())
             {
                 foreach (var item in getCart)
                 {




                     listtem.Add(new CartViewModels
                     {

                     });
                 }
             }


             return View();
         }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddWish(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            var check = await this._product.FindAsync(x => x.ID == id);
            if (check == null)
            {
                return Json(new { success = false, message = "Product không tồn tại!!" });
            }
            else if (await this._wishlist.FindAsync(x => x.ProductID == id && x.UserID == user.Id) != null)
            {
                return Json(new { success = false, message = $"{check.Name} đã tồn tại trong danh sách yêu thích.!" });
            }
            else
            {
                var tem = new Wishlist
                {
                    CreateDate = DateTime.Now,
                    UserID = user.Id,
                    ProductID = check.ID
                };
                try
                {
                    await this._wishlist.AddAsync(tem);
                    await this._wishlist.SaveChangesAsync();
                    return Json(new { success = true, message = $"Thêm thành công!" });
                }
                catch
                {
                    return Json(new { success = false, message = "Thêm thất bại!" });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public async Task<IActionResult> RemoveWish(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            var check = await this._product.FindAsync(x => x.ID == id);
            if (check == null)
            {
                return Json(new { success = false, message = "Product không tồn tại!!" });
            }
            var wshlict = await this._wishlist.FindAsync(x => x.ProductID == id && x.UserID == user.Id);
            if (wshlict == null)
            {
                return Json(new { success = false, message = $"{check.Name} không tồn tại trong danh sách yêu thích.!" });
            }
            else
            {

                try
                {
                    await this._wishlist.DeleteAsync(wshlict);
                    await this._wishlist.SaveChangesAsync();
                    return Json(new { success = true, message = $"Xoa thành công!" });
                }
                catch
                {
                    return Json(new { success = false, message = "Xoa thất bại!" });
                }
            }
        }

        public async Task<IActionResult> Wishlist()
        {
            var list = new List<wishlistViewModels>();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var wshlict = await this._wishlist.ListAsync(x => x.UserID == user.Id, orderBy: q => q.OrderByDescending(s => s.CreateDate));
            if (wshlict.Any())
            {
                foreach (var item in wshlict)
                {
                    var getProduct = await this._product.FindAsync(p => p.ID == item.ProductID);
                    if (getProduct != null)
                    {
                        var getimg = await this._productimg.ListAsync(u => u.ProductID == getProduct.ID);
                        var img = "https://nest-frontend-v6.vercel.app/assets/imgs/shop/product-1-1.jpg";
                        if (getimg.Any())
                        {
                            img = getimg.FirstOrDefault().ImageUrl;
                        }
                        var defauPrice = 0.0m;

                        var getPrice = await this._productvarian.FindAsync(u => u.ProductID == getProduct.ID);
                        if (getPrice != null)
                        {
                            defauPrice = getPrice.SellPrice;
                        }
                        list.Add(new wishlistViewModels
                        {
                            ID = item.ID,
                            img = img,
                            name = getProduct.Name,
                            price = defauPrice,
                            ProductID = getProduct.ID,
                            vote = 100
                        });
                    }
                }
            }
            return View(list);
        }
        [Route("Error/404")]
        public IActionResult NotFoundPage()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Invoice(string id)
        {
            var tem = new InvoiceViewModels();

            if (int.TryParse(id, out var orderCode))
            {
                var flagBalance = await this._balance.FindAsync(u => u.OrderCode == orderCode);
                if (flagBalance != null)
                {
                    var getUser = await this._userManager.FindByIdAsync(flagBalance.UserID);
                    if (getUser == null)
                    {
                        return RedirectToAction("NotFoundPage", "Home");
                    }
                    if (!flagBalance.IsComplete)
                    {
                        var checkORder = await this._payos.getPaymentLinkInformation(orderCode);
                        var status = checkORder.status.ToUpper();
                        switch (status)
                        {
                            case "CANCELLED":
                                flagBalance.DueTime = DateTime.Now;
                                flagBalance.Status = status;
                                flagBalance.IsComplete = true;
                                break;
                            case "PENDING":
                                flagBalance.DueTime = DateTime.Now;
                                flagBalance.Status = status;
                                break;
                            case "EXPIRED":
                                flagBalance.DueTime = DateTime.Now;
                                flagBalance.Status = status;
                                break;
                            case "UNDERPAID":
                                flagBalance.DueTime = DateTime.Now;
                                flagBalance.Status = status;
                                break;
                            case "PROCESSING":
                                flagBalance.DueTime = DateTime.Now;
                                flagBalance.Status = status;
                                break;
                            case "FAILED":
                                flagBalance.DueTime = DateTime.Now;
                                flagBalance.Status = status;
                                break;
                            default:
                                break;


                        }
                        await this._balance.UpdateAsync(flagBalance);
                        try
                        {
                            await this._balance.SaveChangesAsync();
                        }
                        catch
                        {
                            return RedirectToAction("NotFoundPage", "Home");
                        }
                        tem.orderCoce = id;
                        tem.invoiceDate = flagBalance.StartTime;
                        tem.DueDate = flagBalance.DueTime;
                        tem.NameUse = getUser.FirstName + " " + getUser.LastName;
                        tem.paymentMethod = "Online Banking";
                        tem.status = flagBalance.Status;
                        tem.emailUser = getUser.Email;
                        tem.phoneUser = getUser.PhoneNumber;
                        tem.tax = 0;
                        tem.AddressUse = getUser.Address;
                        tem.itemList.Add(new ItemInvoice
                        {
                            nameItem = $"Deposit to {getUser.UserName}",
                            amount = flagBalance.MoneyChange,
                            quantity = 1,
                            unitPrice = flagBalance.MoneyChange
                        });
                        tem.invoiceDate = flagBalance.StartTime;
                    }
                    if (flagBalance.Method.Equals("Deposit", StringComparison.OrdinalIgnoreCase))
                    {
                        tem.orderCoce = id;
                        tem.invoiceDate = flagBalance.StartTime;
                        tem.DueDate = flagBalance.DueTime;
                        tem.NameUse = getUser.FirstName + " " + getUser.LastName;
                        tem.paymentMethod = "Online Banking";
                        tem.status = flagBalance.Status;
                        tem.emailUser = getUser.Email;
                        tem.phoneUser = getUser.PhoneNumber;
                        tem.tax = 0;
                        tem.AddressUse = getUser.Address;
                        tem.itemList.Add(new ItemInvoice
                        {
                            nameItem = $"Deposit to {getUser.UserName}",
                            amount = flagBalance.MoneyChange,
                            quantity = 1,
                            unitPrice = flagBalance.MoneyChange
                        });
                        tem.invoiceDate = flagBalance.StartTime;
                    }
                }

            }
            else
            {

            }

            return View(tem);
        }
        [HttpPost]
        public async Task<IActionResult> CheckQuantity([FromBody] CartItem obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string apiUrl = $"https://localhost:5555/Gateway/UsersService/UpdateCart/{user.Id}";
            var requestData = new
            {

                ProductId = obj.ProductID,
                Quantity = obj.quantity // ❌ Lỗi model.Quantity đã được sửa
            };

            try
            {
                var jsonContent = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Add to quantity to success!" });
                }
                else
                {
                    return Json(new { success = false, message = "Add to quantity to  fail!" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Lỗi server.", error = ex.Message });
            }
        }
        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string apiUrl = $"https://localhost:5555/Gateway/UsersService/ViewCartDetail/{user.Id}";
            List<CartViewModels> cartItems = new List<CartViewModels>();
            try
            {
                var response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return View(cartItems);
                }
                var mes = await response.Content.ReadAsStringAsync();
                cartItems = JsonSerializer.Deserialize<List<CartViewModels>>(mes, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
                return View(cartItems);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }

        }

        [HttpPost]
        public async Task<IActionResult> GetBalance()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new ErroMess { msg = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            var balance = await this._balance.GetBalance(user.Id);
            return Json(new ErroMess { success = true, msg = $"{balance}" });
        }
        public async Task<IActionResult> DeleteCart([FromBody] CartItem obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Home", "Login");
            }
            string apiUrl = $"https://localhost:5555/Gateway/UsersService/DeleteCart/{obj.ProductID}";
            try
            {
                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Delete to product to success!" });
                }
                else
                {
                    return Json(new { success = false, message = "Delete to product to  fail!" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Lỗi server.", error = ex.Message });
            }
        }
        public async Task<IActionResult> AddToCart([FromBody] CartViewModels obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // 🔍 Check if the product exists
            var productVarian = await _productvarian.FindAsync(x => x.ProductID == obj.ProductID);
            if (productVarian == null)
            {
                return Json(new { success = false, message = "Product does not exist!" });
            }

            // 🔥 Check stock quantity before adding to cart
            if (obj.quantity > productVarian.Stock)
            {
                return Json(new { success = false, message = $"Quantity exceeds stock! Only {productVarian.Stock} items left." });
            }

            // 🛒 Send request to add to cart if valid
            string apiUrl = $"https://localhost:5555/Gateway/UsersService/AddCart";
            var requestData = new
            {
                UserID = user.Id,
                ProductId = obj.ProductID,
                Quantity = obj.quantity
            };

            try
            {
                var jsonContent = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Added to cart successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Quantity exceeds available stock!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error.", error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> CartPart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return PartialView("_Cart", new List<CartViewModels>()); // Trả về giỏ hàng trống
            }

            string apiUrl = $"https://localhost:5555/Gateway/UsersService/ViewCartDetail/{user.Id}";
            List<CartViewModels> cartItems = new List<CartViewModels>();

            try
            {
                var response = await client.GetAsync(apiUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    return PartialView("_Cart", new List<CartViewModels>());
                }

                var mes = await response.Content.ReadAsStringAsync();
                cartItems = JsonSerializer.Deserialize<List<CartViewModels>>(mes, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                return PartialView("_Cart", cartItems ?? new List<CartViewModels>());
            }
            catch (Exception ex)
            {
                return PartialView("_Cart", new List<CartViewModels>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> Comment(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string api = $"https://localhost:5555/Gateway/ProductsService/GetAllComment/{id}";
            List<CommentViewModels> comment = new List<CommentViewModels>();
            try
            {
                var respone = await client.GetAsync(api);
                if (respone.IsSuccessStatusCode)
                {
                    return View(comment);
                }
                var mes = await respone.Content.ReadAsStringAsync();
                comment = JsonSerializer.Deserialize<List<CommentViewModels>>(mes, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
                return View(comment);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }

        }
      /*  [HttpGet]
        public async Task<IActionResult> FilterPrice(decimal? minPrice = null, decimal? maxPrice = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            {
                return BadRequest("Giá tối thiểu không thể lớn hơn giá tối đa");
            }

            string api = $"https://localhost:5555/Gateway/ProductsService/GetAllFillter?minPrice={minPrice}&maxPrice={maxPrice}";
            List<ProductViewModel> product = new List<ProductViewModel>();

            try
            {
                var response = await client.GetAsync(api);
                if (response.IsSuccessStatusCode)
                {
                    var mes = await response.Content.ReadAsStringAsync();
                    product = JsonSerializer.Deserialize<List<ProductViewModel>>(mes, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    ViewBag.MinPrice = minPrice ?? 10000;
                    ViewBag.MaxPrice = maxPrice ?? 2000000;
                    return View(product);
                }
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }*/

    }


}

