﻿using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class CourseResource
    {
        public string Name { get; set; }
        public List<User> Students { get; set; }
        public List<Module> Modules { get; set; }
    }
}
