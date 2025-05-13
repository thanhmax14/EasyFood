using BusinessLogic.Hash;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Net.payOS;
using Net.payOS.Types;
using Repository.BalanceChange;
using Repository.ViewModels;

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
        private readonly IOrdersServices _ordersServices;
        private readonly IOrderDetailService _detail;
        public WalletController(IBalanceChangeService balance, PayOS payos, UserManager<AppUser> userManager, ManageTransaction managetrans, IOrdersServices ordersServices, IOrderDetailService detail)
        {
            _balance = balance;
            _payos = payos;
            _userManager = userManager;
            _managetrans = managetrans;
            _ordersServices = ordersServices;
            _detail = detail;
        }

        [HttpGet("{userID}")]
        public async Task<IActionResult> GetBalanceByUser(string userID)
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

                return Ok(await this._balance.GetBalance(getUser.Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { msg = ex.Message });
            }
        }
        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreatePayment([FromBody] DepositViewModel model)
        {

            if (model.number < 100000)
            {
                return BadRequest(new ErroMess { msg = "Nạp tối thiểu 100,000 VND" });
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
                    var check = await this._balance.FindAsync(u => u.OrderCode == orderCode);
                    while (check != null)
                    {
                        orderCode = RandomCode.GenerateOrderCode();
                        check = await this._balance.FindAsync(u => u.OrderCode == orderCode);
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
                        var statime = DateTime.Now;
                        var temDongTien = new BalanceChange
                        {
                            MoneyBeforeChange = tien,
                            MoneyChange = model.number,
                            MoneyAfterChange = 0m,
                            Description = $"{url}",
                            Status = "PROCESSING",
                            Method = "Deposit",
                            OrderCode = orderCode,
                            StartTime = statime,
                            DueTime = statime,
                            CheckDone = false,

                            UserID = getUser.Id,

                        };

                        await this._balance.AddAsync(temDongTien);

                    });
                    await this._balance.SaveChangesAsync();

                    return Ok(new ErroMess { success = true, msg = $"{url}" }); ;
                }
                catch (System.Exception exception)
                {
                    Console.WriteLine(exception);
                    return BadRequest(new ErroMess { msg = "Đã xảy ra lỗi vui lòng thử lại hoặc nhắn tin với Admin" });
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
                    var getBalance = await this._balance.FindAsync(u => u.OrderCode == data.orderCode && u.IsComplete == false);

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
                                getBalance.Status = "Success";
                                getBalance.MoneyBeforeChange = await _balance.GetBalance(user.Id);
                                getBalance.MoneyAfterChange = tongtien;
                                getBalance.MoneyChange = data.amount;
                                getBalance.Display = true;
                                getBalance.IsComplete = true;
                                getBalance.DueTime = DateTime.Now;
                                getBalance.CheckDone = true;
                            }
                            await _balance.UpdateAsync(getBalance);
                        });
                        await _balance.SaveChangesAsync();

                        return Ok(new { success = true });
                    }
                    else
                    {
                        var order = await this._ordersServices.FindAsync(u => u.OrderCode == data.orderCode+"");
                        if (order != null)
                        {
                            order.Status = "PROCESSING";
                            order.PaymentStatus= "Success";
                            order.IsActive = true;
                            order.ModifiedDate = DateTime.Now;
                            await this._ordersServices.UpdateAsync(order);  
                            await this._ordersServices.SaveChangesAsync();
                           var getOrderDetil = await this._detail.ListAsync(_detail => _detail.OrderID == order.ID);
                            foreach (var item in getOrderDetil)
                            {
                                item.Status = "PROCESSING";
                                item.IsActive = true;
                                item.ModifiedDate =DateTime.Now;
                                await this._detail.UpdateAsync(item);
                                await this._detail.SaveChangesAsync();
                            }
                            return Ok(new { success = true });
                        }
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
                return BadRequest(new ErroMess { msg = ex.Message });
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

        [HttpGet("GetWallet/{userid}")]
        public async Task<IActionResult> GetWallet(string userid)
        {
            var list = new List<BalanceListViewModels>();
            if (string.IsNullOrWhiteSpace(userid))
            {
                return BadRequest(new ErroMess { msg = "Vui lòng nhập userID" });
            }

            try
            {
                var getUser = await this._userManager.FindByIdAsync(userid);
                if (getUser == null)
                    return BadRequest(new ErroMess { msg = "Người dùng không tồn tại trong hệ thống" });
                else
                {
                    var getListBalance = await this._balance.ListAsync(
                   u => u.Display && getUser.Id == u.UserID,
                   orderBy: x => x.OrderByDescending(query => query.DueTime.HasValue)  // Ưu tiên bản ghi có DueTime
                                   .ThenByDescending(query => query.DueTime)           // Sắp xếp giảm dần theo DueTime
                                   .ThenByDescending(query => query.StartTime)         // Nếu DueTime = NULL, dùng StartTime
               );


                    if (getListBalance.Any())
                    {
                        var count = 0;
                        foreach (var item in getListBalance)
                        {
                            count++;
                            var getInvoce = RegexAll.ExtractPayosLink(item.Description);
                            if (getInvoce == null)
                                getInvoce = item.Description;
                            list.Add(new BalanceListViewModels
                            {
                                No = count,
                                After = item.MoneyAfterChange,
                                Before = item.MoneyBeforeChange,
                                Change = item.MoneyChange,
                                Date = item.DueTime ?? DateTime.Now,
                                Invoice = getInvoce,
                                Status = item.Status,
                                Types = item.Method
                            });
                        }
                    }
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { msg = ex.Message });
            }
        }

        /*    [ApiExplorerSettings(IgnoreApi = true)]*/
        [HttpPost("WithdrawPayment")]
        public async Task<IActionResult> WithdrawPayment([FromBody] WithdrawViewModels model)
        {
            if (model.amount < 500000)
                return BadRequest(new ErroMess { msg = "Rút tối thiểu 500,000 VND" });
            if (model.amount % 1 != 0)
                return BadRequest(new ErroMess { msg = "Số tiền phải là số nguyên, không được có phần thập phân." });
            var checkUser = await _userManager.FindByIdAsync(model.UserID);
            if (checkUser == null)
                return Unauthorized(new ErroMess { msg = "Người dùng không tồn tại!." });

            if (await _balance.CheckMoney(model.UserID, model.amount))
            {
                var getbalance = await this._balance.GetBalance(checkUser.Id);
                var statime = DateTime.Now;
                var temDongTien = new BalanceChange
                {
                    MoneyBeforeChange = getbalance,
                    MoneyChange = -model.amount,
                    MoneyAfterChange = (getbalance - model.amount),
                    Description = $"{model.AccountName}&{model.accountNumber}&{model.BankName}&{model.amount}",
                    Status = "PROCESSING",
                    Method = "Withdraw",
                    StartTime = statime,
                    DueTime = statime,
                    UserID = checkUser.Id,
                    CheckDone = true,
                };
                try
                {
                    await this._balance.AddAsync(temDongTien);
                    await this._balance.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest(new ErroMess { msg = "Đã xảy ra lỗi, hãy thử lại hoặc liên hệ admin!!" });
                }
            }
            else
            {
                return BadRequest(new ErroMess { msg = "Số dư của bạn không đủ!" });
            }

            return Ok(new ErroMess { success = true, msg = "Thành Công" });
        }

    }
}
