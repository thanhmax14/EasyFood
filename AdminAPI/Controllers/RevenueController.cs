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

            return Ok(new
            {
                TotalCommission = totalCommission,
                TotalStores = totalStores,
                TotalSellers = totalSellers.Count,
                TotalUsers = totalUsers
            });
        }

        [HttpGet("GetMonthlyStatistics")]
        public async Task<IActionResult> GetMonthlyStatistics()
        {
            var now = DateTime.UtcNow;
            var startDate = now.AddDays(-30);

            // Doanh thu nhận được trong 30 ngày (tổng tiền hoa hồng)
            var commissionLast30Days = await _dbContext.OrderDetails
                .Where(od => od.Order.Status == "Success" && od.Order.CreatedDate >= startDate)
                .SumAsync(od => (od.ProductPrice * od.Quantity) * 0.03m);

            // Số store đăng ký trong 30 ngày qua
            var newStoresLast30Days = await _dbContext.StoreDetails
                .CountAsync(s => s.CreatedDate >= startDate);

            // Số user đăng ký trong 30 ngày qua
            var newUsersLast30Days = await _dbContext.Users
                .CountAsync(u => u.joinin >= startDate);

            return Ok(new
            {
                CommissionLast30Days = commissionLast30Days,
                NewStoresLast30Days = newStoresLast30Days,
                NewUsersLast30Days = newUsersLast30Days
            });
        }
    }
}

