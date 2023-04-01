using Microsoft.Azure.Cosmos;
using System.Net;

namespace University.Server.Exceptions
{
    public class RequestException : Exception
    {
        public List<string>? ErrorMessages { get; }

        public HttpStatusCode StatusCode { get; }

        public RequestException(string message, List<string>? errors = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            ErrorMessages = errors;
            StatusCode = statusCode;
        }

        public static RequestException ResolveCosmosException(CosmosException ex, Guid? guid)
        {
            switch (ex.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    return new BadRequestException($"CosmosDB: {ex.Message}");
                case HttpStatusCode.NotFound:
                    return new NotFoundException(guid != null ? $"CosmosDB: Couldn't find the entity with id '{guid}'." : $"CosmosDB: {ex.Message}");
                default:
                    return new InternalServerException($"CosmosDB: {ex.Message}");
            }
        }

        public static RequestException ResolveCosmosException(CosmosException ex)
        {
            return ResolveCosmosException(ex, null);
        }
    }
}
