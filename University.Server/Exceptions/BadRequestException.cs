using System.Net;

namespace University.Server.Exceptions
{
    public class BadRequestException : RequestException
    {
        public BadRequestException(string message)
            : base(message, null, HttpStatusCode.BadRequest)
        {
        }
    }
}
