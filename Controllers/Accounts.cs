using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Webapi.shared;
using Webapi.shared.DTOs;

namespace Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Accounts : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<Accounts> _logger;

        public Accounts(UserManager<ApplicationUser> userManager, ILogger<Accounts> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByNameAsync(model.Username) != null)
                return BadRequest("Username already exists.");

            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest("Email already exists.");

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                EmailConfirmed = true, // Logic is still needs work
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = model.PhoneNumber == null ? false:true, // Logic is still needs work
                PhoneNumber = model.PhoneNumber == null ? model.PhoneNumber : "NaN" // Logic is still needs work
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Username or password cannot be empty");
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecureKeyHere123!@#SDMASANSDSDADFSAEJ"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://webapi",
                audience: "https://website",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAccounts()
        {
            _logger.LogInformation("Getting all accounts");
            var accounts = await _userManager.Users
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                })
                .ToListAsync();

            return accounts == null ? NotFound() : Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetAccount(string id)
        {
            _logger.LogInformation("Getting account with ID: {id}", id);
            var account = await _userManager.FindByIdAsync(id);
            if (account == null)
            {
                _logger.LogWarning("Account with ID: {id} not found", id);
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = account.Id,
                UserName = account.UserName,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber
            };

            return Ok(userDto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAccount([FromBody] ManageUser model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state.");
                return BadRequest(new { message = "Invalid request: Model validation failed." });
            }

            var user = await _userManager.FindByIdAsync(model.Account.Id);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            user.UserName = model.Account.UserName;
            user.Email = model.Account.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return BadRequest(new { message = $"Failed to update account: {errors}" });
            }

            if (!string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.NewPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                    return BadRequest(new { message = $"Password change failed: {errors}" });
                }
            }

            return Ok(new { message = "Account updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Account with ID: {id} not found for deletion", id);
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to delete account." });
            }

            _logger.LogInformation("Account with ID: {id} deleted", id);
            return NoContent();
        }

        private bool AccountExists(string id)
        {
            return _userManager.Users.Any(e => e.Id == id);
        }
    }

    // DTO Classes
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;""'<>,.?/|\\-]).{8,}$", ErrorMessage = "Password must contain at least one letter and one number.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; } // Added phone number to the model
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ManageUser
    {
        public UserDto? Account { get; set; }
        public string? Password { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? Token { get; set; }

        public IEnumerable<orderDto>? OrdersACC { get; set; }
    }

    public class UserDto // DTO for returning user information
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
