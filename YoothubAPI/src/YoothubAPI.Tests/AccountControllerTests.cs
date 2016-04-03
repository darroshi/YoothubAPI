using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using YoothubAPI.Controllers.Account;
using YoothubAPI.Models;

namespace YoothubAPI.Tests
{
    public class AccountControllerTests
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpContext _contextMock;

        public AccountControllerTests()
        {
            // Setup Identity with InMemoryStore
            var services = new ServiceCollection();

            services.AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<ApplicationDbContext>();
            services.AddInstance<ILoggerFactory>(new LoggerFactory())
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Mock HttpContext
            _contextMock = Substitute.For<HttpContext>();
            var response = Substitute.For<HttpResponse>();
            _contextMock.Response.Returns(response);
            var user = Substitute.For<ClaimsPrincipal>();
            user.GetUserId().Returns(null as string);
            _contextMock.Response.Returns(response);
            _contextMock.User.Returns(user);
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Returns(_contextMock);
            services.AddInstance(contextAccessor);

            // Mock ILoggerFactory
            _loggerFactory = Substitute.For<ILoggerFactory>();

            var provider = services.BuildServiceProvider();

            // test account controller
            _userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            _signInManager = provider.GetRequiredService<SignInManager<ApplicationUser>>();
        }

        [Fact]
        public async Task GetExternalAuthenticationSchemesTest()
        {
            using (var controller = new AccountController(_userManager, _signInManager, _loggerFactory))
            {
                var result = await controller.GetExternalAuthenticationSchemes();
                Assert.IsType<JsonResult>(result);
            }

        }
    }
}
