using University.Server.Domain.Models;

namespace University.Server.Domain.Services.Communication
{
    public class CourseResponse : BaseResponse
    {
        public Course Course { get; private set; }

        private CourseResponse(bool success, string message, Course course) : base(success, message)
        {
            Course = course;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="course">Saved course.</param>
        /// <returns>Response.</returns>
        public CourseResponse(Course course) : this(true, string.Empty, course)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public CourseResponse(string message) : this(false, message, null)
        { }
    }
}
