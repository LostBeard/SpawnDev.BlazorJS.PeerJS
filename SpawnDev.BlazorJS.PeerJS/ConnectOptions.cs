using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// Peer.Connect options<br/>
    /// https://peerjs.com/docs/#peerconnect-options
    /// </summary>
    public class ConnectOptions
    {
        /// <summary>
        /// A unique label by which you want to identify this data connection. If left unspecified, a label will be generated at random. Can be accessed with dataConnection.label.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Label { get; set; }
        /// <summary>
        /// Metadata associated with the connection, passed in by whoever initiated the connection. Can be accessed with dataConnection.metadata. Can be any serializable type.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Metadata { get; set; }
        /// <summary>
        /// Can be binary (default), binary-utf8, json, or none. Can be accessed with dataConnection.serialization.<br/>
        /// binary-utf8 will take a performance hit because of the way UTF8 strings are packed into binary format.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Serialization { get; set; }
        /// <summary>
        /// Whether the underlying data channels should be reliable (e.g. for large file transfers) or not (e.g. for gaming or streaming). Defaults to false.<br/>
        /// Setting reliable to true will use a shim for incompatible browsers (Chrome 30 and below only) and thus may not offer full performance.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Reliable { get; set; }
    }
}
