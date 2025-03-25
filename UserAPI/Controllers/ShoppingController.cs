using BusinessLogic.Hash;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Net.payOS;
using Net.payOS.Types;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly IBalanceChangeService _balance;
        private readonly IProductService _product;
        public readonly ICartService _cart;
        public readonly IProductVariantService _productWariant;
        public readonly UserManager<AppUser > _userManager;
        private readonly ManageTransaction _managetrans;
        private readonly IOrderDetailService _orderDetail;
        private readonly IOrdersServices _order;
        private readonly PayOS _payos;

        public ShoppingController(IBalanceChangeService balance, IProductService product, ICartService cart, IProductVariantService productWariant, UserManager<AppUser> userManager, ManageTransaction managetrans, IOrderDetailService orderDetail, IOrdersServices order, PayOS payos)
        {
            _balance = balance;
            _product = product;
            _cart = cart;
            _productWariant = productWariant;
            _userManager = userManager;
            _managetrans = managetrans;
            _orderDetail = orderDetail;
            _order = order;
            _payos = payos;
        }

        [HttpPost("BuyProduct")]
        public async Task<IActionResult> BuyProduct([FromBody] BuyRequest request)
        {
            if (!request.IsOnline)
            {
                var user = await _userManager.FindByIdAsync(request.UserID);
                if (user == null)
                {
                    return Unauthorized(new ErroMess { msg = "Bạn chưa đăng nhập!!" });
                }

                if (request.Products == null || !request.Products.Any())
                {
                    return BadRequest(new ErroMess { msg = "Vui lòng chọn sản phẩm cần mua!" });
                }

                var temOrderDeyail = new List<OrderDetail>();
                decimal totelPrice = 0;


                foreach (var id in request.Products)
                {
                    var checkcart = await this._cart.FindAsync(u => u.UserID == request.UserID && u.ProductID == id.Key);
                    if (checkcart != null)
                    {
                        var getQuatity = await this._productWariant.FindAsync(u => u.ProductID == id.Key &&u.IsActive);

                        if(getQuatity==null)
                            return NotFound(new ErroMess { msg = "Sản phẩm mua không tồn tại!" });
                        if (checkcart.Quantity < getQuatity.Stock)
                        {
                            totelPrice += getQuatity.Price * id.Value;
                        }
                    }
                }
                var orderID = Guid.NewGuid();
                if (await _balance.CheckMoney(user.Id, totelPrice) == false)
                {
                    return BadRequest(new ErroMess { msg = "Số dư trong tài khoản không đủ để mua hàng!" });
                }
                foreach (var id in request.Products)
                {
                    var product = await _product.GetAsyncById(id.Key);
                    if (product == null)
                    {
                        return NotFound(new ErroMess { msg = "Sản phẩm mua không tồn tại!" });
                    }
                    var checkcart = await this._cart.FindAsync(u => u.UserID == request.UserID && u.ProductID == id.Key);
                    if (checkcart == null)
                    {
                        return NotFound(new ErroMess { msg = "Sản phẩm mua không tồn tại trong giỏ hàng!" });
                    }
                    var getQuatity = await this._productWariant.FindAsync(u => u.ProductID == id.Key);
                    if (checkcart.Quantity > getQuatity.Stock)
                    {
                        return BadRequest(new ErroMess { msg = "Số lượng sản phẩm mua vượt quá số lượng tồn kho!" });
                    }

                    temOrderDeyail.Add(new OrderDetail
                    {
                        ID = Guid.NewGuid(),
                        OrderID = orderID,
                        ProductID = id.Key,
                        Quantity = id.Value,
                        ProductPrice = getQuatity.Price,
                        TotalPrice = getQuatity.Price * id.Value,
                        Status = "Success"
                    });

                }

                var order = new Order
                {
                    ID = orderID,
                    UserID = request.UserID,
                    TotalsPrice = totelPrice,
                    Status = "PROCESSING",
                    CreatedDate = DateTime.Now,
                    MethodPayment = "wallet",
                    StatusPayment = "PROCESSING",
                    Quantity = request.Products.Sum(u => u.Value),
                    OrderCode=""

                };
               var balan =  new BalanceChange
                {
                    UserID = user.Id,
                    MoneyChange = -totelPrice,
                    MoneyBeforeChange = await _balance.GetBalance(user.Id),
                    MoneyAfterChange = await _balance.GetBalance(user.Id) - totelPrice,
                    Method = "Buy",
                    Status = "PROCESSING",
                    DisPlay = true,
                    IsComplele = false,
                    checkDone = true,
                    StartTime = DateTime.Now
                };

                if (await _balance.CheckMoney(user.Id, totelPrice))
                {
                    try
                    {
                        await this._balance.AddAsync(balan);
                        await _order.AddAsync(order);
                        await this._balance.SaveChangesAsync();
                        await this._order.SaveChangesAsync();
                    }
                    catch
                    {
                        order.StatusPayment= "Failed";
                        order.Status= "Failed";
                        balan.Status = "Failed";
                        balan.DueTime =DateTime.Now;
                        balan.MoneyBeforeChange = await _balance.GetBalance(user.Id);
                        balan.MoneyAfterChange = await _balance.GetBalance(user.Id) + totelPrice;
                        balan.MoneyChange = totelPrice;
                        await this._order.SaveChangesAsync();
                        await this._balance.SaveChangesAsync();
                        return BadRequest(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!" });
                    }
                }
                    try
                    {
                    var result = await _managetrans.ExecuteInTransactionAsync(async () =>
                    {
                        foreach (var item in temOrderDeyail)
                        {
                            await _orderDetail.AddAsync(item);
                        }
                    });
                    await this._orderDetail.SaveChangesAsync();
                    if (result)
                    {
                        balan.Status= "Success";
                        order.StatusPayment = "Success";
                        order.Status = "Success";
                        balan.DueTime = DateTime.Now;
                        var productKeys = request.Products; 

                        foreach (var productId in productKeys)
                        {
                            var getCart = await this._cart.FindAsync(u => u.ProductID == productId.Key);

                            if (getCart != null)
                            {
                                await this._cart.DeleteAsync(getCart);
                            }
                            var getWarian = await this._productWariant.FindAsync(u => u.ProductID == productId.Key);
                         getWarian.Stock -= productId.Value;
                           if(getWarian.Stock == 0 ||getWarian.Stock<0)
                            {
                                getWarian.Stock = 0;
                                getWarian.IsActive = false;
                            }
                            await this._productWariant.UpdateAsync(getWarian);
                            await this._productWariant.SaveChangesAsync();
                        }
                        await this._cart.SaveChangesAsync();
                        await this._balance.SaveChangesAsync();
                        await this._order.SaveChangesAsync();
                        return Ok(new ErroMess { success=true,msg= "Đặt hàng thành công!" });
                    }
                    else
                    {
                        return BadRequest(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!" });
                    }
                }
                catch(Exception e)
                {
                    order.StatusPayment = "Failed";
                    order.Status = "Failed";
                    balan.Status = "Failed";
                    balan.DueTime = DateTime.Now;
                    balan.MoneyBeforeChange = await _balance.GetBalance(user.Id);
                    balan.MoneyAfterChange = await _balance.GetBalance(user.Id) + totelPrice;
                    balan.MoneyChange = totelPrice;
                    await this._order.SaveChangesAsync();
                    await this._balance.SaveChangesAsync();

                    return BadRequest(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!" });
                }
            }
            else
            {
                var user = await _userManager.FindByIdAsync(request.UserID);
                if (user == null)
                {
                    return Unauthorized(new ErroMess { msg = "Bạn chưa đăng nhập!!" });
                }

                if (request.Products == null || !request.Products.Any())
                {
                    return BadRequest(new ErroMess { msg = "Vui lòng chọn sản phẩm cần mua!" });
                }

                var temOrderDeyail = new List<OrderDetail>();
                decimal totelPrice = 0;


                foreach (var id in request.Products)
                {
                    var checkcart = await this._cart.FindAsync(u => u.UserID == request.UserID && u.ProductID == id.Key);
                    if (checkcart != null)
                    {
                        var getQuatity = await this._productWariant.FindAsync(u => u.ProductID == id.Key && u.IsActive);

                        if(getQuatity == null)
                        {
                            return NotFound(new ErroMess { msg = "Sản phẩm mua không tồn tại!" });
                        }

                        if (id.Value< getQuatity.Stock)
                        {
                            totelPrice += getQuatity.Price * id.Value;
                        }
                    }
                }
                var orderID = Guid.NewGuid();

                foreach (var id in request.Products)
                {
                    var product = await _product.GetAsyncById(id.Key);
                    if (product == null)
                    {
                        return NotFound(new ErroMess { msg = "Sản phẩm mua không tồn tại!" });
                    }
                   /* var checkcart = await this._cart.FindAsync(u => u.UserID == request.UserID && u.ProductID == id.Key);
                    if (checkcart == null)
                    {
                        return NotFound(new ErroMess { msg = "Sản phẩm mua không tồn tại trong giỏ hàng!" });
                    }*/
                    var getQuatity = await this._productWariant.FindAsync(u => u.ProductID == id.Key);
                    if (id.Value> getQuatity.Stock)
                    {
                        return BadRequest(new ErroMess { msg = "Số lượng sản phẩm mua vượt quá số lượng tồn kho!" });
                    }

                    temOrderDeyail.Add(new OrderDetail
                    {
                        ID = Guid.NewGuid(),
                        OrderID = orderID,
                        ProductID = id.Key,
                        Quantity = id.Value,
                        ProductPrice = getQuatity.Price,
                        TotalPrice = getQuatity.Price * id.Value,
                        Status = "PROCESSING", IsActive =false
                    });

                }
                int orderCode = RandomCode.GenerateOrderCode();
                var check = await this._order.FindAsync(u => u.OrderCode == orderCode + "");
                while (check != null)
                {
                    orderCode = RandomCode.GenerateOrderCode();
                    check = await this._order.FindAsync(u => u.OrderCode == orderCode+"");
                }
                var order = new Order
                {
                    ID = orderID,
                    UserID = request.UserID,
                    TotalsPrice = totelPrice,
                    Status = "PROCESSING",
                    CreatedDate = DateTime.Now,
                    MethodPayment = "wallet",
                    StatusPayment = "PROCESSING",
                    Quantity = request.Products.Sum(u => u.Value),
                    OrderCode = ""+ orderCode

                };
             /*   var balan = new BalanceChange
                {
                    UserID = user.Id,
                    MoneyChange = -totelPrice,
                    MoneyBeforeChange = await _balance.GetBalance(user.Id),
                    MoneyAfterChange = await _balance.GetBalance(user.Id) - totelPrice,
                    Method = "Buy",
                    Status = "PROCESSING",
                    DisPlay = true,
                    IsComplele = false,
                    checkDone = true,
                    StartTime = DateTime.Now
                };*/

             
                    try
                    {
                       // await this._balance.AddAsync(balan);
                        await _order.AddAsync(order);
                        await this._balance.SaveChangesAsync();
                        await this._order.SaveChangesAsync();
                    }
                    catch
                    {
                        order.StatusPayment = "Failed";
                        order.Status = "Failed";
                      /*  balan.Status = "Failed";
                        balan.DueTime = DateTime.Now;
                        balan.MoneyBeforeChange = await _balance.GetBalance(user.Id);
                        balan.MoneyAfterChange = await _balance.GetBalance(user.Id) + totelPrice;
                        balan.MoneyChange = totelPrice;*/
                        await this._order.SaveChangesAsync();
                       // await this._balance.SaveChangesAsync();
                        return BadRequest(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!11" });
                    }
                
                try
                {
                    var url = "";
                    var result = await _managetrans.ExecuteInTransactionAsync(async () =>
                    {
                        foreach (var item1 in temOrderDeyail)
                        {
                            await _orderDetail.AddAsync(item1);
                        }
                        long expirationTimestamp = DateTimeOffset.Now.AddMinutes(5).ToUnixTimeSeconds();
                        ItemData item = new ItemData($"Thực hiện mua hàng ở tài khoản {user.UserName}:", 1, (int)totelPrice);
                        List<ItemData> items = new List<ItemData> { item };
                        PaymentData paymentData = new PaymentData(orderCode, (int)totelPrice, "", items, $"{request.CalledUrl}/{orderCode}",
                           $"{request.SuccessUrl}/{orderCode}"
                        , null, null, null, null, null, expirationTimestamp
                           );
                        CreatePaymentResult createPayment = await this._payos.createPaymentLink(paymentData);
                    url = $"https://pay.payos.vn/web/{createPayment.paymentLinkId}/";                     
                    });
                    await this._orderDetail.SaveChangesAsync();
                    if (result)
                    {
                        var productKeys = request.Products;

                        foreach (var productId in productKeys)
                        {
                            
                            var getWarian = await this._productWariant.FindAsync(u => u.ProductID == productId.Key);
                            getWarian.Stock -= productId.Value;
                            if (getWarian.Stock == 0 || getWarian.Stock < 0)
                            {
                                getWarian.Stock = 0;
                                getWarian.IsActive = false;
                            }
                            await this._productWariant.UpdateAsync(getWarian);
                            await this._productWariant.SaveChangesAsync();
                        }
                        return Ok(new ErroMess { success = true, msg = $"{url}" }); ;
                    }
                    else
                    {
                        return BadRequest(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!22" });
                    }
                }
                catch (Exception e)
                {
                    order.StatusPayment = "Failed";
                    order.Status = "Failed";
                    await this._order.SaveChangesAsync();
                    return BadRequest(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!33" });
                }
            }
        }
    }
}
