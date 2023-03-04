﻿using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class UpdateModuleResource
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CreditPoints { get; set; }
        public EModuleType? ModuleType { get; set; }
    }
}