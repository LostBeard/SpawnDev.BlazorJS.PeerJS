using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// Options used for MediaConnection.Answer
    /// https://peerjs.com/docs/#mediaconnection-answer-options
    /// </summary>
    public class AnswerOptions
    {
        /// <summary>
        /// Function which runs before create answer to modify sdp answer message.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FuncCallback<string, string>? SdpTransform { get; set; }
    }
}
