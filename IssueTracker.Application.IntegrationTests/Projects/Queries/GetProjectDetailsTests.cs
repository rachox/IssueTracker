﻿using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.Domain.Entities;

namespace IssueTracker.Application.IntegrationTests.Projects.Queries
{
    public class GetProjectDetailsTests : ApplicationBaseTest
    {
        public GetProjectDetailsTests(CustomWebApplicationFactory factory)
            :base(factory)
        {
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsValid_ShouldGetDetailsIncludingMembersAndIssuesWithMembers()
        {
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsValid_ShouldGetDetailsIncludingMembersAndIssuesWithMembers));
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            var query = new GetProjectDetailsQuery { ProjectId = project.Id};
            var result = await Mediator.Send(query);

            Assert.Equal(project.Members.Count, result.Members.Count);
            Assert.Equal(project.Issues.Count, result.Issues.Count);
            Assert.True(result.Issues.All(x => x.Title != String.Empty));
            Assert.True(result.Issues.All(x => x.Members.Count > 0));
            Assert.True(result.Issues.All(x => x.Priority == Domain.Enums.PriorityLevel.None));
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsInvalid_ThrowsInvalidOperationException()
        {
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsInvalid_ThrowsInvalidOperationException));
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            var query = new GetProjectDetailsQuery { ProjectId = 0 };

            await Assert.ThrowsAsync<InvalidOperationException>( () => Mediator.Send(query));
            Assert.True(Database.Func(x => x.Projects.Count() > 0));
        }


    }
}
