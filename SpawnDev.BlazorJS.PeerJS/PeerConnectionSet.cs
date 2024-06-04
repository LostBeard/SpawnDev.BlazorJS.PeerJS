using Microsoft.Extensions.DependencyInjection;
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
        public PeerConnection[]? GetPeerConnections(string id) => JSRef!.Get<PeerConnection[]?>(id);
        /// <summary>
        /// Returns all DataConnection associated with this peer id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataConnection[]? GetDataConnections(string id) => GetPeerConnections<DataConnection>(id, DataConnection.ConnectionType);
        /// <summary>
        /// Returns all MediaConnections associated with this peer id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MediaConnection[]? GetMediaConnections(string id) => GetPeerConnections<MediaConnection>(id, MediaConnection.ConnectionType);
        /// <summary>
        /// Returns all PeerConnections of the given type associated with this peer id
        /// </summary>
        /// <typeparam name="TConnection"></typeparam>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private TConnection[]? GetPeerConnections<TConnection>(string id, string type) where TConnection : PeerConnection
        {
            var connections = GetPeerConnections(id);
            if (connections == null) return null;
            var ret = new List<TConnection>();
            foreach (var connection in connections)
            {
                if (connection.Type != type)
                {
                    connection.Dispose();
                }
                else
                {
                    ret.Add(connection.JSRefMove<TConnection>());
                }
            }
            return ret.ToArray();
        }
        /// <summary>
        /// Returns a dictionary of all MediaConnections keyed by peer id
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, MediaConnection[]> GetMediaConnections() => GetConnections<MediaConnection>(MediaConnection.ConnectionType);
        /// <summary>
        /// Returns a dictionary of all DataConnections keyed by peer id
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, DataConnection[]> GetDataConnections() => GetConnections<DataConnection>(DataConnection.ConnectionType);
        private Dictionary<string, TConnection[]> GetConnections<TConnection>(string type) where TConnection : PeerConnection
        {
            var ret = new Dictionary<string, TConnection[]>();
            foreach (var id in Ids)
            {
                var conns = GetPeerConnections<TConnection>(id, type)!;
                if (conns == null) continue;
                ret[id] = conns;
            }
            return ret;
        }
    }
}
