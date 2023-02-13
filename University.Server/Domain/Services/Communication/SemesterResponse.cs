using University.Server.Domain.Models;

namespace University.Server.Domain.Services.Communication
{
    public class SemesterResponse : BaseResponse
    {
        public Semester Semester { get; private set; }

        private SemesterResponse(bool success, string message, Semester semester) : base(success, message)
        {
            Semester = semester;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="semester">Saved semester.</param>
        /// <returns>Response.</returns>
        public SemesterResponse(Semester semester) : this(true, string.Empty, semester)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SemesterResponse(string message) : this(false, message, null)
        { }
    }
}
