using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects.WebRTC;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// Wraps WebRTC's DataChannel. To get one, use peer.connect or listen for the connect event.<br/>
    /// https://peerjs.com/docs/#dataconnection
    /// </summary>
    public class DataConnection : PeerConnection
    {
        /// <summary>
        /// The value used for connections of this type
        /// </summary>
        public static string ConnectionType { get; } = "data";
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public DataConnection(IJSInProcessObjectReference _ref) : base(_ref) { }

        /// <summary>
        /// data is serialized by BinaryPack by default and sent to the remote peer.
        /// </summary>
        /// <param name="data">You can send any type of data, including objects, strings, and blobs.</param>
        public void Send(object data) => JSRef!.CallVoid("send", data);

        /// <summary>
        /// A reference to the RTCDataChannel object associated with the connection.
        /// </summary>
        public RTCDataChannel DataChannel => JSRef!.Get<RTCDataChannel>("dataChannel");
        /// <summary>
        /// The optional label passed in or assigned by PeerJS when the connection was initiated.
        /// </summary>
        public string Label => JSRef!.Get<string>("label");
        /// <summary>
        /// A reference to the RTCPeerConnection object associated with the connection.
        /// </summary>
        public RTCPeerConnection PeerConnection => JSRef!.Get<RTCPeerConnection>("peerConnection");
        /// <summary>
        /// Whether the underlying data channels are reliable; defined when the connection was initiated.
        /// </summary>
        public bool Reliable => JSRef!.Get<bool>("reliable");
        /// <summary>
        /// The serialization format of the data sent over the connection. Can be binary (default), binary-utf8, json, or none.
        /// </summary>
        public string Serialization => JSRef!.Get<string>("serialization");
        /// <summary>
        /// The number of messages queued to be sent once the browser buffer is no longer full.
        /// </summary>
        public int BufferSize=> JSRef!.Get<int>("bufferSize");

        /// <summary>
        /// Emitted when data is received from the remote peer.
        /// </summary>
        public JSEventCallback<JSObject> OnData{ get => new JSEventCallback<JSObject>("data", On, Off); set { } }
        /// <summary>
        /// Emitted when the connection is established and ready-to-use.
        /// </summary>
        public JSEventCallback OnOpen { get => new JSEventCallback("open", On, Off); set { } }
    }
}
