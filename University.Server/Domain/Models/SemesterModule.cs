﻿namespace University.Server.Domain.Models
{
    public class SemesterModule
    {
        public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
        public User? Professor { get; set; } = null;
        public Module ReferenceModule { get; set; }
    }
}