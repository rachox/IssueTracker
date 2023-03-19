﻿using IssueTracker.Application.IntegrationTests.Common;
using IssueTracker.Application.IntegrationTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using IssueTracker.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Queries;

namespace IssueTracker.Application.IntegrationTests.Projects.Queries
{
    public class GetProjectsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly TestingHelpers _testing;


        public GetProjectsTests()
        {
            _factory = new CustomWebApplicationFactory();
            _testing = new TestingHelpers(_factory);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Handle_Always_GetProjectsOnlyWithUserAsMember(int numberOfUserProject)
        {
            var loggedUserId = _factory.Services.GetRequiredService<ICurrentUserService>().UserId;
            var testProjects = CreateTestProjects(numberOfUserProject, loggedUserId);
            await _testing.ActionDatabaseAsync(async ctx =>
                await ctx.AddRangeAsync(testProjects)
            );
            
            var query = new GetProjectsQuery();
            var projects = await _testing.MediatorSendAsync(query);

            Assert.Equal(numberOfUserProject, projects.Count());
        }

        private List<Project> CreateTestProjects(int numberOfUserProjects, string userId)
        {
            var output = new List<Project>();

            var otherMember = new ProjectMember { UserId = Guid.NewGuid().ToString() };

            for (int i = 1; i <= numberOfUserProjects; i++)
            {
                var projectTitle = $"Project {i}";
                output.Add(new Project { 
                    Title = projectTitle, 
                    Members = new List<ProjectMember> { 
                        new ProjectMember { UserId = userId } 
                    } 
                });
            }
            output.Add(new Project { Title = "Other project", Members = new List<ProjectMember> { otherMember } });

            return output;
        }
    }
}
