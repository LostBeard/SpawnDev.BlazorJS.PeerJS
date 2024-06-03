using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// A PeerJS Error object
    /// https://nodejs.org/api/errors.html#class-error
    /// </summary>
    public class PeerError : Error
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public PeerError(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// The error.type property is a string label that identifies the kind of error. error.type is the most stable way to identify an error. It will only change between major versions of Node.js. In contrast, error.message strings may change between any versions of Node.js. See Node.js error codes for details about specific codes.
        /// </summary>
        public string? Type => JSRef!.Get<string?>("type");
    }
}
