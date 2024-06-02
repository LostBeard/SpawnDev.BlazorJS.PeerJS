using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// Wraps WebRTC's DataChannel. To get one, use peer.connect or listen for the connect event.<br/>
    /// https://peerjs.com/docs/#dataconnection
    /// </summary>
    public class DataConnection : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public DataConnection(IJSInProcessObjectReference _ref) : base(_ref) { }

    }
}
