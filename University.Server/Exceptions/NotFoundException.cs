using System.Net;

namespace University.Server.Exceptions
{
    public class NotFoundException : RequestException
    {
        public NotFoundException(string message)
            : base(message, null, HttpStatusCode.NotFound)
        {
        }
    }
}
