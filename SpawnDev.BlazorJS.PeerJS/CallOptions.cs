using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// Options used for Peer.Call
    /// https://peerjs.com/docs/#peercall-options
    /// </summary>
    public class CallOptions
    {
        /// <summary>
        /// Metadata associated with the connection, passed in by whoever initiated the connection. Can be accessed with mediaConnection.metadata. Can be any serializable type.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Metadata { get; set; }
        /// <summary>
        /// Function which runs before create offer to modify sdp offer message.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FuncCallback<string, string>? SdpTransform { get; set; }
    }
}
