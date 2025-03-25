using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using Repository.BalanceChange;

namespace SellerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueController : ControllerBase
    {
        private readonly IBalanceChangeService _balance;
        private readonly IProductService _product;
        public readonly ICartService _cart;
        public readonly IProductVariantService _productWariant;
        public readonly UserManager<AppUser> _userManager;
        private readonly ManageTransaction _managetrans;
        private readonly IOrderDetailService _orderDetail;
        private readonly IOrdersServices _order;
        private readonly EasyFoodDbContext _dbContext;

        public RevenueController(IBalanceChangeService balance, IProductService product, ICartService cart, IProductVariantService productWariant, UserManager<AppUser> userManager, ManageTransaction managetrans, IOrderDetailService orderDetail, IOrdersServices order, EasyFoodDbContext dbContext)
        {
            _balance = balance;
            _product = product;
            _cart = cart;
            _productWariant = productWariant;
            _userManager = userManager;
            _managetrans = managetrans;
            _orderDetail = orderDetail;
            _order = order;
            _dbContext = dbContext;
        }

        [HttpGet("GetOrderStatistics")]
        public async Task<IActionResult> GetOrderStatistics(string sellerId)
        {
            var now = DateTime.UtcNow;
            var startDate = now.AddDays(-30);


            var store = await _dbContext.StoreDetails
                .Where(s => s.UserID == sellerId)
                .Select(s => s.ID)
                .FirstOrDefaultAsync();

            if (store == null)
                return NotFound("Store not found");

            var productIds = await _dbContext.Products
                .Where(p => p.StoreID == store)
                .Select(p => p.ID)
                .ToListAsync();

            var orderDetails = await _dbContext.OrderDetails
                .Where(od => productIds.Contains(od.ProductID) && od.Order.CreatedDate >= startDate)
                .GroupBy(od => od.Order.CreatedDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Orders = g.Select(od => od.OrderID).Distinct().Count(),
                    Earnings = g.Where(od => od.Order.Status == "Success") // Lọc đơn hàng thành công
                                .Sum(od => od.ProductPrice * od.Quantity),
                    FailedOrders = g.Count(od => od.Order.Status == "Failed")
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var statistics = Enumerable.Range(0, 31)
                .Select(i =>
                {
                    var currentDate = startDate.AddDays(i).Date;
                    var data = orderDetails.FirstOrDefault(x => x.Date == currentDate);

                    return new
                    {
                        Date = currentDate.ToString("yyyy-MM-dd"),
                        Orders = data?.Orders ?? 0,
                        Earnings = data?.Earnings ?? 0,
                        FailedOrders = data?.FailedOrders ?? 0
                    };
                })
                .ToList();

            return Ok(statistics);
        }
    }
}
