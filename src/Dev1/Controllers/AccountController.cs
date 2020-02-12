using AutoMapper;
using MyCompany.Models;
using MyCompany.Models.Vm;
using MyCompany.Service;
using MyCompany.Service.Dto;
using MyCompany.Web.Extensions;
using MyCompany.Web.Filters;
using MyCompany.Web.Rest.Problems;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MyCompany.Controllers {
    [Route("api")]
    [ApiController]
    public class AccountController : ControllerBase {
        private readonly ILogger<AccountController> _log;
        private readonly IMailService _mailService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _userMapper;
        private readonly IUserService _userService;

        public AccountController(ILogger<AccountController> log, UserManager<User> userManager, IUserService userService,
            IMapper userMapper, IMailService mailService)
        {
            _log = log;
            _userManager = userManager;
            _userService = userService;
            _userMapper = userMapper;
            _mailService = mailService;
        }

        [HttpPost("register")]
        [ValidateModel]
        public async Task<IActionResult> RegisterAccount([FromBody] ManagedUserVM managedUserVm)
        {
            if (!CheckPasswordLength(managedUserVm.Password)) throw new InvalidPasswordException();

            var user = await _userService.RegisterUser(managedUserVm, managedUserVm.Password);
            await _mailService.SendActivationEmail(user);
            return CreatedAtAction(nameof(GetAccount), user);
        }

        [HttpGet("activate")]
        [ValidateModel]
        public async Task ActivateAccount([FromQuery(Name = "key")] string key)
        {
            var user = await _userService.ActivateRegistration(key);
            if (user == null) throw new InternalServerErrorException("Not user was found for this activation key");
        }

        [HttpGet("authenticate")]
        public string IsAuthenticated()
        {
            _log.LogDebug("REST request to check if the current user is authenticated");
            return _userManager.GetUserName(User);
        }

        [HttpGet("account")]
        public async Task<ActionResult<UserDto>> GetAccount()
        {
            var user = await _userService.GetUserWithUserRoles();
            if (user == null) throw new InternalServerErrorException("User could not be found");

            return Ok(new UserDto(user));
        }

        [HttpPost("account")]
        [ValidateModel]
        public async Task<ActionResult> SaveAccount([FromBody] UserDto userDto)
        {
            var userName = _userManager.GetUserName(User);
            if (userName == null) throw new InternalServerErrorException("Current user login not found");

            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null &&
                !string.Equals(existingUser.Login, userName, StringComparison.InvariantCultureIgnoreCase))
                throw new EmailAlreadyUsedException();

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) throw new InternalServerErrorException("User could not be found");

            await _userService.UpdateUser(userDto.FirstName, userDto.LastName, userDto.Email, userDto.LangKey,
                userDto.ImageUrl);
            return Ok();
        }

        [HttpPost("account/change-password")]
        [ValidateModel]
        public async Task<ActionResult> ChangePassword([FromBody] PasswordChangeDto passwordChangeDto)
        {
            if (!CheckPasswordLength(passwordChangeDto.NewPassword)) throw new InvalidPasswordException();

            await _userService.ChangePassword(passwordChangeDto.CurrentPassword, passwordChangeDto.NewPassword);
            return Ok();
        }

        [HttpPost("account/reset-password/init")]
        public async Task<ActionResult> RequestPasswordReset()
        {
            var mail = await Request.BodyAsStringAsync();
            var user = await _userService.RequestPasswordReset(mail);
            if (user == null) throw new EmailNotFoundException();

            await _mailService.SendPasswordResetMail(user);
            return Ok();
        }

        [HttpPost("account/reset-password/finish")]
        [ValidateModel]
        public async Task RequestPasswordReset([FromBody] KeyAndPasswordVM keyAndPasswordVm)
        {
            if (!CheckPasswordLength(keyAndPasswordVm.NewPassword)) throw new InvalidPasswordException();

            var user = await _userService.CompletePasswordReset(keyAndPasswordVm.NewPassword, keyAndPasswordVm.Key);

            if (user == null) throw new InternalServerErrorException("No user was found for this reset key");
        }

        private static bool CheckPasswordLength(string password)
        {
            return !string.IsNullOrEmpty(password) &&
                   password.Length >= ManagedUserVM.PasswordMinLength &&
                   password.Length <= ManagedUserVM.PasswordMaxLength;
        }
    }
}
