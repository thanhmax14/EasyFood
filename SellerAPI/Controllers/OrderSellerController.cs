using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Repository.BalanceChange;
using Repository.ViewModels;

namespace SellerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderSellerController : ControllerBase
    {
        private readonly IBalanceChangeService _balance;
        private readonly IProductService _product;
        public readonly ICartService _cart;
        public readonly IProductVariantService _productWariant;
        public readonly UserManager<AppUser> _userManager;
        private readonly ManageTransaction _managetrans;
        private readonly IOrderDetailService _orderDetail;
        private readonly IOrdersServices _order;
        private readonly IStoreDetailService _storedetail;

        public OrderSellerController(IBalanceChangeService balance, IProductService product, ICartService cart, IProductVariantService productWariant, UserManager<AppUser> userManager, ManageTransaction managetrans, IOrderDetailService orderDetail, IOrdersServices order, IStoreDetailService storedetail)
        {
            _balance = balance;
            _product = product;
            _cart = cart;
            _productWariant = productWariant;
            _userManager = userManager;
            _managetrans = managetrans;
            _orderDetail = orderDetail;
            _order = order;
            _storedetail = storedetail;
        }



        [HttpPost("GetOrderSeller")]
        public async Task<IActionResult> GetOrderSeller([FromBody]string sellerId)
        {
            var getStore = await this._storedetail.FindAsync(u => u.UserID == sellerId);
            if (getStore == null)
                return NotFound("Store not found");
            var list = new  List<GetSellerOrder>();

            var product = await this._product.ListAsync(u => u.StoreID == getStore.ID);
            if (product.Any())
            {
                var ggetOrderDetail = await this._orderDetail.ListAsync(u => product.Select(p => p.ID).Contains(u.ProductID));

                if (ggetOrderDetail.Any())
                {
                    var getOrder = await this._order.ListAsync(u => ggetOrderDetail.Select(p => p.OrderID).Contains(u.ID),orderBy: x => x.OrderByDescending(q=> q.CreatedDate));
                    if (getOrder.Any())
                    {
                        var userIds = getOrder.Select(o => o.UserID).Distinct();
                        var users = await _userManager.Users
                            .Where(u => userIds.Contains(u.Id))
                            .ToDictionaryAsync(u => u.Id, u => u.UserName);
                        list = getOrder.Select((u, index) => new GetSellerOrder
                        {
                            STT = index + 1,
                            OrderID = u.ID,
                            UserName = users.ContainsKey(u.UserID) ? users[u.UserID] : "Unknown", // Lấy username
                            OrderDate = u.CreatedDate,
                            quantity = ggetOrderDetail.Where(p => p.OrderID == u.ID).Sum(p => p.Quantity),
                            total = u.TotalsPrice,
                            status = u.Status,
                            shortID = u.ID.ToString().Substring(0, 5)
                        }).ToList();
                        return Ok(list);
                    }
                }
            }
            return Ok(false);
        }
    }
}
