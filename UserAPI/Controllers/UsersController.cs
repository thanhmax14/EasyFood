﻿using AutoMapper;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Repository.ViewModels;
using System.Runtime.InteropServices;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStoreDetailService _storeDetailService;
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly ICartService _cartService;
        private readonly IProductImageService _productImageService;


        public UsersController(UserManager<AppUser> userManager, IStoreDetailService storeDetailService, IProductService productService, IProductVariantService productVariantService, ICartService cartService, IProductImageService productImageService)
        {
            _userManager = userManager;
            _storeDetailService = storeDetailService;
            _productService = productService;
            _productVariantService = productVariantService;
            _cartService = cartService;
            _productImageService = productImageService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = new List<UsersViewModel>();
            var obj = _userManager.Users.ToList();
            if (obj.Any())
            {
                foreach (var user in obj)
                {

                    list.Add(new UsersViewModel
                    {
                        Birthday = user.Birthday,
                        Address = user.Address,
                        img = user.ImageUrl,
                        RequestSeller = user.RequestSeller,
                        isUpdateProfile = user.IsProfileUpdated,
                        ModifyUpdate = user.ModifyUpdate,
                        PhoneNumber = user.PhoneNumber,
                        UserName = user.UserName,
                        Email = user.Email,
                    });

                };

            }

            return Ok(list);
        }
        [HttpGet("View-Profile/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest(new { message = "User ID không hợp lệ." });
                }

                var stores = await _storeDetailService.ListAsync(x => x.UserID == id);
                var storeId = stores?.FirstOrDefault()?.ID ?? Guid.Empty; // Kiểm tra null

                var UserModel = new UsersViewModel
                {
                    Birthday = user.Birthday,
                    Address = user.Address,
                    img = user.ImageUrl,
                    RequestSeller = user.RequestSeller,
                    isUpdateProfile = user.IsProfileUpdated,
                    ModifyUpdate = user.ModifyUpdate,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName,
                    Email = user.Email,
                    StoreDeatilId = storeId
                };

                return Ok(UserModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] IndexUserViewModels obj)
        {
            if (obj.userView == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy user." });
            }
            if(!string.IsNullOrEmpty(obj.userView.PhoneNumber))
            {
                var existPhone = await _userManager.Users.Where(x => x.PhoneNumber == obj.userView.PhoneNumber && x.Id != id).FirstOrDefaultAsync();

                if (existPhone != null)
                {
                    return BadRequest(new { success = false, message = "Số điện thoại đã được sử dụng bởi người dùng khác." });
                }
            }

            // Cập nhật thông tin
            user.Birthday = obj.userView.Birthday;
            user.Address = obj.userView.Address;
            user.PhoneNumber = obj.userView.PhoneNumber;
            user.UserName = obj.userView.UserName;
            user.Email = obj.userView.Email;
            /*   _mapper.Map(obj, user);*/

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "Cập nhật user thành công." });
            }

            return BadRequest(new { message = "Cập nhật user thất bại.", errors = result.Errors });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy user." });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "Xóa user thành công." });
            }

            return BadRequest(new { message = "Xóa user thất bại.", errors = result.Errors });
        }
        [HttpPost("register-seller/{id}")]
        public async Task<IActionResult> RegisterSeller(string id)
        {
            // Tìm user theo Id
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy user." });
            }
            // Kiểm tra rỗng Address, PhoneNumber, Birthday
            if (string.IsNullOrEmpty(user.Address) ||
                string.IsNullOrEmpty(user.PhoneNumber) ||
                user.Birthday == null || user.Birthday == DateTime.MinValue)
            {
                return BadRequest(new { message = "Vui lòng cập nhật đầy đủ thông tin trước khi đăng ký seller." });
            }
            // Cập nhật trạng thái RequestSeller
            user.RequestSeller = "1"; // Đánh dấu yêu cầu đăng ký seller
            user.ModifyUpdate = DateTime.UtcNow; // Cập nhật thời gian sửa đổi

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "Đăng ký seller thành công. Đang chờ phê duyệt." });
            }
            return BadRequest(new { message = "Đăng ký seller thất bại.", errors = result.Errors });
        }
        [HttpGet("ViewCartDetail/{id}")]
        public async Task<IActionResult> ViewCartDetail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var carts = await _cartService.ListAsync(x => x.UserID == user.Id);
            if (carts == null || !carts.Any())
            {
                return NotFound("Giỏ hàng trống.");
            }

            var products = await _productService.ListAsync();
            var productVariantIds = products.Select(x => x.ID).ToList();

            // 🔹 Lọc chỉ lấy các biến thể có IsActive == true
            var productVariants = await _productVariantService.ListAsync(p => productVariantIds.Contains(p.ProductID) && p.IsActive);

            var result = new List<CartViewModels>(); // Danh sách kết quả

            foreach (var cart in carts)
            {
                var product = products.FirstOrDefault(p => p.ID == cart.ProductTypesID);
                if (product == null)
                {
                    continue; // Bỏ qua nếu không có sản phẩm
                }

                // 🔹 Lấy thông tin biến thể sản phẩm, đã được lọc trước đó
                var variant = productVariants.FirstOrDefault(v => v.ProductID == product.ID);
                if (variant == null)
                {
                    continue; // Nếu không có biến thể nào active thì bỏ qua sản phẩm này
                }

                // 🔹 Lấy ảnh sản phẩm
                var productImg = await _productImageService.FindAsync(x => x.ProductID == product.ID);

                var cartItem = new CartViewModels
                {
                    ProductID = cart.ProductTypesID,
                    ProductName = product.Name ?? "Không có tên",
                    quantity = cart.Quantity,
                    price = variant.SellPrice,
                    Subtotal = cart.Quantity * variant.SellPrice,
                    img = productImg?.ImageUrl ?? "/images/default.jpg", // Nếu không có ảnh thì lấy ảnh mặc định
                    Stock = variant.Stock // 🔹 Lấy số lượng trong kho từ ProductVariant
                };

                result.Add(cartItem);
            }

            return Ok(result); // Trả về danh sách giỏ hàng
        }


        [HttpPost("AddCart")]
        public async Task<IActionResult> AddCart([FromBody] CartViewModels obj)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(obj.UserID);
                if (user == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }
                var productVarian = await _productVariantService.FindAsync(x => x.ProductID == obj.ProductID);

                var product = await _productService.FindAsync(x => x.ID == obj.ProductID);

                if (product == null)
                {
                    return BadRequest(new { message = "Product not found" });
                }

                var cartItem = await _cartService.FindAsync(x => x.UserID == user.Id && x.ProductTypesID == obj.ProductID);
                int currentQuantity = cartItem?.Quantity ?? 0;
                int newTotalQuantity = currentQuantity + obj.quantity;

                // 🚨 Kiểm tra nếu tổng số lượng vượt quá stock
                if (newTotalQuantity > productVarian.Stock)
                {
                    return BadRequest(new { message = $"Số lượng vượt quá tồn kho! Chỉ còn {productVarian.Stock} sản phẩm." });
                }
                if (cartItem != null)
                {
                    cartItem.Quantity += obj.quantity;
                    await _cartService.UpdateAsync(cartItem);
                    await _cartService.SaveChangesAsync();
                }
                else
                {
                    var newCart = new Cart
                    {
                        ID = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserID = user.Id,
                        ProductTypesID = obj.ProductID,
                        Quantity = obj.quantity
                    };
                    await _cartService.AddAsync(newCart);
                    await _cartService.SaveChangesAsync();
                }

                return Ok(new { success = true, message = "Add Cart Success!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi server: {ex.Message}" });
            }
        }

        [HttpPost("UpdateCart/{id}")]
        public async Task<IActionResult> UpdateCart([FromBody] CartItem obj)
        {
            var productVarian = await _productVariantService.FindAsync(x => x.ProductID == obj.ProductID);
            var carItem = await _cartService.FindAsync(x => x.ProductTypesID == obj.ProductID);

            if (carItem == null)
            {
                return BadRequest(new { message = "Cart not found" });
            }

            int currentQuantity = carItem.Quantity;

            // 🚨 Nếu đang tăng số lượng thì kiểm tra tồn kho
            if (obj.quantity > currentQuantity && obj.quantity > productVarian.Stock)
            {
                return BadRequest(new { message = $"Số lượng vượt quá tồn kho! Chỉ còn {productVarian.Stock} sản phẩm." });
            }

            // Cập nhật số lượng (tăng hoặc giảm)
            carItem.Quantity = obj.quantity;
            await _cartService.UpdateAsync(carItem);
            await _cartService.SaveChangesAsync();

            return Ok(new { success = true });
        }



        [HttpPost("DeleteCart/{id}")]
        public async Task<IActionResult> DeleteCart(Guid id)
        {
            try
            {
                var cartItem = await _cartService.FindAsync(z => z.ProductTypesID == id);
                if (cartItem == null)
                {
                    return BadRequest(new { message = "ProductId not found" });
                }

                await _cartService.DeleteAsync(cartItem);
                await _cartService.SaveChangesAsync();

                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the product.", error = ex.Message });
            }
        }

    }
}
