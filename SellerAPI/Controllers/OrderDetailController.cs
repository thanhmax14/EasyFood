using BusinessLogic.Services.OrderDetailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SellerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet("GetOrderDetailsByOrderIdAsync/{storeId}")]
        public async Task<IActionResult> GetOrderDetailsByOrderIdAsync(Guid storeId)
        {
            var orderDetails = await _orderDetailService.GetOrderDetailsByOrderIdAsync(storeId);
            if (orderDetails == null || !orderDetails.Any())
            {
                return NotFound("No order details found for the given store.");
            }
            return Ok(orderDetails);
        }
    }
}
