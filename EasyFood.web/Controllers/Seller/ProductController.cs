using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository.ViewModels;

namespace EasyFood.web.Controllers.Seller
{

    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? storeId)
        {
            if (storeId == null || storeId == Guid.Empty)
            {
                // Nếu storeId không có, thử lấy storeId từ user
                var userId = User.FindFirst("UserID")?.Value ?? string.Empty;
                storeId = await _productService.GetCurrentStoreIDAsync(userId);

                if (storeId == null || storeId == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "Invalid store ID.";
                    return RedirectToAction("Index");
                }
            }

            await LoadCategoriesAsync();
            ViewBag.StoreID = storeId;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(ProductListViewModel model, List<IFormFile> ImgFiles)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirst("UserID")?.Value ?? string.Empty;
            model.StoreId = model.StoreId == Guid.Empty ? await _productService.GetCurrentStoreIDAsync(userId) : model.StoreId;

            if (model.StoreId == Guid.Empty)
            {
                ModelState.AddModelError("", "Invalid store ID. Please ensure your store is registered.");
                return View(model);
            }

            // Xử lý upload hình ảnh...
            var images = new List<ProductImageViewModel>();
            if (ImgFiles != null && ImgFiles.Count > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (var file in ImgFiles)
                {
                    if (file.Length > 0 && file.Length <= 5 * 1024 * 1024) // Giới hạn 5MB
                    {
                        try
                        {
                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                            var filePath = Path.Combine(uploadPath, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            images.Add(new ProductImageViewModel
                            {
                                ImageUrl = "/uploads/" + fileName,
                                IsMain = images.Count == 0, // Ảnh đầu tiên là ảnh chính
                                FileName = file.FileName,
                                ContentType = file.ContentType
                            });
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"Error uploading {file.FileName}: {ex.Message}");
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", $"File {file.FileName} is too large. Maximum size is 5MB.");
                        return View(model);
                    }
                }
            }

            var result = await _productService.CreateProductAsync(model, userId, images);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to create product. Please try again.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction("Index");
        }

        // Tải danh mục sản phẩm
        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories
                .Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name })
                .ToList();
        }

        private Guid GetCurrentStoreID()
        {
            var storeId = User.FindFirst("StoreID")?.Value;
            return Guid.TryParse(storeId, out var result) ? result : Guid.Empty;
        }

        public async Task<IActionResult> Index(Guid storeId)
        {
            var products = await _productService.GetAllProductsAsync(storeId);
            return View(products);
        }
    }
}
