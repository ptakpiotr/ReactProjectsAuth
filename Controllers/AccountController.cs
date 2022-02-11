using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReactProjectsAuthApi.Data;
using ReactProjectsAuthApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReactProjectsAuthApi.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ChatDbContext _chatContext;
        private readonly IEmailSender _sender;

        public AccountController(ILogger<AccountController> logger, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IMapper mapper,IConfiguration config,
            ChatDbContext chatContext,HangfireJobs jobs,IEmailSender sender)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _config = config;
            _chatContext = chatContext;
            _sender = sender;
            jobs.RemoveChats();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<IdentityUser>(model);
                user.UserName = model.Email;
                var res = await _userManager.CreateAsync(user, model.Password);

                if (res.Succeeded)
                {
                    bool rolesExist = await _roleManager.RoleExistsAsync("User");

                    if (!rolesExist)
                    {
                        var adminRole = new IdentityRole { Name = "Admin" };
                        var userRole = new IdentityRole { Name = "User" };
                        await _roleManager.CreateAsync(adminRole);
                        await _roleManager.CreateAsync(userRole);
                    }

                    await _userManager.AddToRoleAsync(user, "User");

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string redirUrl = Url.Action("ConfirmEmail", "Account", new { email = model.Email, token }, Request.Scheme);

                    _chatContext.Relations.Add(new RelationshipsModel
                    {
                        UserEmail = user.Email,
                        Friends = $"{user.Email}, "
                    });
                    await _chatContext.SaveChangesAsync();
                    await _sender.SendEmailAsync(model.Email,"Email confirmation",redirUrl);
                    return Ok();

                }
            }

            return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user is null)
                {
                    return NotFound();
                }
                if (user.EmailConfirmed)
                {
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        List<Claim> claims = new();
                        claims.Add(new Claim(ClaimTypes.Name,user.UserName));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(ClaimTypes.Email, user.Email));


                        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
                        var tokenDescriptor = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims,
                            expires: DateTime.Now.AddMinutes(30), signingCredentials: credentials);
                        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor) });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }

            return BadRequest();
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return NotFound();
            }

            var res = await _userManager.ConfirmEmailAsync(user, token);

            if (res.Succeeded)
            {
                return Ok(new
                {
                    Message = "Email confirmed succesfully"
                });
            }


            return BadRequest();
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is null)
                {
                    return NotFound();
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                string redirUrl = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

                return (Ok(new CodeModel
                {
                    Token = redirUrl
                }));
            }

            return BadRequest();
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(string email, string token, ResetPasswordModel model)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token) || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return NotFound();
            }

            var res = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (res.Succeeded)
            {
                return Ok(new
                {
                    Message = "Password changed succesfully"
                });
            }


            return BadRequest();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user is null)
                {
                    return NotFound();
                }

                var res = await _userManager.DeleteAsync(user);

                if (res.Succeeded)
                {
                    return NoContent();
                }
            }

            return BadRequest();
        }
    }
}
