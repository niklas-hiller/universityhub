using System.Net;

namespace University.Server.Exceptions
{
    public class InternalServerException : RequestException
    {
        public InternalServerException(string message, List<string>? errors = default)
            : base(message, errors, HttpStatusCode.InternalServerError) { }
    }
}
