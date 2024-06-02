using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// This class provides access to Peer.Connections
    /// </summary>
    public class PeerConnectionSet : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public PeerConnectionSet(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Returns peer ids
        /// </summary>
        public string[] Ids => JS.Call<string[]>("Object.keys", JSRef);
        /// <summary>
        /// Returns the connections for a given peer id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Array<PeerConnection>? GetPeerConnections(string id) => JSRef!.Get<Array<PeerConnection>?>(id);
    }
}
