﻿using IssueTracker.Domain.Entities;
using IssueTracker.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.IntegrationTests.Library.Helpers
{
    public static class ProjectHelpers
    {
        public static Project CreateTestProject(string title)
        {
            var projectMember1 = new Member { UserId = Guid.NewGuid().ToString() };
            var projectMember2 = new Member { UserId = Guid.NewGuid().ToString() };

            var issueMember1 = new Member { UserId = projectMember1.UserId };
            var issueMember2 = new Member { UserId = projectMember2.UserId };

            var issue1 = new Issue
            {
                Title = "Issue 1",
                Members = new List<Member> { issueMember1 },
            };

            var issue2 = new Issue
            {
                Title = "Issue 2",
                Members = new List<Member> { issueMember2 },
            };

            return new Project
            {
                Title = title,
                Members = new List<Member> { projectMember1, projectMember2 },
                Issues = new List<Issue> { issue1, issue2 }
            };
        }

        public static async Task<Project> AddToDatabaseAsync(this Project project, DatabaseHelpers database)
        {
            await database.ActionAsync(async ctx => await ctx.Projects.AddAsync(project));
            return project;
        }

        public static async Task<Project> SeedDatabaseWithMembersUsersAsync(this Task<Project> project, DatabaseHelpers database, int numberOfMembersToSkip = 0)
        {
            var _project = project.GetAwaiter().GetResult();
            foreach (var member in _project.Members.Skip(numberOfMembersToSkip))
            {
                var appUser = new ApplicationUser(member.UserId.Substring(0, 8), "Name", "Surname");
                appUser.Id = member.UserId;
                await database.ActionAsync<AuthDbContext>(ctx => ctx.Users.AddAsync(appUser));
            }

            return _project;
        }
    }
}
