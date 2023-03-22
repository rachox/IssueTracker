﻿using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.Domain.Entities;

namespace IssueTracker.Application.IntegrationTests.Projects.Queries
{
    public class GetProjectDetailsTests : BaseTest
    {
        public GetProjectDetailsTests(CustomWebApplicationFactory factory)
            :base(factory)
        {
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsValid_GetDetailsIncludingMembersAndIssuesWithMembers()
        {
            var project = CreateTestProject(nameof(Handle_WhenProjectIdIsValid_GetDetailsIncludingMembersAndIssuesWithMembers));
            await _testing.ActionDatabaseAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            var query = new GetProjectDetailsQuery { ProjectId = project.Id};
            var result = await _testing.MediatorSendAsync(query);

            Assert.Equal(project.Members.Count, result.Members.Count);
            Assert.Equal(project.Issues.Count, result.Issues.Count);
            Assert.True(result.Issues.All(x => x.Title != String.Empty));
            Assert.True(result.Issues.All(x => x.Members.Count > 0));
            Assert.True(result.Issues.All(x => x.Priority == Domain.Enums.PriorityLevel.None));
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsInvalid_ThrowsInvalidOperationException()
        {
            var project = CreateTestProject(nameof(Handle_WhenProjectIdIsInvalid_ThrowsInvalidOperationException));
            await _testing.ActionDatabaseAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            var query = new GetProjectDetailsQuery { ProjectId = 0 };

            await Assert.ThrowsAsync<InvalidOperationException>( () => _testing.MediatorSendAsync(query));
            Assert.True(_testing.FuncDatabase(x => x.Projects.Count() > 0));
        }

        public static Project CreateTestProject(string title)
        {
            var projectMember1 = new ProjectMember { UserId = Guid.NewGuid().ToString() };
            var projectMember2 = new ProjectMember { UserId = Guid.NewGuid().ToString() };

            var issueMember1 = new IssueMember { UserId = projectMember1.UserId };
            var issueMember2 = new IssueMember { UserId = projectMember2.UserId };

            var issue1 = new Issue
            {
                Title = "Issue 1",
                Members = new List<IssueMember> { issueMember1 },
            };

            var issue2 = new Issue
            {
                Title = "Issue 2",
                Members = new List<IssueMember> { issueMember2 },
            };

            return new Project
            {
                Title = title,
                Members = new List<ProjectMember> { projectMember1, projectMember2 },
                Issues = new List<Issue> { issue1, issue2 }
            };
        }
    }
}