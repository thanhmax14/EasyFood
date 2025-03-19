using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.ViewModels;

namespace ProductsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly StoreDetailService _storeDetailService;
        private readonly CategoryService _categoryService;


        public StoreController(StoreDetailService storeDetailService, CategoryService categoryService)
        {
           
            _categoryService = categoryService;
            _storeDetailService = storeDetailService;
        }

        [HttpGet("GetAllStores")]
        public async Task<IActionResult> GetallStore()
        {

            var list = new List<StoreViewModel>();
            var store = await _storeDetailService.ListAsync(s => s.IsActive, orderBy: x => x.OrderByDescending(s => s.CreatedDate));
            if (store.Any())
            {
                foreach (var item in store)
                {
                    list.Add(new StoreViewModel
                    {
                        Name = item.Name,
                        LongDescriptions = item.LongDescriptions,
                        Address = item.Address,
                        Phone = item.Phone,
                        ShortDescriptions = item.ShortDescriptions,
                        CreatedDate = item.CreatedDate,
                        ID = item.ID,
                        Img = item.Img,
                        IsActive = item.IsActive,
                        ModifiedDate = item.ModifiedDate,
                        Status = item.Status,   
                    });
                }
                return Ok(list);
            }
            return BadRequest(false);
        }


        [HttpGet("GetStoreDetail")]

        public async Task<IActionResult> GetStoreDetail(Guid id)
        {
            var store = new StoreDetailsViewModels();

            var storeDetails = await _storeDetailService.FindAsync(s => s.ID == id);
           
            
            
            store.ID = storeDetails.ID;
            store.Name = storeDetails.Name;
            store.Address = storeDetails.Address;
            store.Phone = storeDetails.Phone;
            store.ShortDescriptions = storeDetails.ShortDescriptions;
            store.CreatedDate = storeDetails.CreatedDate;
            store.Img = storeDetails.Img;
            store.LongDescriptions = storeDetails.LongDescriptions;
           
            if (storeDetails == null)
            {
                return BadRequest("Store not Found");
            }
            else { 

            return Ok(store);

              }
        }


    }
}
