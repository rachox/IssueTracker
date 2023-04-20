﻿using IssueTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment
{
    public class GetProjectDetailsForManagmentQuery : IRequest<ProjectManagmentDto>
    {
        public int ProjectId { get; set; }
    }

    public class GetProjectDetailsForManagmentQueryHandler : IRequestHandler<GetProjectDetailsForManagmentQuery, ProjectManagmentDto>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IUserService _userService;

        public GetProjectDetailsForManagmentQueryHandler(IApplicationDbContext ctx, IUserService userService)
        {
            _ctx = ctx;
            _userService = userService;
        }

        public async Task<ProjectManagmentDto> Handle(GetProjectDetailsForManagmentQuery request, CancellationToken cancellationToken)
        {
            var output = new ProjectManagmentDto();

            var entity = await _ctx.Projects
                .Include(x => x.Members)
                .FirstAsync(x => x.Id == request.ProjectId);
            output.Id = entity.Id;
            output.Title = entity.Title;
            output.Members = entity.Members;
            foreach (var member in output.Members)
            {
                member.User = await _userService.GetUserByIdAsync(member.UserId);
            }
            var allUsers = await _userService.GetAllUsersAsync();
            output.OtherUsers = allUsers.ExceptBy(entity.Members.Select(x => x.UserId), allUsers => allUsers.UserId);

            return output;
        }
    }
}