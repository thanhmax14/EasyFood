using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.ViewModels;
using System.Runtime.InteropServices;

namespace AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementSellerController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public ManagementSellerController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var list = new List<UsersViewModel>();
                var obj = _userManager.Users.ToList();

                if (obj.Any())
                {
                    foreach (var user in obj)
                    {
                        // Kiểm tra nếu là Admin thì bỏ qua
                        if (_userManager.IsInRoleAsync(user, "Admin").Result)
                            continue;

                        list.Add(new UsersViewModel
                        {
                            Birthday = user.Birthday,
                            Address = user.Address,
                            img = user.img,
                            RequestSeller = user.RequestSeller,
                            isUpdateProfile = user.isUpdateProfile,
                            ModifyUpdate = user.ModifyUpdate,
                            PhoneNumber = user.PhoneNumber,
                            UserName = user.UserName,
                            Email = user.Email,
                            IsBanByadmin = user.IsBanByadmin,
                        });
                    }
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users", error = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                var userModel = new UsersViewModel
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    ModifyUpdate = user.ModifyUpdate,
                    isUpdateProfile = user.isUpdateProfile,
                    RequestSeller = user.RequestSeller,
                    img = user.img,
                    Address = user.Address,
                    Birthday = user.Birthday,
                };
                return Ok(userModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user", error = ex.Message });
            }
        }

        [HttpPost("Accept-seller/{id}")]
        public async Task<IActionResult> AcceptSeller(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                // Cập nhật trạng thái RequestSeller
                user.RequestSeller = "2";
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Failed to update user" });
                }

                // Thêm user vào role (chú ý không nên dùng ID role, nên dùng tên role)
                var roleResult = await _userManager.AddToRoleAsync(user, "Seller");

                if (!roleResult.Succeeded)
                {
                    return BadRequest(new { message = "Failed to update role" });
                }

                return Ok(new { message = "User approved as seller" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }
        }
        [HttpPost("Reject-seller/{id}")]
        public async Task<IActionResult> Reject(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }
                if (await _userManager.IsInRoleAsync(user, "Seller"))
                {
                    var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, "Seller");
                    if (!removeRoleResult.Succeeded)
                    {
                        return BadRequest(new { message = "Failed to remove user from Seller role" });
                    }
                }
                user.RequestSeller = "3";
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Failed to update user" });
                }
                return Ok(new { message = "User reject as seller" });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }
        }
        [HttpPost("Admin-Hiden/{email}")]
        public async Task<IActionResult> Hiden(string email, [FromBody] UsersViewModel obj)
        {
            try
            {
                if (obj == null || string.IsNullOrWhiteSpace(obj.Email))
                {
                    return BadRequest(new { message = "Invalid request, email is required" });
                }

                var user = await _userManager.FindByEmailAsync(obj.Email); // Tối ưu hơn FirstOrDefault()
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                user.IsBanByadmin = true; // Cập nhật theo yêu cầu từ client (ẩn hoặc bỏ ẩn)
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Failed to update user", errors = result.Errors });
                }

                return Ok(new { success = true, message = "User status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }
        }

        [HttpPost("Admin-Show/{email}")]
        public async Task<IActionResult> Show(string email, [FromBody] UsersViewModel obj)
        {
            try
            {
                if (obj == null || string.IsNullOrWhiteSpace(obj.Email))
                {
                    return BadRequest(new { message = "Invalid request, email is required" });
                }
                var user = _userManager.Users.FirstOrDefault(x => x.Email == email);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }
                user.IsBanByadmin = false;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Failed to Show users" });
                }
                return Ok(new { message = "User show success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }

        }
        [HttpPut("Admin-Update{email}")]
        public async Task<IActionResult> updateByAdmin(string email, [FromBody] AdminViewModel obj)
        {
            try
            {
                if (obj == null)
                {
                    return BadRequest(new { message = "User not found" });

                }
                var user = await _userManager.FindByEmailAsync(email);
                user.UserName = obj.UserName;
                user.Email = obj.Email;
                user.Address = obj.Address;
                user.PhoneNumber = obj.PhoneNumber;
                user.Birthday = obj.Birthday;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "User can not update" });
                }
                return Ok(new { message = "Update user success" });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }

        }
    }
}
