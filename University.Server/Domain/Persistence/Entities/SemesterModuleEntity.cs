﻿using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence.Entities
{
    public class SemesterModuleEntity : BaseEntity
    {
        public ICollection<LectureEntity> Lectures { get; set; } = new List<LectureEntity>();
        public Guid ProfessorId { get; set; }
        public Guid ModuleId { get; set; }
    }
}