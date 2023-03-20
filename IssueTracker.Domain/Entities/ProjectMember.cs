﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Domain.Entities
{
    public class ProjectMember : IEquatable<ProjectMember>
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string UserId { get; set; }

        public bool Equals(ProjectMember other)
        {
            return UserId == other.UserId && ProjectId == other.ProjectId;
        }

        public override bool Equals(object obj) => Equals(obj as ProjectMember);
        public override int GetHashCode() => (UserId, ProjectId).GetHashCode();
    }
}
