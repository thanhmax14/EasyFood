﻿using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Models;
using Repository.StoreDetails;
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

        private readonly StoreDetailsRepository _storeRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IProductVariantService _variantService;
        public SellerController(IReviewService reviewService, UserManager<AppUser> userManager, IProductService productService, IStoreDetailService storeDetailService, StoreDetailsRepository storeRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment, IProductVariantService variantService)
        {
            _reviewService = reviewService;
            _userManager = userManager;
            _productService = productService;
            _storeDetailService = storeDetailService;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
            _storeRepository = storeRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _variantService = variantService;
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

        public IActionResult CreateStore()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStore(StoreViewModel model, IFormFile? ImgFile)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                bool isSeller = await _storeRepository.IsUserSellerAsync(user.Id);
                if (!isSeller)
                {
                    ModelState.AddModelError("", "Bạn không có quyền tạo cửa hàng.");
                    return View(model);
                }

                var storeEntity = _mapper.Map<StoreDetails>(model);
                storeEntity.UserID = user.Id;
                storeEntity.Status = "PENDING";
                storeEntity.IsActive = false;
                storeEntity.CreatedDate = DateTime.Now;
                storeEntity.ModifiedDate = null;

                storeEntity.LongDescriptions = model.LongDescriptions?.Trim();
                storeEntity.ShortDescriptions = model.ShortDescriptions?.Trim();
                storeEntity.Address = model.Address?.Trim();
                storeEntity.Phone = model.Phone?.Trim();

                if (ImgFile != null && ImgFile.Length > 0)
                {
                    string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
                    string extension = Path.GetExtension(ImgFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Img", "Chỉ hỗ trợ file ảnh (.png, .jpeg, .jpg)");
                        return View(model);
                    }

                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + extension;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImgFile.CopyToAsync(fileStream);
                    }

                    storeEntity.Img = "/uploads/" + uniqueFileName;
                }

                await _storeDetailService.AddStoreAsync(storeEntity, user.Id);

                // 🟢 Dùng Session thay vì TempData
                HttpContext.Session.SetString("SuccessMessage", "Đăng ký cửa hàng thành công! Vui lòng chờ quản trị viên duyệt.");

                return RedirectToAction("ViewStore"); // Điều hướng sau khi tạo thành công
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ViewStore()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId của người đăng nhập
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            bool isSeller = await _storeRepository.IsUserSellerAsync(userId);
            if (!isSeller)
            {
                TempData["ErrorMessage"] = "Bạn không phải là seller!";
                return RedirectToAction("Index", "Home");
            }

            var stores = await _storeDetailService.GetStoresByUserIdAsync(userId);

            // Kiểm tra nếu có store nhưng đang chờ duyệt
            bool hasPendingStore = stores.Any(s => s.Status == "PENDING");

            ViewBag.HasPendingStore = hasPendingStore; // Truyền thông tin ra View

            return View(stores);
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStore(Guid id, StoreViewModel model, IFormFile? ImgFile)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Lấy thông tin cửa hàng hiện tại để giữ nguyên ảnh cũ nếu không upload ảnh mới
            var existingStore = await _storeDetailService.GetStoreByIdAsync(id);
            if (existingStore == null)
            {
                ModelState.AddModelError("", "Không tìm thấy cửa hàng.");
                return View(model);
            }

            string imgPath = existingStore.Img; // Giữ ảnh cũ nếu không có ảnh mới

            if (ImgFile != null && ImgFile.Length > 0)
            {
                string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
                string extension = Path.GetExtension(ImgFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Img", "Chỉ hỗ trợ file ảnh (.png, .jpeg, .jpg)");
                    return View(model);
                }

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImgFile.CopyToAsync(fileStream);
                }

                imgPath = "/uploads/" + uniqueFileName; // Cập nhật đường dẫn ảnh mới
            }

            // Cập nhật cửa hàng
            var success = await _storeDetailService.UpdateStoreAsync(id, model.Name, model.LongDescriptions,
                                                                model.ShortDescriptions, model.Address,
                                                                model.Phone, imgPath);

            if (!success)
            {
                ModelState.AddModelError("", "Cập nhật cửa hàng thất bại.");
                return View(model);
            }

            return RedirectToAction("ViewStore");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateStore(Guid id)
        {
            var store = await _storeDetailService.GetStoreByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            var model = new StoreViewModel
            {
                Name = store.Name,
                CreatedDate = store.CreatedDate,
                ModifiedDate = DateTime.Now,
                LongDescriptions = store.LongDescriptions,
                ShortDescriptions = store.ShortDescriptions,
                Address = store.Address,
                Phone = store.Phone,
                Img = store.Img // Giữ ảnh cũ
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ClearSuccessMessage()
        {
            HttpContext.Session.Remove("SuccessMessage");
            return Ok();
        }

        //[HttpGet]
        //public IActionResult GetProductsByStore(Guid storeId)
        //{
        //    var products = _productService.GetProductsByStoreId(storeId);
        //    return Json(products); // Đảm bảo trả về JSON hợp lệ
        //}

        //[HttpGet]
        //[Route("Seller/ViewProductList/{storeId}")]
        public IActionResult ViewProductList(Guid storeId)
        {
            var products = _productService.GetProductsByStoreId(storeId);

            ViewBag.StoreId = storeId; // Giữ StoreId để dùng cho nút "Create"

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct(Guid storeId)
        {
            var categories = await _productService.GetCategoriesAsync();

            var model = new ProductViewModel
            {
                StoreID = storeId,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.StoreId = storeId;
                var categories = await _productService.GetCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                }).ToList();

                return View(model);
            }

            await _productService.CreateProductAsync(model);
            //return RedirectToAction("ViewProductList", "Seller");
            return RedirectToAction("ViewProductList", "Seller", new { storeId = model.StoreID });
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(Guid productId)
        {
            var model = await _productService.GetProductByIdAsync(productId);
            if (model == null)
            {
                return NotFound();
            }

            var categories = await _productService.GetCategoriesAsync();
            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.Name
            }).ToList();

            ViewBag.ProductID = productId;
            ViewBag.StoreID = model.StoreID;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(ProductUpdateViewModel model, List<IFormFile> NewImages)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _productService.GetCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                }).ToList();

                ViewBag.StoreID = model.StoreID;
                return View(model);
            }

            string webRootPath = _webHostEnvironment.WebRootPath; // Lấy đường dẫn thư mục wwwroot

            await _productService.UpdateProductAsync(model, NewImages, webRootPath);

            return RedirectToAction("ViewProductList", "Seller", new { storeId = model.StoreID });
        }

        [HttpPost]
        [Route("Seller/ToggleStatus")]
        public async Task<IActionResult> ToggleStatus(Guid productId, bool isActive)
        {
            var success = await _productService.ToggleProductStatus(productId);
            if (!success) return NotFound(new { message = "Product not found" });

            return Ok(new { success = true, message = "Product status updated successfully!" });
        }
        public async Task<IActionResult> ViewProductVariants(Guid productId)
        {
            var variants = await _variantService.GetVariantsByProductIdAsync(productId);
            if (variants.Any())
            {
                ViewBag.StoreId = variants.First().StoreID; // Lấy StoreID từ danh sách variant
            }
            ViewBag.ProductId = productId; // Lưu ProductId để sử dụng trong View
            return View(variants);
        }
        [HttpGet]
        public IActionResult CreateProductVariant(Guid productId)
        {
            var model = new ProductVariantCreateViewModel
            {
                ProductID = productId
            };
            ViewBag.ProductID = productId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductVariant(ProductVariantCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _variantService.CreateProductVariantAsync(model);

            return RedirectToAction("ViewProductVariants", new { productId = model.ProductID });
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProductVariant(Guid variantId)
        {
            var model = await _variantService.GetProductVariantForEditAsync(variantId);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.ProductID = model.ProductID;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProductVariant(ProductVariantEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _variantService.UpdateProductVariantAsync(model);
                if (success)
                {
                    return RedirectToAction("ViewProductVariants", "Seller", new { productId = model.ProductID });
                }
            }
            return View(model);
        }


        public async Task<IActionResult> ReplyFeedback(string id)
        {
            try
            {
                // Kiểm tra id có hợp lệ không
                if (!Guid.TryParse(id, out Guid reviewId))
                {
                    return Json(new { success = false, message = "ID không hợp lệ." });
                }

                // Tìm review theo ReviewId
                var review = await _reviewService.FindAsync(r => r.ID == reviewId);

                if (review == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đánh giá." });
                }

                // Lấy thông tin người dùng
                var user = await _userManager.FindByIdAsync(review.UserID);
                if (user == null)
                {
                    return Json(new { success = false, message = "Người dùng không tồn tại." });
                }

                // Lấy thông tin sản phẩm
                var product = await _productService.GetAsyncById(review.ProductID);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                // Tạo ViewModel để hiển thị trong View
                var reviewModel = new ReivewViewModel
                {
                    ID = review.ID,
                    Username = user.UserName,
                    ProductName = product.Name,
                    Rating = review.Rating,
                    Cmt = review.Cmt,
                    Datecmt = review.Datecmt,
                    Relay = review.Relay,
                    Status = review.Status,
                    UserID = review.UserID,
                    ProductID = review.ProductID
                };

                return View(reviewModel);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để debug sau này
                Console.WriteLine($"Lỗi: {ex.Message}");

                // Trả về lỗi JSON để tránh chết chương trình
                return Json(new { success = false, message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ReplyFeedback(ReivewViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Relay))
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            try
            {

                string apiUrl = $"https://localhost:5555/Gateway/ReviewService/UpdateReply/{model.ID}";

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
                    return Redirect("/Seller/FeedbackList");
                }
                return Json(dataRepone);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi kết nối API Gateway! " + ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Show(string id)
        {
            string apiUrl = $"https://localhost:5555/Gateway/ReviewService/ShowFeedback/{id}";

            if (!Guid.TryParse(id, out Guid guidId))
            {
                return BadRequest(new ErroMess { success = false, msg = "ID không hợp lệ" });
            }

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Không gửi body nếu API không yêu cầu
                var response = await client.PutAsync(apiUrl, null);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var mes = await response.Content.ReadAsStringAsync();
                var dataResponse = JsonSerializer.Deserialize<ErroMess>(mes, options);

                return response.IsSuccessStatusCode ? Json(dataResponse) : Json(dataResponse);
            }
            catch (Exception)
            {
                return StatusCode(500, new ErroMess { success = false, msg = "Lỗi kết nối API Gateway!" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Hidden(string id)
        {
            string apiUrl = $"https://localhost:5555/Gateway/ReviewService/HiddenFeedback/{id}";

            if (!Guid.TryParse(id, out Guid guidId))
            {
                return BadRequest(new ErroMess { success = false, msg = "ID không hợp lệ" });
            }

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Không gửi body nếu API không yêu cầu
                var response = await client.PutAsync(apiUrl, null);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var mes = await response.Content.ReadAsStringAsync();
                var dataResponse = JsonSerializer.Deserialize<ErroMess>(mes, options);

                return response.IsSuccessStatusCode ? Json(dataResponse) : Json(dataResponse);
            }
            catch (Exception)
            {
                return StatusCode(500, new ErroMess { success = false, msg = "Lỗi kết nối API Gateway!" });
            }
        }
    }
}
