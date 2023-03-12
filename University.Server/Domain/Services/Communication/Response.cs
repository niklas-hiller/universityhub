using University.Server.Domain.Models;

namespace University.Server.Domain.Services.Communication
{
    public class Response<T> : BaseResponse where T : Base
    {
        public T? ResponseEntity { get; private set; }

        private Response(int statusCode, string message, T? responseEntity) : base(statusCode, message)
        {
            ResponseEntity = responseEntity;
        }

        /// <summary>
        /// Creates an simple response.
        /// </summary>
        /// <param name="statusCode">Status Code.</param>
        /// <returns>Response.</returns>
        public Response(int statusCode) : this(statusCode, string.Empty, default)
        { }

        /// <summary>
        /// Creates an success response.
        /// </summary>
        /// <param name="statusCode">Status Code.</param>
        /// <param name="responseEntity">Response Entity.</param>
        /// <returns>Response.</returns>
        public Response(int statusCode, T responseEntity) : this(statusCode, string.Empty, responseEntity)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="statusCode">Status Code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public Response(int statusCode, string message) : this(statusCode, message, default)
        { }
    }
}
