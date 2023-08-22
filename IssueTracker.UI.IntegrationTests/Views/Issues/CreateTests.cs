﻿using FluentAssertions;
using IssueTracker.Domain.Constants;
using IssueTracker.Domain.Entities;
using IssueTracker.UI.IntegrationTests;
using IssueTracker.UI.Models.Issues;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace IssueTracker.UI.FunctionalTests.Views.Issues
{
    public class CreateTests : UiTestsFixture
    {
        private Project _project;
        private string URI { get => $"/Projects/{_project.Id}/Issues/Create"; }
        private string FORM_ACTION { get => $"action=\"{URI}\""; }
        public CreateTests() : base()
        {
            AuthenticateFactory(new List<Claim>()
            {
                new Claim(AppClaimTypes.ProjectAccess, "1")
            });
            _project = ProjectHelpers.CreateTestProject("Test Project");
            _project.Id = 1;
            _project.AddToDatabaseAsync(Database)
                .SeedDatabaseWithMembersUsersAsync(Database).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task Get_WhenUserAuthenticated_ShouldReturnViewWithFormToCreateIssue()
        { 
            //Act
            var page = await Client.GetAsync(URI);
            var pageHtml = await page.Content.ReadAsStringAsync();

            //Assert
            page.StatusCode.Should().Be(HttpStatusCode.OK);
            pageHtml.Should().Contain(FORM_ACTION);
        }

        [Fact]
        public async Task Post_WhenEnteredDataIsValid_ShouldAddIssueToDatabase()
        {
            //Arrange
            var model = new CreateIssueViewModel()
            {
                ProjectId = _project.Id,
                Title = nameof(Post_WhenEnteredDataIsValid_ShouldAddIssueToDatabase),
                Description = "Description",
                Priority = Domain.Enums.PriorityLevel.Medium,
                AssignedMembersId = _project.Members.Select(x => x.UserId)
            };

            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, URI, model);

            //Assert
            var addedIssue = Database.Func(ctx => ctx.Issues.Include(x => x.Members).First(x => x.Title == model.Title));
            addedIssue.Should().NotBeNull();
            addedIssue.Title.Should().Be(model.Title);
            addedIssue.Description.Should().Be(model.Description);
            addedIssue.Priority.Should().Be(model.Priority);
            addedIssue.Members.Should().HaveCount(_project.Members.Count);
        }
    }
}
