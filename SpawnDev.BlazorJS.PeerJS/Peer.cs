using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.PeerJS
{
    public class Peer : JSObject
    {
        static Task? _Init = null;
        /// <summary>
        /// Load the SimplePeer Javascript library
        /// </summary>
        /// <returns>Returns when the library has been loaded</returns>
        public static Task Init(string? libPath = null)
        {
            _Init ??= JS.LoadScript(libPath ?? "_content/SpawnDev.BlazorJS.PeerJS/peerjs.min.js");
            return _Init;
        }
        #region Constructors
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Peer(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Create a new WebRTC peer connection.
        /// </summary>
        /// <param name="config"></param>
        public Peer(PeerOptions config) : base(JS.New(nameof(Peer), config)) { }
        /// <summary>
        /// Create a new WebRTC peer connection.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="config"></param>
        public Peer(string id, PeerOptions config) : base(JS.New(nameof(Peer), id, config)) { }
        /// <summary>
        /// Create a new WebRTC peer connection.
        /// </summary>
        /// <param name="id"></param>
        public Peer(string id) : base(JS.New(nameof(Peer), id)) { }
        /// <summary>
        /// Create a new WebRTC peer connection.
        /// </summary>
        public Peer() : base(JS.New(nameof(Peer))) { }
        #endregion
        /// <summary>
        /// Connects to the remote peer specified by id and returns a data connection. Be sure to listen on the error event in case the connection fails.
        /// </summary>
        /// <param name="id">The brokering ID of the remote peer (their peer.id).</param>
        /// <param name="options"></param>
        /// <returns>DataConnection</returns>
        public DataConnection Connect(string id, ConnectOptions? options = null) => JSRef!.Call<DataConnection>("connect", id, options);
        /// <summary>
        /// Calls the remote peer specified by id and returns a media connection. Be sure to listen on the error event in case the connection fails.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public MediaConnection Call(string id, MediaStream stream, CallOptions? options = null) => JSRef!.Call<MediaConnection>("call", id, stream, options);
    }
}
