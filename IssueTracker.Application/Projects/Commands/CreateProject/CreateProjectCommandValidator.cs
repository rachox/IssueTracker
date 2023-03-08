﻿using FluentValidation;
using IssueTracker.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Projects.Commands.CreateProject
{
    public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
    {
        private readonly IApplicationDbContext _ctx;

        public CreateProjectCommandValidator(IApplicationDbContext ctx)
        {
            _ctx = ctx;
            const int MAX_TITLE_LENGTH = 100;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(MAX_TITLE_LENGTH).WithMessage($"Title must not exceed {MAX_TITLE_LENGTH}.")
                .Must(BeUniqueTitle).WithMessage("Project with the same name already exists.");
        }

        public bool BeUniqueTitle(string title)
        {
            return !_ctx.Projects.Any(x => x.Title == title);
        }

    }
}
