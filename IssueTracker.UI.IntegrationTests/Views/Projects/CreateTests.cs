﻿using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.UI.IntegrationTests.Views.Projects
{
    public class CreateTests : UiBaseTest
    {
        public CreateTests(CustomWebApplicationFactory factory)
            :base(factory)
        {

        }

        [Fact]
        public async Task Get_WhenUserInManagerRole_ShouldShowFormToCreateNewProject()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Manager") };
            AuthenticateFactory(claims);
            //Arrange
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);

            //Act
            var page = await client.GetAsync("/");
            var pageHtml = await page.Content.ReadAsStringAsync();

            //Assert
            Assert.Equal(HttpStatusCode.OK, page.StatusCode);
            Assert.Contains("action=\"/Projects\"", pageHtml);
        }

        [Fact]
        public async Task Get_WhenUserIsNotInRole_ShouldNotShowFormToCreateNewProject()
        {
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);

            var page = await client.GetAsync("/");
            var pageHtml = await page.Content.ReadAsStringAsync();

            Assert.DoesNotContain("action=\"/Projects\"", pageHtml);
        }

        [Fact]
        public async Task Post_WhenUserInManagerRole_ShouldAddProjectToDatabase()
        {
            //Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Manager") };
            AuthenticateFactory(claims);
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);
            var model = new { Title = "Test Project" };

            //Act
            var response = await client.SendFormAsync(HttpMethod.Post, "/", "/Projects", model);

            //Assert
            var userId = Factory.Services.GetRequiredService<ICurrentUserService>().UserId;
            var addedProject = Database.Func(ctx => ctx.Projects.Include(x => x.Members).First(x => x.Title == model.Title));
            Assert.Contains(userId, addedProject.Members.Select(x => x.UserId));
        }
    }
}
