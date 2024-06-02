using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// Wraps WebRTC's media streams. To get one, use peer.call or listen for the call event.<br/>
    /// https://peerjs.com/docs/#mediaconnection
    /// </summary>
    public class MediaConnection : PeerConnection
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public MediaConnection(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// When receiving a call event on a peer, you can call .answer on the media connection provided by the callback to accept the call and optionally send your own media stream.
        /// </summary>
        /// <param name="stream">A WebRTC media stream from getUserMedia.</param>
        /// <param name="options">Function which runs before create answer to modify sdp answer message.</param>
        public void Answer(MediaStream? stream = null, AnswerOptions? options = null) => JSRef!.CallVoid("answer", stream);
        /// <summary>
        /// Emitted when a remote peer adds a stream.
        /// </summary>
        public JSEventCallback<MediaStream> OnStream { get => new JSEventCallback<MediaStream>("stream", On, Off); set { } }
    }
}
