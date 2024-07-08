using SpawnDev.BlazorJS.JSObjects.WebRTC;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// Options used when creating a new Peer<br/>
    /// https://peerjs.com/docs/#peer-options
    /// </summary>
    public class PeerOptions
    {
        /// <summary>
        /// Optional token that can be used for authentication on the PeerServer<br/>
        /// peerServer.on('connection', (client) => { console.log('client.token:', client.token); })
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Token { get; set; }
        /// <summary>
        /// API key for the cloud PeerServer. This is not used for servers other than 0.peerjs.com.<br/>
        /// PeerServer cloud runs on port 443. Please ensure it is not blocked or consider running your own PeerServer instead.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Key { get; set; }
        /// <summary>
        /// Server host. Defaults to 0.peerjs.com. Also accepts '/' to signify relative hostname.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Host { get; set; }
        /// <summary>
        /// Server port. Defaults to 443.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ushort? Port { get; set; }
        /// <summary>
        /// Ping interval in ms. Defaults to 5000.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PingInterval { get; set; }
        /// <summary>
        /// The path where your self-hosted PeerServer is running. Defaults to '/'.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Path { get; set; }
        /// <summary>
        /// true if you're using SSL.<br/>
        /// Note that our cloud-hosted server and assets may not support SSL.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Secure { get; set; }
        /// <summary>
        /// Configuration hash passed to RTCPeerConnection. This hash contains any custom ICE/TURN server configuration. Defaults to { 'iceServers': [{ 'urls': 'stun:stun.l.google.com:19302' }], 'sdpSemantics': 'unified-plan' }
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public RTCConfiguration? Config { get; set; }
        /// <summary>
        /// Prints log messages depending on the debug level passed in. Defaults to 0.<br/>
        /// 0 Prints no logs.<br/>
        /// 1 Prints only errors.<br/>
        /// 2 Prints errors and warnings.<br/>
        /// 3 Prints all logs.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Debug { get; set; }
    }
}
