    using BusinessLogic.Hash;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Repository.ViewModels;
    using System.Security.Claims;

    namespace AuthService.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class AuthController : ControllerBase
        {
            private readonly SignInManager<AppUser> _signInManager;
            private readonly UserManager<AppUser> _userManager;
            private readonly IEmailSender _emailSender;

            public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IEmailSender emailSender)
            {
                _signInManager = signInManager;
                _userManager = userManager;
                _emailSender = emailSender;
            }
            [HttpPost("login")]
            public async Task<IActionResult> Login(string username, string password)
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return BadRequest(new { status = "error", msg = "Tên Người Dùng Không Được Để Trống" });
                }
                if (string.IsNullOrEmpty(password))
                {
                    return Unauthorized(new { status = "error", msg = "Mật Khẩu Không Được Để Trống" });
                }
                // Xử lý ReturnUrl tương tự GET
          
                var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
                if (user == null)
                {
                    return Unauthorized(new { status = "error", msg = "Tài khoản không tồn tại" });
                }
                if (user.IsBanByadmin)
                {
                    return Unauthorized(new { status = "error", msg = "Tài khoản của bạn đã bị khóa bởi quản trị viên." });
                }
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return Unauthorized(new { status = "error", msg = "Bạn phải xác thực email trước khi đăng nhập." });
                }
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
                if (!isPasswordValid)
                {
                    await _userManager.AccessFailedAsync(user);
                    var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        return Unauthorized(new { status = "error", msg = "Tài khoản của bạn đã bị khóa do quá nhiều lần đăng nhập thất bại." });
                    }
                    else
                    {
                        return Unauthorized(new { status = "error", msg = $"Sai mật khẩu! Bạn còn {5 - failedAttempts} lần thử." });
                    }
                }

                await _userManager.ResetAccessFailedCountAsync(user);
                var result = await _signInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: true);
                if (result.IsLockedOut)
                {
                    return Unauthorized(new { status = "error", msg = "Tài khoản của bạn đã bị khóa." });
                }
                var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
                if (isTwoFactorEnabled)
                {
                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Authenticator");
                    return Unauthorized(new { status = "verify", msg = "Nhập mã xác minh từ ứng dụng xác thực.", token = token });
                }
                if (result.Succeeded)
                {             
                        return Ok(new
                        {
                            status = "success",
                            msg = "Đăng nhập thành công"
                       
                        });
                }
                return Unauthorized(new { status = "error", msg = "Thông tin đăng nhập không chính xác!" });
            }

            [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    if (ModelState.ContainsKey("Username") && ModelState["Username"].Errors.Any())
                    {
                        return BadRequest(new { status = "error", msg = "" + ModelState["Username"].Errors[0].ErrorMessage });
                    }

                    if (ModelState.ContainsKey("Email") && ModelState["Email"].Errors.Any())
                    {
                        return BadRequest(new { status = "error", msg = "" + ModelState["Email"].Errors[0].ErrorMessage });
                    }

                    if (ModelState.ContainsKey("Password") && ModelState["Password"].Errors.Any())
                    {
                        return BadRequest(new { status = "error", msg = "" + ModelState["Password"].Errors[0].ErrorMessage });
                    }

                    if (ModelState.ContainsKey("repassword") && ModelState["repassword"].Errors.Any())
                    {
                        return BadRequest(new { status = "error", msg = "" + ModelState["repassword"].Errors[0].ErrorMessage });
                    }

                    return BadRequest(new { status = "error", msg = "Dữ liệu không hợp lệ" });
                }
                var existingUser = await _userManager.FindByNameAsync(model.Username);
                if (existingUser != null)
                {
                    return BadRequest(new { status = "error", msg = "Tên người dùng đã tồn tại. Vui lòng chọn tên khác." });
                }
                var existingEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    return BadRequest(new { status = "error", msg = "Email đã được đăng ký. Vui lòng sử dụng email khác." });
                }
                var user = new AppUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Home",
                        new { userId = user.Id, token = token }, Request.Scheme);

                    await _emailSender.SendEmailAsync(user.Email, "Xác nhận email",
                        $"Vui lòng nhấp vào liên kết để xác nhận email: {confirmationLink}");

                    return Ok(new { status = "success", msg = "Đăng ký thành công." });

                }
                var firstResultError = result.Errors.FirstOrDefault()?.Description;
                return BadRequest(new { status = "error", msg = firstResultError ?? "Đăng ký thất bại." });


            }

        [HttpGet]
        public async Task<IActionResult> hello()
        {
            return Ok("ok roi");
        }
        }
    }
