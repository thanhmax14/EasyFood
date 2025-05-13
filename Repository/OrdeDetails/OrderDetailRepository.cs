using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using Repository.BaseRepository;
using Repository.ViewModels;

namespace Repository.OrdeDetails
{
    public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly EasyFoodDbContext _context;
        public OrderDetailRepository(EasyFoodDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<OrderDetailSellerViewModel>> GetOrderDetailsByOrderIdAsync(Guid orderId)
        {
            var orderDetails = await (from od in _context.OrderDetails
                                      join p in _context.Products on od.ProductID equals p.ID
                                      join o in _context.Orders on od.OrderID equals o.ID
                                      join u in _context.Users on o.UserID equals u.Id into userJoin
                                      from u in userJoin.DefaultIfEmpty() // LEFT JOIN Users
                                      join pi in _context.ProductImages.Where(pi => pi.IsMain)
                                          on p.ID equals pi.ProductID into productImages
                                      from pi in productImages.DefaultIfEmpty() // LEFT JOIN ProductImages
                                      where od.IsActive == true
                                            && o.ID == orderId
                                      //&& o.Status == "PROCESSING" // ✅ Thêm điều kiện lọc theo Status
                                      select new OrderDetailSellerViewModel
                                      {
                                          OrderDetailID = od.ID,
                                          ProductPrice = od.ProductPrice,
                                          TotalPrice = od.TotalPrice,
                                          Quantity = od.Quantity,
                                          IsActive = od.IsActive,

                                          ProductID = p.ID,
                                          ProductName = p.Name,

                                          OrderID = o.ID,
                                          Status = o.Status, // Đảm bảo lấy trạng thái từ bảng Orders
                                          UserID = u != null ? u.Id : null,
                                          UserName = u != null ? u.UserName : null,
                                          Email = u != null ? u.Email : null,
                                          PhoneNumber = u != null ? u.PhoneNumber : null,
                                          AvatarUrl = u != null ? u.ImageUrl : null,
                                          Address = u != null ? u.Address : null, // ➕ Lấy địa chỉ của khách hàng

                                          ImageUrl = pi != null ? pi.ImageUrl : null
                                      }).ToListAsync();

            return orderDetails;
        }

        public async Task<List<OrderDetail>> GetByOrderId(Guid orderId)
        {
            var orderDetails = await _context.OrderDetails.Where(od => od.OrderID == orderId).ToListAsync();
            return orderDetails.Any() ? orderDetails : throw new Exception("Order details not found");
        }


    }
}
