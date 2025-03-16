using BusinessLogic.Hash;
using BusinessLogic.Services.BalanceChanges;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Net.payOS;
using Net.payOS.Types;
using Repository.BalanceChange;
using Repository.ViewModels;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IBalanceChangeService _balance;
        private readonly PayOS _payos;
        private readonly UserManager<AppUser> _userManager;
        private readonly ManageTransaction _managetrans;
        public WalletController(IBalanceChangeService balance, PayOS payos, UserManager<AppUser> userManager, ManageTransaction managetrans)
        {
            _balance = balance;
            _payos = payos;
            _userManager = userManager;
            _managetrans = managetrans;
        }

        [HttpGet("{userID}")]
        public async Task<IActionResult> GetWalletByUser(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
            {
                return BadRequest(new ErroMess { msg = "Vui lòng nhập userID" });
            }

            try
            {
             var getUser = await this._userManager.FindByIdAsync(userID);
                if (getUser == null)
                    return BadRequest(new ErroMess { msg = "Người dùng không tồn tại trong hệ thống" });

                return Ok( await this._balance.GetBalance(getUser.Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { msg= ex.Message });
            }
        }
        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreatePayment([FromBody] DepositViewModel model)
        {
           
            if (model.number < 100000)
            {
                return BadRequest(new ErroMess {msg= "Nạp tối thiểu 100,000 VND" });
            }
            else
            {
              
                try
                {
                    var getUser = await this._userManager.FindByIdAsync(model.UserID);
                    if (getUser == null)
                        return BadRequest(new ErroMess { msg = "Người dùng không tồn tại trong hệ thống" });
                    var tien = await this._balance.GetBalance(getUser.Id);


                    int orderCode = RandomCode.GenerateOrderCode();
                    var check = await this._balance.FindAsync(u=> u.orderCode ==orderCode);
                    while (check != null)
                    {
                        orderCode = RandomCode.GenerateOrderCode();
                        check = await this._balance.FindAsync(u => u.orderCode == orderCode);
                    }
                    long expirationTimestamp = DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds();

                    ItemData item = new ItemData($"Thực hiện nạp tiền vào tài khoản {getUser.UserName}:", 1, int.Parse(model.number + ""));
                    List<ItemData> items = new List<ItemData> { item };
                    PaymentData paymentData = new PaymentData(orderCode, int.Parse(model.number + ""), "", items, $"{model.CalleURL}/{orderCode}",
                       $"{model.ReturnUrl}/{orderCode}"
                    , null, null, null, null, null, expirationTimestamp
                       );
                    CreatePaymentResult createPayment = await this._payos.createPaymentLink(paymentData);
                    var url = $"https://pay.payos.vn/web/{createPayment.paymentLinkId}/";
                    await this._managetrans.ExecuteInTransactionAsync(async () =>
                    {

                        var temDongTien = new BalanceChange
                        {
                            MoneyBeforeChange = tien,
                            MoneyChange = model.number,
                            MoneyAfterChange = 0m,
                            Description = $"{url}",
                            Status = "progressing",
                            Method = "deposit",
                            orderCode = orderCode,
                            StartTime = DateTime.Now,
                            UserID = getUser.Id,
   
                        };

                        await this._balance.AddAsync(temDongTien);

                    });
                    await this._balance.SaveChangesAsync();

                    return Ok(new ErroMess { success=true, msg=$"{url}"}); ;
                }
                catch (System.Exception exception)
                {
                    Console.WriteLine(exception);
                    return BadRequest(new ErroMess { msg= "Đã xảy ra lỗi vui lòng thử lại hoặc nhắn tin với Admin" });
                }
            }
        }
     
        [HttpPost("webhook-url")]
        public async Task<IActionResult> ReceivePaymentAsync([FromBody] WebhookType webhook)
        {
            try
            {
                WebhookData data = _payos.verifyPaymentWebhookData(webhook);
                if (data != null && webhook.success)
                {
                    var getBalance = await this._balance.FindAsync(u => u.orderCode == data.orderCode && u.IsComplele == false);
  
                    if (getBalance != null)
                    {
                        await this._managetrans.ExecuteInTransactionAsync(async () =>
                        {
                            var url = getBalance.Description;
                                    var user = await this._userManager.FindByIdAsync(getBalance.UserID);
                                    if (user != null)
                                    {
                                        var tongtien = await _balance.GetBalance(user.Id) + data.amount;
                                        getBalance.Description = $"Thực hiện nạp tiền vào tài khoản,[{url}]";
                                        getBalance.Status = "done";
                                        getBalance.MoneyBeforeChange = await _balance.GetBalance(user.Id);
                                        getBalance.MoneyAfterChange = tongtien;
                                        getBalance.MoneyChange =data.amount;
                                        getBalance.DisPlay = true;
                                        getBalance.IsComplele = true;
                                        getBalance.DueTime= DateTime.Now;
                                    }
                            await _balance.UpdateAsync(getBalance);
                        });
                        await _balance.SaveChangesAsync();

                        return Ok(new { success = true });
                    }
                    return Ok(false);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống" });
            }

        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("confirm-webhook")]
        public IActionResult ConfirmWebhook([FromBody] WebhookType webhook)
        {
            try
            {
                WebhookData data = _payos.verifyPaymentWebhookData(webhook);
                if (data != null && webhook.success)

                    return Ok(true);
                else
                    return Ok(false);
            }
            catch (System.Exception exception)
            {

                Console.WriteLine(exception);
                return Ok(false);
            }

        }
    }
}
