using University.Server.Domain.Models;

namespace University.Server.Domain.Services.Communication
{
    public class SaveUserResponse : BaseResponse
    {
        public User User { get; private set; }

        private SaveUserResponse(bool success, string message, User user) : base(success, message)
        {
            User = user;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="user">Saved user.</param>
        /// <returns>Response.</returns>
        public SaveUserResponse(User user) : this(true, string.Empty, user)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SaveUserResponse(string message) : this(false, message, null)
        { }
    }
}
