﻿using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// A peer can connect to other peers and listen for connections.<br/>
    /// https://peerjs.com/docs/#peer
    /// </summary>
    public class Peer : EventEmitter
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
        /// <summary>
        /// Close the connection to the server, leaving all existing data and media connections intact. peer.disconnected will be set to true and the disconnected event will fire.
        /// </summary>
        public void Disconnect() => JSRef!.CallVoid("disconnect");
        /// <summary>
        /// Attempt to reconnect to the server with the peer's old ID. Only disconnected peers can be reconnected. Destroyed peers cannot be reconnected. If the connection fails (as an example, if the peer's old ID is now taken), the peer's existing connections will not close, but any associated errors events will fire.
        /// </summary>
        public void Reconnect() => JSRef!.CallVoid("reconnect");
        /// <summary>
        /// Close the connection to the server and terminate all existing connections. peer.destroyed will be set to true.<br/>
        /// This cannot be undone; the respective peer object will no longer be able to create or receive any connections, its ID will be forfeited on the (cloud) server, and all of its data and media connections will be closed.
        /// </summary>
        public void Destroy() => JSRef!.CallVoid("destroy");
        /// <summary>
        /// The brokering ID of this peer. If no ID was specified in the constructor, this will be undefined until the open event is emitted.
        /// </summary>
        public string Id => JSRef!.Get<string>("id");
        /// <summary>
        /// A hash of all connections associated with this peer, keyed by the remote peer's ID.<br/>
        /// We recommend keeping track of connections yourself rather than relying on this hash.
        /// </summary>
        public PeerConnectionSet Connections => JSRef!.Get<PeerConnectionSet>("connections");
        /// <summary>
        /// false if there is an active connection to the PeerServer.
        /// </summary>
        public bool Disconnected => JSRef!.Get<bool>("disconnected");
        /// <summary>
        /// true if this peer and all of its connections can no longer be used.
        /// </summary>
        public bool Destroyed => JSRef!.Get<bool>("destroyed");
        /// <summary>
        /// Emitted when a connection to the PeerServer is established. You may use the peer before this is emitted, but messages to the server will be queued.<br/>
        /// id string - the brokering ID of the peer (which was either provided in the constructor or assigned by the server)
        /// </summary>
        public JSEventCallback<string> OnOpen { get => new JSEventCallback<string>("open", On, Off); set { } }
        /// <summary>
        /// Emitted when a new data connection is established from a remote peer.
        /// </summary>
        public JSEventCallback<DataConnection> OnConnection { get => new JSEventCallback<DataConnection>("connection", On, Off); set { } }
        /// <summary>
        /// Emitted when a remote peer attempts to call you. The emitted mediaConnection is not yet active; you must first answer the call (mediaConnection.answer([stream]);). Then, you can listen for the stream event.
        /// </summary>
        public JSEventCallback<MediaConnection> OnCall { get => new JSEventCallback<MediaConnection>("call", On, Off); set { } }
        /// <summary>
        /// Emitted when the peer is destroyed and can no longer accept or create any new connections. At this time, the peer's connections will all be closed.
        /// </summary>
        public JSEventCallback OnClose { get => new JSEventCallback("close", On, Off); set { } }
        /// <summary>
        /// Emitted when the peer is disconnected from the signalling server, either manually or because the connection to the signalling server was lost. When a peer is disconnected, its existing connections will stay alive, but the peer cannot accept or create any new connections. You can reconnect to the server by calling peer.reconnect().
        /// </summary>
        public JSEventCallback OnDisconnected { get => new JSEventCallback("disconnected", On, Off); set { } }
        /// <summary>
        /// Errors on the peer are almost always fatal and will destroy the peer. Errors from the underlying socket and PeerConnections are forwarded here.<br/>
        /// 
        /// </summary>
        public JSEventCallback<JSObject> OnError { get => new JSEventCallback<JSObject>("error", On, Off); set { } }
    }
}
