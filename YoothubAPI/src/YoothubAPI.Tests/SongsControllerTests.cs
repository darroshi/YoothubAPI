using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using YoothubAPI.Controllers.Songs;
using YoothubAPI.Models;
using YoothubAPI.Services;

namespace YoothubAPI.Tests
{
    public class SongsControllerTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILoggerFactory _loggerFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IYoutubeService _youtubeService;
        private readonly HttpContext _contextMock;

        public SongsControllerTests()
        {
            // Setup Identity with InMemoryStore
            var services = new ServiceCollection();

            services.AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase());
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

            _youtubeService = Substitute.For<IYoutubeService>();

            var provider = services.BuildServiceProvider();

            // test account controller
            _userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            _dbContext = provider.GetRequiredService<ApplicationDbContext>();
        }

        [Fact]
        public async void GetSongsTest()
        {
            using (var controller = new SongsController(_youtubeService, _userManager, _loggerFactory, _dbContext))
            {
                throw new Exception();
                controller.ActionContext = new ActionContext { HttpContext = _contextMock };
                var result = await controller.Get(null, null);
                Assert.IsType<JsonResult>(result);
            }
        }
    }
}
