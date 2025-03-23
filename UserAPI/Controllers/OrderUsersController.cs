using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.BalanceChange;
using Repository.ViewModels;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderUsersController : ControllerBase
    {
        private readonly IBalanceChangeService _balance;
        private readonly IProductService _product;
        public readonly ICartService _cart;
        public readonly IProductVariantService _productWariant;
        public readonly UserManager<AppUser> _userManager;
        private readonly ManageTransaction _managetrans;
        private readonly IOrderDetailService _orderDetail;
        private readonly IOrdersServices _order;

        public OrderUsersController(IBalanceChangeService balance, IProductService product, ICartService cart, IProductVariantService productWariant, UserManager<AppUser> userManager, ManageTransaction managetrans, IOrderDetailService orderDetail, IOrdersServices order)
        {
            _balance = balance;
            _product = product;
            _cart = cart;
            _productWariant = productWariant;
            _userManager = userManager;
            _managetrans = managetrans;
            _orderDetail = orderDetail;
            _order = order;
        }

        [HttpGet]
       public async Task<IActionResult>GetOrderUser(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            if (user == null)
            {
                return Unauthorized(new ErroMess { msg = "Bạn chưa đăng nhập!!" });
            }
            var listOrder = new List<OrderViewModel>();
            var order = await _order.ListAsync(x => x.UserID == userID && x.IsActive);

            if (order.Any())
            {
                foreach(var item in order)
                {
                    listOrder.Add(new OrderViewModel {
                    Address = user.Address,
                    Email = user.Email,
                    Name = user.FirstName +" "+ user.lastAssces,                
                    OrderDate= item.CreatedDate,
                    PaymentMethod = item.MethodPayment,
                    Status = item.Status,
                    Total = item.TotalsPrice
                    });

                }
            }
             return Ok(listOrder);
        }
    }
}
