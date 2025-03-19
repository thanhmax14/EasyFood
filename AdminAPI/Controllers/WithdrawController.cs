using BusinessLogic.Services.BalanceChanges;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;

namespace AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IBalanceChangeService _balance;

        public WithdrawController(UserManager<AppUser> userManager, IBalanceChangeService balance)
        {
            _userManager = userManager;
            _balance = balance;
        }

        [HttpGet("GetWithdraw")]
        public async Task<IActionResult> GetWithdraw()
        {
            // Lấy danh sách giao dịch có Method = "Withdraw"
            var balances = await _balance.ListAsync(p => p.Method == "Withdraw");

            if (!balances.Any())
            {
                return NotFound(new { message = "Không có giao dịch rút tiền nào!" });
            }

            var result = new List<WithdrawAdminListViewModel>();
            var count = 0;
            foreach (var balance in balances)
            {
                count++;
                // Lấy thông tin user từ UserManager
                var user = await _userManager.FindByIdAsync(balance.UserID);

                // Tạo ViewModel
                var withdrawModel = new WithdrawAdminListViewModel
                {
                    No = count,
                    ID = balance.ID,
                    MoneyChange = balance.MoneyChange,
                    StartTime = balance.StartTime,
                    DueTime = balance.DueTime,
                    Description = balance.Description,
                    UserID = balance.UserID,
                    Status = balance.Status,
                    Method = balance.Method,
                    UserName = user?.UserName

                };

                result.Add(withdrawModel);
            }

            return Ok(result);
        }
        [HttpPut("AcceptWithdraw/{id}")]
        public async Task<IActionResult> AcceptWithdraw(Guid id, [FromBody] WithdrawAdminListViewModel obj)
        {
            if (obj == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            try
            {
                var withdraw = await _balance.FindAsync(w => w.ID == id);
                if (withdraw == null)
                {
                    return NotFound(new { success = false, msg = "Không tìm thấy yêu cầu rút tiền." });
                }

                // ✅ Cập nhật trạng thái và thời gian xử lý
                withdraw.Status = "Success";
                withdraw.DueTime = DateTime.UtcNow;

                // ✅ Lưu thay đổi vào DB
                await _balance.UpdateAsync(withdraw);
                await _balance.SaveChangesAsync();

                return Ok(new { success = true, msg = "Xác nhận rút tiền thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, msg = "Lỗi: " + ex.Message });
            }
        }
    }
}
