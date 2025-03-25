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
using Repository.ViewModels;

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

        [HttpPost("GetOrderStatistics")]
        public async Task<IActionResult> GetOrderStatistics([FromBody] string sellerId)
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
         Orders = g.Select(od => od.OrderID).Count(),
         Earnings = g.Where(od => od.Order.Status == "Success")
                     .Sum(od => (od.ProductPrice * od.Quantity)) - (g.Where(od => od.Order.Status == "Success")
                     .Sum(od => (od.ProductPrice * od.Quantity) * 0.03m)),
         CANCELLED = g.Count(od => od.Order.Status == "CANCELLED"),
         Success = g.Count(od => od.Order.Status == "Success"),
         PROCESSING = g.Count(od => od.Order.Status == "PROCESSING"),
     })
     .OrderBy(x => x.Date)
     .ToListAsync();


            var list = new List<RevenueSeller>();

            foreach (var i in Enumerable.Range(0, 31))
            {
                var currentDate = startDate.AddDays(i).Date;
                var data = orderDetails.FirstOrDefault(x => x.Date == currentDate);

                var revenue = new RevenueSeller
                {
                    CANCELLED = data?.CANCELLED ?? 0,
                    Success = data?.Success ?? 0,
                    Earnings = data?.Earnings ?? 0,
                    Orders = data?.Orders ?? 0,
                    Date = currentDate.ToString("yyyy-MM-dd"),
                    PROCESSING = data?.PROCESSING ?? 0
                };

                list.Add(revenue);
            }

            return Ok(list);

        }

        [HttpPost("GetProductStatistics")]
        public async Task<IActionResult> GetProductStatistics([FromBody] string sellerId)
        {
            var store = await _dbContext.StoreDetails
                .Where(s => s.UserID == sellerId)
                .Select(s => s.ID)
                .FirstOrDefaultAsync();

            if (store == null)
                return NotFound("Store not found");

            var productStats = await _dbContext.Products
     .Where(p => p.StoreID == store)
     .Select(p => new PieProductView
     {
         ProductName = p.Name,
         OrderCount = _dbContext.OrderDetails
                         .Where(od => od.ProductID == p.ID)
                         .Sum(od => od.Quantity)
     })
     .ToListAsync();
            return Ok(productStats);
        }



        [HttpPost("GetRevenueOrderToday")]
        public async Task<IActionResult> GetRevenueOrderToday([FromBody] string sellerId)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var yesterday = today.AddDays(-1);

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

            // Lấy doanh thu theo ngày
            var earningsData = await _dbContext.OrderDetails
                .Where(od => productIds.Contains(od.ProductID) && od.Order.Status == "Success")
                .GroupBy(od => od.Order.CreatedDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Earnings = g.Sum(od => (od.ProductPrice * od.Quantity) * 0.97m) // Trừ 3% phí
                })
                .ToDictionaryAsync(g => g.Date, g => g.Earnings);

            decimal todayEarnings = earningsData.ContainsKey(today) ? earningsData[today] : 0;
            decimal yesterdayEarnings = earningsData.ContainsKey(yesterday) ? earningsData[yesterday] : 0;

            // Tính % thay đổi doanh thu (có thể là số âm)
            decimal earningsChange = (yesterdayEarnings == 0) ? (todayEarnings == 0 ? 0 : -100)
                : ((todayEarnings - yesterdayEarnings) / yesterdayEarnings) * 100;

            // Tổng số đơn hàng hôm nay
            int totalOrdersToday = await _dbContext.OrderDetails.CountAsync(od => productIds.Contains(od.ProductID) && od.Order.CreatedDate.Date == today);

            // Số đơn hàng thành công hôm nay
            int successOrdersToday = await _dbContext.OrderDetails.CountAsync(od => productIds.Contains(od.ProductID) && od.Order.Status == "Success" && od.Order.CreatedDate.Date == today);

            // Số đơn hàng bị hủy/refund hôm nay
            int failedOrdersToday = await _dbContext.OrderDetails.CountAsync(od => productIds.Contains(od.ProductID) && od.Order.Status == "CANCELLED" && od.Order.CreatedDate.Date == today);

            // Tính tỷ lệ hoàn thành đơn hôm nay (có thể âm nếu đơn hủy nhiều hơn đơn thành công)
            decimal completionRate = totalOrdersToday == 0 ? 0 : ((decimal)(successOrdersToday - failedOrdersToday) / totalOrdersToday) * 100;

            var result = new RevenuToday
            {
                Earnings = todayEarnings,
                Orders = totalOrdersToday,
                Refunds = failedOrdersToday,
                EarningsChange = earningsChange,
                CompletionRate = completionRate
            };

            return Ok(result);

        }
    }

    }
