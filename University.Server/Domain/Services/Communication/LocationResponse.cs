using University.Server.Domain.Models;

namespace University.Server.Domain.Services.Communication
{
    public class LocationResponse : BaseResponse
    {
        public Location Location { get; private set; }

        private LocationResponse(bool success, string message, Location location) : base(success, message)
        {
            Location = location;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="location">Saved location.</param>
        /// <returns>Response.</returns>
        public LocationResponse(Location location) : this(true, string.Empty, location)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public LocationResponse(string message) : this(false, message, null)
        { }
    }
}
