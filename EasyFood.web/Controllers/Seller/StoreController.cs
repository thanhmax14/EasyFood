using AutoMapper;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.StoreDetails;
using Repository.ViewModels;
using System.IO;

public class StoreController : Controller
{
    private readonly IStoreDetailService _storeService;
    private readonly StoreDetailsRepository _storeRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public StoreController(IStoreDetailService storeService, UserManager<AppUser> userManager, IMapper mapper, IWebHostEnvironment webHostEnvironment, StoreDetailsRepository storeRepository)
    {
        _storeService = storeService;
        _userManager = userManager;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
        _storeRepository = storeRepository;
    }

    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StoreViewModel model, IFormFile? ImgFile)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Kiểm tra xem user có phải là seller không
            bool isSeller = await _storeRepository.IsUserSellerAsync(user.Id);
            if (!isSeller)
            {
                ModelState.AddModelError("", "Bạn không có quyền tạo cửa hàng.");
                return View(model);
            }

            // Ánh xạ dữ liệu từ ViewModel sang Model
            var storeEntity = _mapper.Map<StoreDetails>(model);
            storeEntity.UserID = user.Id;
            storeEntity.Status = "PENDING";
            storeEntity.IsActive = false;
            storeEntity.CreatedDate = DateTime.Now;
            storeEntity.ModifiedDate = null;

            // Gán đầy đủ các thuộc tính
            storeEntity.LongDescriptions = model.LongDescriptions?.Trim();
            storeEntity.ShortDescriptions = model.ShortDescriptions?.Trim();
            storeEntity.Address = model.Address?.Trim();
            storeEntity.Phone = model.Phone?.Trim();

            // Xử lý hình ảnh
            if (ImgFile != null && ImgFile.Length > 0)
            {
                string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
                string extension = Path.GetExtension(ImgFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Img", "Chỉ hỗ trợ file ảnh (.png, .jpeg, .jpg)");
                    return View(model);
                }

                // Kiểm tra thư mục uploads, nếu chưa có thì tạo mới
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

            await _storeService.AddStoreAsync(storeEntity, user.Id);
            return RedirectToAction("Index"); // Điều hướng sau khi tạo thành công
        }
        return View(model);
    }
    public async Task<IActionResult> Index()
    {
        var stores = await _storeService.GetAllStoresAsync();
        return View(stores);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var store = await _storeService.GetStoreByIdAsync(id);
        if (store == null)
        {
            return NotFound();
        }

        var model = new StoreViewModel
        {
            Name = store.Name,
            LongDescriptions = store.LongDescriptions,
            ShortDescriptions = store.ShortDescriptions,
            Address = store.Address,
            Phone = store.Phone,
            Img = store.Img // Giữ ảnh cũ
        };

        return View(model);
    }


    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Guid id, StoreViewModel model, IFormFile? ImgFile)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        // Lấy thông tin cửa hàng hiện tại để giữ nguyên ảnh cũ nếu không upload ảnh mới
        var existingStore = await _storeService.GetStoreByIdAsync(id);
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
        var success = await _storeService.UpdateStoreAsync(id, model.Name, model.LongDescriptions,
                                                            model.ShortDescriptions, model.Address,
                                                            model.Phone, imgPath);

        if (!success)
        {
            ModelState.AddModelError("", "Cập nhật cửa hàng thất bại.");
            return View(model);
        }

        return RedirectToAction("Index");
    }


}
