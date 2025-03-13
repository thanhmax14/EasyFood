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

        public StoreController(StoreDetailService storeDetailService)
        {
            _storeDetailService = storeDetailService;
        }

        [HttpGet]
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
    }
}
