using WebApplication1.Models;
using WebApplication1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace AskHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser()
                {
                    UserName = userViewModel.Username,
                    Email = userViewModel.Email
                };

                IdentityResult result = await _userManager.CreateAsync(user, userViewModel.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, userViewModel.RememberMe);
                    return Ok(new { Message = "Registration successful" });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(userViewModel.Email);
                if (user is not null)
                {
                    bool correctPassword = await _userManager.CheckPasswordAsync(user, userViewModel.Password);

                    if (correctPassword)
                    {
                        await _signInManager.SignInAsync(user, userViewModel.RememberMe);
                        var roles = await _userManager.GetRolesAsync(user);
                        return Ok(new { Message = "Login successful" });
                    }
                }

                return Unauthorized(new { Message = "Invalid Email or Password" });
            }

            return BadRequest(ModelState);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "Logged out successfully" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UserUpdateViewModel userViewModel)
        {
            if (User.Identity is not null && User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                var deleteResult = await _userManager.DeleteAsync(user);
                if (!deleteResult.Succeeded)
                {
                    return BadRequest(deleteResult.Errors);
                }

                AppUser newUser = new AppUser()
                {
                    UserName = user.UserName,
                    Email = user.Email
                };

                var createResult = await _userManager.CreateAsync(newUser, userViewModel.Password);
                if (createResult.Succeeded)
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return Ok(new { Message = "User updated successfully" });
                }
                else
                {
                    return BadRequest(createResult.Errors);
                }
            }

            return Unauthorized(new { Message = "Unauthorized access" });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete()
        {
            if (User.Identity is not null && User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    return Ok(new { Message = "Account deleted successfully" });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            return Unauthorized(new { Message = "Unauthorized access" });
        }
    }
}
