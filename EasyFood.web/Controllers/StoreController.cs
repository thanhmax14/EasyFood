using AutoMapper;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;

namespace EasyFood.web.Controllers
{
    public class StoreController : Controller
    {
        private readonly IStoreDetailService _storeService;
        private readonly UserManager<AppUser> _userManager;

        private readonly IMapper _mapper;

        public StoreController(IStoreDetailService storeService, UserManager<AppUser> userManager, IMapper mapper)
        {
            _storeService = storeService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoreViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                var storeEntity = _mapper.Map<StoreDetails>(model); // Map ViewModel thành Model
                await _storeService.AddStoreAsync(storeEntity, user.Id);

                //return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
