using AutoMapper;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.StoreDetails;

namespace EasyFood.web.Controllers
{
    [Authorize]
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

            if (stores == null || stores.Count == 0)
            {
                TempData["Message"] = "No inactive stores found.";
            }

            return View(stores);
        }

        public async Task<IActionResult> ViewStoreRegistrationRequests()
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

            return RedirectToAction("ViewStoreRegistrationRequests");
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

            return RedirectToAction("ViewStoreRegistrationRequests");
        }

        [HttpPost]
        [Route("AdminStore/UpdateStoreIsActive")]
        public async Task<JsonResult> UpdateStoreIsActive(Guid storeId, bool isActive)
        {
            bool isUpdated = await _storeService.UpdateStoreIsActiveAsync(storeId, isActive);

            if (!isUpdated)
            {
                return Json(new { success = false, message = "Store not found" });
            }

            return Json(new { success = true, message = "Store status updated successfully", isActive });
        }

    }
}
