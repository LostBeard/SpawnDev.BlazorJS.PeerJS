using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// Base class for MediaConnection and DataConnection
    /// </summary>
    public class PeerConnection : EventEmitter
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public PeerConnection(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// For media connections, this is always 'media'.
        /// </summary>
        public string Type => JSRef!.Get<string>("type");
        /// <summary>
        /// Any type of metadata associated with the connection, passed in by whoever initiated the connection.
        /// </summary>
        public JSObject? Metadata => JSRef!.Get<JSObject>("metadata");
        /// <summary>
        /// Any type of metadata associated with the connection, passed in by whoever initiated the connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T MetadataAs<T>() => JSRef!.Get<T>("metadata");
        /// <summary>
        /// Closes the media connection.
        /// </summary>
        public void Close() => JSRef!.CallVoid("close");
        /// <summary>
        /// Whether the media connection is active (e.g. your call has been answered). You can check this if you want to set a maximum wait time for a one-sided call.
        /// </summary>
        public bool Open => JSRef!.Get<bool>("open");
        /// <summary>
        /// The ID of the peer on the other end of this connection.
        /// </summary>
        public string Peer => JSRef!.Get<string>("peer");
        /// <summary>
        /// Emitted when either you or the remote peer closes the data connection.
        /// </summary>
        public ActionEvent OnClose { get => new ActionEvent("close", On, Off); set { } }
        /// <summary>
        /// Emitted when an error occurs
        /// </summary>
        public ActionEvent<PeerError> OnError { get => new ActionEvent<PeerError>("error", On, Off); set { } }
    }
}
