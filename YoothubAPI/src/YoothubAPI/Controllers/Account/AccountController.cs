using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using YoothubAPI.Models;

namespace YoothubAPI.Controllers.Account
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        // GET: /Account/GetExternalAuthenticationSchemes
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<AuthenticationDescription>))]
        public async Task<IActionResult> GetExternalAuthenticationSchemes()
        {
            var schemes = await Task.Run(() => _signInManager.GetExternalAuthenticationSchemes().ToList());
            return Json(schemes);
        }

        // GET: /Account/GetLoggedUserInfo
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ApplicationUser))]
        public async Task<IActionResult> GetLoggedUserInfo()
        {
            var currentUser = await _userManager.FindByIdAsync(User.GetUserId());
            return Json(currentUser);
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin([FromForm]string provider, [FromForm]string returnUrl = null)
        {
            if (provider == null) return new BadRequestObjectResult("Bad input data format.");

            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return new HttpStatusCodeResult(403);
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return Redirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return new HttpStatusCodeResult(403);
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                var email = info.ExternalPrincipal.FindFirstValue(ClaimTypes.Email);
                var displayName = $"{info.ExternalPrincipal.FindFirstValue(ClaimTypes.GivenName)} {info.ExternalPrincipal.FindFirstValue(ClaimTypes.Surname)}";
                var user = new ApplicationUser { UserName = email, Email = email, DisplayName = displayName };
                var resultUser = await _userManager.CreateAsync(user);
                if (resultUser.Succeeded)
                {
                    resultUser = await _userManager.AddLoginAsync(user, info);
                    if (resultUser.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return Redirect(returnUrl);
                    }
                }
            }
            return new HttpStatusCodeResult(403);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return new HttpStatusCodeResult(200);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return new BadRequestResult();
            }
        }
    }
}
