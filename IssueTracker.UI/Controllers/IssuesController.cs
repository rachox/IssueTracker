﻿using FluentValidation;
using IssueTracker.Application.Issues.Commands.CreateIssue;
using IssueTracker.Application.Issues.Commands.UpdateIssue;
using IssueTracker.Application.Issues.Queries.GetIssueDetails;
using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.Domain.Entities;
using IssueTracker.UI.Models.Issues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    [Authorize(Policy = "ProjectAccess")]
    [Route("Projects/{projectid}/[controller]")]
    public class IssuesController : ControllerWithMediatR
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> Create(int projectId)
        {
            var query = new GetProjectDetails { ProjectId = projectId };
            var result = await Mediator.Send(query);
            var vm = new CreateIssueViewModel(result.Members)
            {
                ProjectId = projectId
            };
            return View(vm);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateIssueViewModel model)
        {
            var members = new List<Member>();
            model.AssignedMembersId?.ToList().ForEach(x => members.Add(new Member() { UserId = x }));
            var command = new CreateIssue()
            {
                ProjectId = model.ProjectId,
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                Members = members
            };

            try
            {
                await Mediator.Send(command);
            }
            catch (ValidationException exception)
            {
                exception.Errors.ToList().ForEach(x => ModelState.AddModelError(x.PropertyName, x.ErrorMessage));
                return View(model);
            }
            return RedirectToAction("Details", "Projects", new { Id = model.ProjectId });
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Details(int id)
        {
            var query = new GetIssueDetails(id);
            var result = await Mediator.Send(query);

            var vm = new IssueViewModel()
            {
                Id = result.Id,
                Title = result.Title, 
                Description = result.Description,
                Priority = result.Priority,
                Status = result.Status,
                Project = result.Project,
                Members = result.Members
            };

            return View(vm);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Edit(int id)
        {
            var query = new GetIssueDetails(id);
            var result = await Mediator.Send(query);

            var vm = new EditIssueViewModel()
            {
                Id = result.Id,
                Title = result.Title,
                Description = result.Description,
                Priority = result.Priority,
                Status = result.Status,
                ProjectId = result.Project.Id,
            };

            return View(vm);
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> Edit(int id, EditIssueViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var command = new UpdateIssue()
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                Status = model.Status,
                ProjectId = model.ProjectId
            };

            try
            {
                await Mediator.Send(command);
            }
            
            catch (ValidationException exception)
            {
                exception.Errors.ToList().ForEach(x => ModelState.AddModelError(x.PropertyName, x.ErrorMessage));
                return View(model);
            }
            return RedirectToAction("Details", new { id = model.Id, projectId = model.ProjectId});
        }
    }

}
