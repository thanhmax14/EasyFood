using BusinessLogic.Services.BalanceChanges;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.BalanceChange;
using Repository.ViewModels;

namespace AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IBalanceChangeService _balance;
        private readonly ManageTransaction _managetrans;
        public WithdrawController(UserManager<AppUser> userManager, IBalanceChangeService balance, ManageTransaction managetrans)
        {
            _userManager = userManager;
            _balance = balance;
            _managetrans = managetrans;
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

            foreach (var balance in balances)
            {
                // Lấy thông tin user từ UserManager
                var user = await _userManager.FindByIdAsync(balance.UserID);

                // Tạo ViewModel
                var withdrawModel = new WithdrawAdminListViewModel
                {
                    ID = balance.ID,
                    MoneyChange = Math.Abs(balance.MoneyChange),
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

            // Sắp xếp theo điều kiện: PROCESSING trước, sau đó theo ngày từ cũ đến mới
            result = result.OrderBy(w => w.Status != "PROCESSING") // Đưa PROCESSING lên đầu
                           .ThenBy(w => w.StartTime) // Nếu là PROCESSING, sắp xếp theo StartTime từ cũ đến mới
                           .ToList();

            // Cập nhật lại số thứ tự (No) theo thứ tự sau khi sắp xếp
            for (int i = 0; i < result.Count; i++)
            {
                result[i].No = i + 1; // Đánh số thứ tự từ 1
            }

            return Ok(result);
        }


        [HttpGet("AcceptWithdraw/{id}")]
        public async Task<IActionResult> AcceptWithdraw(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid guidId))
                return BadRequest(new ErroMess { msg = "ID không hợp lệ!" });
            var flag = await _balance.FindAsync(p => p.ID == guidId);
            if (flag == null)
                return NotFound(new ErroMess { msg = "Không tìm thấy yêu cầu rút tiền!" });

            var user = await _userManager.FindByIdAsync(flag.UserID);
            if (user == null)
                return NotFound(new ErroMess { msg = "Không tìm thấy user!" });

            try
            {
                // ✅ Cập nhật trạng thái và thời gian xử lý
                flag.Status = "Success";
                //withdraw.DueTime = DateTime.Now;

                // ✅ Lưu thay đổi vào DB
                await _balance.UpdateAsync(flag);
                await _balance.SaveChangesAsync();
                return Ok(new { success = true, msg = "Xác nhận rút tiền thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { msg = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpGet("RejectWithdraw/{id}")]
        public async Task<IActionResult> RejectWithdraw(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid guidId))
                return BadRequest(new ErroMess { msg = "ID không hợp lệ!" });
            var flag = await _balance.FindAsync(p => p.ID == guidId);
            if (flag == null)
                return NotFound(new ErroMess { msg = "Không tìm thấy yêu cầu rút tiền!" });

            var user = await _userManager.FindByIdAsync(flag.UserID);
            if (user == null)
                return NotFound(new ErroMess { msg = "Không tìm thấy user!" });

            try
            {
                var result = await _managetrans.ExecuteInTransactionAsync(async () =>
                {
                    var tongtien = await _balance.GetBalance(user.Id) + Math.Abs(flag.MoneyChange);


                    flag.Status = "CANCELLED";
                    flag.MoneyBeforeChange = await _balance.GetBalance(user.Id);
                    flag.MoneyAfterChange = tongtien;
                    flag.DisPlay = true;
                    flag.IsComplele = true;
                    flag.DueTime = DateTime.Now;
                    flag.checkDone = true;

                    await _balance.UpdateAsync(flag);
                });
                if (!result)
                {
                    return StatusCode(500, new ErroMess { msg = "Hủy rút tiền thất bại, giao dịch không hoàn thành!" });
                }

                await _balance.SaveChangesAsync();
                return Ok(new { success = true, msg = "Xác nhận từ chối rút tiền thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { msg = "Lỗi hệ thống: " + ex.Message });
            }
        }



    }
}
