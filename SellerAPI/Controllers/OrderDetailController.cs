using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SellerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly IOrdersServices _orderService;

        public OrderDetailController(IOrderDetailService orderDetailService, IOrdersServices orderService)
        {
            _orderDetailService = orderDetailService;
            _orderService = orderService;
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
        [HttpPost("accept/{orderId}")]
        public async Task<IActionResult> AcceptOrder(Guid orderId)
        {
            var result = await _orderService.AcceptOrder(orderId);
            if (!result) return BadRequest("Accept Order Failed.");

            return Ok(new { message = "Order Accepted Successfully" });
        }

        [HttpPost("reject/{orderId}")]
        public async Task<IActionResult> RejectOrder(Guid orderId)
        {
            var result = await _orderService.RejectOrder(orderId);
            if (!result) return BadRequest("Reject Order Failed.");

            return Ok(new { message = "Order Rejected Successfully" });
        }
    }
}
