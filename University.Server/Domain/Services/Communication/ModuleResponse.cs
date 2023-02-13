using University.Server.Domain.Models;

namespace University.Server.Domain.Services.Communication
{
    public class ModuleResponse : BaseResponse
    {
        public Module Module { get; private set; }

        private ModuleResponse(bool success, string message, Module module) : base(success, message)
        {
            Module = module;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="module">Saved module.</param>
        /// <returns>Response.</returns>
        public ModuleResponse(Module module) : this(true, string.Empty, module)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ModuleResponse(string message) : this(false, message, null)
        { }
    }
}
