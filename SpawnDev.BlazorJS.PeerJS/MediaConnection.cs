using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// Wraps WebRTC's media streams. To get one, use peer.call or listen for the call event.<br/>
    /// https://peerjs.com/docs/#mediaconnection
    /// </summary>
    public class MediaConnection : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public MediaConnection(IJSInProcessObjectReference _ref) : base(_ref) { }

    }
}
