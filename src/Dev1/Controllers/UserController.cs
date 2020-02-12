using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Pagination;
using JHipsterNet.Pagination.Extensions;
using MyCompany.Models;
using MyCompany.Security;
using MyCompany.Service;
using MyCompany.Service.Dto;
using MyCompany.Web.Extensions;
using MyCompany.Web.Filters;
using MyCompany.Web.Rest.Problems;
using MyCompany.Web.Rest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyCompany.Controllers {
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly ILogger<UserJwtController> _log;
        private readonly IMailService _mailService;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public UserController(ILogger<UserJwtController> log, UserManager<User> userManager, IUserService userService,
            IMailService mailService)
        {
            _log = log;
            _userManager = userManager;
            _userService = userService;
            _mailService = mailService;
        }

        [HttpPost("users")]
        [ValidateModel]
        public async Task<ActionResult<User>> CreateUser([FromBody] UserDto userDto)
        {
            _log.LogDebug($"REST request to save User : {userDto}");
            if (userDto.Id != null)
                throw new BadRequestAlertException("A new user cannot already have an ID", "userManagement",
                    "idexists");
            // Lowercase the user login before comparing with database
            if (await _userManager.FindByNameAsync(userDto.Login.ToLowerInvariant()) != null)
                throw new LoginAlreadyUsedException();
            if (await _userManager.FindByEmailAsync(userDto.Email.ToLowerInvariant()) != null)
                throw new EmailAlreadyUsedException();

            var newUser = await _userService.CreateUser(userDto);
            await _mailService.SendCreationEmail(newUser);
            return CreatedAtAction(nameof(GetUser), new {login = newUser.Login}, newUser)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert("userManagement.created", newUser.Login));
        }

        [HttpPut("users")]
        [ValidateModel]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            _log.LogDebug($"REST request to update User : {userDto}");
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null && !existingUser.Id.Equals(userDto.Id)) throw new EmailAlreadyUsedException();
            existingUser = await _userManager.FindByNameAsync(userDto.Login);
            if (existingUser != null && !existingUser.Id.Equals(userDto.Id)) throw new LoginAlreadyUsedException();

            var updatedUser = await _userService.UpdateUser(userDto);

            return ActionResultUtil.WrapOrNotFound(updatedUser)
                .WithHeaders(HeaderUtil.CreateAlert("userManagement.updated", userDto.Login));
        }

        [HttpGet("users")]
        public ActionResult<IEnumerable<User>> GetAllUsers(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Users");
            var page = _userManager.Users.UsePageable(pageable);
            var headers = PaginationUtil.GeneratePaginationHttpHeaders(page, HttpContext.Request);
            return Ok(page.Content).WithHeaders(headers);
        }

        [HttpGet("users/authorities")]
        [Authorize(Roles = RolesConstants.ADMIN)]
        public ActionResult<IEnumerable<string>> GetAuthorities()
        {
            return Ok(_userService.GetAuthorities());
        }

        [HttpGet("users/{login}")]
        public async Task<IActionResult> GetUser([FromRoute] string login)
        {
            _log.LogDebug($"REST request to get User : {login}");
            var result = await _userManager.FindByNameAsync(login);
            //TODO adopt a more idiomatic syntax with LINQ Select/Where cf. cancelation token
            var userDto = result != null ? new UserDto(result) : null;
            return ActionResultUtil.WrapOrNotFound(userDto);
        }

        [HttpDelete("users/{login}")]
        [Authorize(Roles = RolesConstants.ADMIN)]
        public async Task<IActionResult> DeleteUser([FromRoute] string login)
        {
            _log.LogDebug($"REST request to delete User : {login}");
            await _userService.DeleteUser(login);
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert("userManagement.deleted", login));
        }
    }
}
