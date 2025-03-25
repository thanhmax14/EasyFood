using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DBContext;
using Models;
using Repository.BalanceChange;
using Microsoft.EntityFrameworkCore;
using Repository.ViewModels;

namespace AdminAPI.Controllers
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


        [HttpGet("GetAdminStatistics")]
        public async Task<IActionResult> GetAdminStatistics()
        {
            // Tính tổng tiền hoa hồng từ đơn thành công (3% của tổng doanh thu seller)
            var totalCommission = await _dbContext.OrderDetails
                .Where(od => od.Order.Status == "Success")
                .SumAsync(od => (od.ProductPrice * od.Quantity) * 0.03m);

            // Đếm tổng số store
            var totalStores = await _dbContext.StoreDetails.CountAsync();

            // Đếm tổng số seller (dựa trên role "Seller")
            var totalSellers = await _userManager.GetUsersInRoleAsync("Seller");

            // Đếm tổng số user
            var totalUsers = await _dbContext.Users.CountAsync();

            return Ok(new GetRevenueTotal
            {
                TotalCommission = totalCommission,
                TotalStores = totalStores,
                TotalSellers = totalSellers.Count,
                TotalUsers = totalUsers
            });
        }

        [HttpGet("GetDailyStatistics")]
        public async Task<IActionResult> GetDailyStatistics()
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-30); // 30 ngày trước
            var days = Enumerable.Range(0, 31)
                .Select(i => startDate.AddDays(i))
                .ToList();

            // Lấy doanh thu từng ngày
            var commissionDict = await _dbContext.OrderDetails
                .Where(od => od.Order.Status == "Success" && od.Order.CreatedDate >= startDate)
                .GroupBy(od => od.Order.CreatedDate.Date)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(od => (od.ProductPrice * od.Quantity) * 0.03m));

            // Lấy số tiền nạp từng ngày
            var depositDict = await _dbContext.BalanceChanges
                .Where(bc => bc.Method == "Deposit" && bc.StartTime >= startDate)
                .GroupBy(bc => bc.StartTime.Value.Date)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(bc => bc.MoneyChange));

            // Lấy số tiền rút từng ngày
            var withdrawDict = await _dbContext.BalanceChanges
                .Where(bc => bc.Method == "Withdraw" && bc.StartTime >= startDate)
                .GroupBy(bc => bc.StartTime.Value.Date)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(bc => bc.MoneyChange));

            // Lấy số user đăng ký từng ngày
            var userDict = await _dbContext.Users
                .Where(u => u.joinin >= startDate)
                .GroupBy(u => u.joinin.Value.Date)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            // Ghép dữ liệu lại theo từng ngày
            var statistics = days.Select(date => new RevenuAdmin
            {
                Date = date.ToString("yyyy-MM-dd"),
                Commission = commissionDict.GetValueOrDefault(date, 0),
                Deposit = depositDict.GetValueOrDefault(date, 0),
                Withdraw = withdrawDict.GetValueOrDefault(date, 0),
                NewUsers = userDict.GetValueOrDefault(date, 0)
            }).ToList();

            return Ok(statistics);
        }


    }
}

