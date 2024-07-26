using Microsoft.Extensions.DependencyInjection;
using SpawnDev.BlazorJS.WebWorkers;
using System.Reflection;
using Array = SpawnDev.BlazorJS.JSObjects.Array;

namespace SpawnDev.BlazorJS.PeerJS
{
    public class WebPeer : RemoteDispatcher
    {
        public Peer Peer { get; private set; }
        public DataConnection? DataConnection { get; private set; } = null;
        public WebPeer(IServiceProvider serviceProvider, Peer peer, DataConnection dataConnection) : base(serviceProvider)
        {
            Peer = peer;
            InitDataConnection(dataConnection);
        }
        public WebPeer(IServiceProvider serviceProvider, Peer peer, string targetId, ConnectOptions? options = null) : base(serviceProvider)
        {
            Peer = peer;
            InitDataConnection(Peer.Connect(targetId, options));
        }
        protected override void SendCall(object?[] args) => DataConnection!.Send(args);
        private void InitDataConnection(DataConnection dataConnection)
        {
            DataConnection = dataConnection;
            DataConnection.OnOpen += DataConnection_OnOpen;
            DataConnection.OnClose += DataConnection_OnClose;
            DataConnection.OnError += DataConnection_OnError;
            DataConnection.On<Array>("data", DataConnection_OnData);
            if (DataConnection.Open)
            {
                try
                {
                    SendReadyFlag();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SendReadyFlag failed: {ex.Message}");
                }
            }
        }
        private void DataConnection_OnData(Array msg) => _ = HandleCall(msg);
        private void DataConnection_OnOpen()
        {
            //Log("DataConnection_OnOpen");
            //Send($"Hello from {id}");
            try
            {
                SendReadyFlag();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendReadyFlag failed: {ex.Message}");
            }
        }
        private void DataConnection_OnClose()
        {
            JS.Log("DataConnection_OnClose");
            //DisposeDataConnection();
            ResetWhenReady();
        }
        private void DataConnection_OnError(PeerError error)
        {
            //Log($"DataConnection_OnError: {error.Type}");
        }
        public override void Dispose()
        {
            if (IsDisposed) return;
            if (DataConnection != null)
            {
                DataConnection.OnOpen -= DataConnection_OnOpen;
                DataConnection.OnClose -= DataConnection_OnClose;
                DataConnection.OnError -= DataConnection_OnError;
                DataConnection.Off<Array>("data", DataConnection_OnData);
                DataConnection.Dispose();
            }
            base.Dispose();
        }
        protected override Task<string?> CanCallCheck(MethodInfo methodInfo, RemoteCallableAttribute? remoteCallableAttr, ServiceDescriptor? info, object? instance)
        {
            return base.CanCallCheck(methodInfo, remoteCallableAttr, info, instance);
        }
        protected override Task<string?> PreCallCheck(MethodInfo methodInfo, object?[]? args = null)
        {
            return base.PreCallCheck(methodInfo, args);
        }
        protected override Task<object?> ResolveLocalInstance(Type parameterType)
        {
            return base.ResolveLocalInstance(parameterType);
        }
    }
}
