using AutoMapper;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.StoreDetails;
using Repository.ViewModels;

namespace EasyFood.web.Controllers
{
    public class AdminStoreController : Controller
    {
        private readonly IStoreDetailService _storeService;
        private readonly StoreDetailsRepository _storeRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminStoreController(IStoreDetailService storeService, UserManager<AppUser> userManager, IMapper mapper, IWebHostEnvironment webHostEnvironment, StoreDetailsRepository storeRepository)
        {
            _storeService = storeService;
            _userManager = userManager;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _storeRepository = storeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var stores = await _storeService.GetInactiveStoresAsync();
            return View(stores);
        }

        [HttpPost]
        public async Task<IActionResult> Hidden(Guid id)
        {
            var result = await _storeService.HideStoreAsync(id);

            if (result)
            {
                TempData["success"] = "Store has been hidden successfully.";
            }
            else
            {
                TempData["fail"] = "Failed to hide store.";
            }

            return RedirectToAction("Index", "AdminStore"); // Chuyển hướng đến /AdminStore/Index
        }

        [HttpPost]
        public async Task<IActionResult> Show(Guid id)
        {
            var result = await _storeService.ShowStoreAsync(id);

            if (result)
            {
                TempData["success"] = "Store has been hidden successfully.";
            }
            else
            {
                TempData["fail"] = "Failed to hide store.";
            }

            return RedirectToAction("Index", "AdminStore"); // Chuyển hướng đến /AdminStore/Index
        }

        public async Task<IActionResult> ViewHiddenStore()
        {
            var stores = await _storeService.GetActiveStoresAsync();
            return View(stores);
        }
    }
}
