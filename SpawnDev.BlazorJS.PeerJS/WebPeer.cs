using Microsoft.Extensions.DependencyInjection;
using SpawnDev.BlazorJS.Reflection;
using System.Reflection;
using Array = SpawnDev.BlazorJS.JSObjects.Array;

namespace SpawnDev.BlazorJS.PeerJS
{
    /// <summary>
    /// Method parameters marked with the CallSide attribute will be resolved on the called side
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Parameter)]
    public class CallSideAttribute : Attribute { }
    /// <summary>
    /// Method parameters marked with the FromServices attribute will be resolved from the called side peer service provider
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Parameter)]
    public class FromServicesAttribute : Attribute { }
    /// <summary>
    /// Used to mark methods as callable
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class PeerCallableAttribute : Attribute
    {
        /// <summary>
        /// Methods with the PeerCallable attribute and NoReply = true will not send exceptions or results back to the caller<br/>
        /// That makes NoReply calls quicker
        /// </summary>
        public bool NoReply { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WebPeer : AsyncCallDispatcher, IDisposable
    {
        private BlazorJSRuntime JS;
        private IServiceScope ServiceProviderScope;
        private IServiceProvider ScopedServiceProvider;
        private IServiceCollection ServiceDescriptors;
        private Dictionary<string, TaskCompletionSource<Array>> waitingResponse = new Dictionary<string, TaskCompletionSource<Array>>();
        public bool InheritAttributes { get; set; } = true;
        public Peer Peer { get; private set; }
        public DataConnection? DataConnection { get; private set; } = null;
        /// <summary>
        /// If set to true, calls from the remote peer onto this peer are enabled<br/>
        /// Per call access, if enabled, will apply
        /// </summary>
        public bool ServeEnabled { get; set; } = true;
        /// <summary>
        /// If false, static methods cannot be called
        /// </summary>
        public bool AllowStaticMethods { get; set; } = true;
        /// <summary>
        /// If false, private methods cannot be called
        /// </summary>
        public bool AllowPrivateMethods { get; set; } = true;
        /// <summary>
        /// If true, static methods in non-service types can be called.<br/>
        /// </summary>
        public bool AllowNonServiceStaticMethods { get; set; } = true;
        /// <summary>
        /// If true, special methods (like getters and setters) will be allowed
        /// </summary>
        public bool AllowSpecialMethods { get; set; } = false;
        /// <summary>
        /// If true, the PeerCallable attribute must be present on the method or the containing class
        /// </summary>
        public bool RequirePeerCallableAttribute { get; set; } = true;
        /// <summary>
        /// Returns true if this instance has been disposed
        /// </summary>
        public bool IsDisposed { get; private set; }
        public WebPeer(IServiceProvider serviceProvider, Peer peer, DataConnection dataConnection)
        {
            JS = serviceProvider.GetRequiredService<BlazorJSRuntime>();
            ServiceDescriptors = serviceProvider.GetRequiredService<IServiceCollection>();
            ServiceProviderScope = serviceProvider.CreateScope();
            ScopedServiceProvider = ServiceProviderScope.ServiceProvider;
            Peer = peer;
            InitDataConnection(dataConnection);
        }
        public WebPeer(IServiceProvider serviceProvider, Peer peer, string targetId, ConnectOptions? options = null)
        {
            JS = serviceProvider.GetRequiredService<BlazorJSRuntime>();
            ServiceDescriptors = serviceProvider.GetRequiredService<IServiceCollection>();
            ServiceProviderScope = serviceProvider.CreateScope();
            ScopedServiceProvider = ServiceProviderScope.ServiceProvider;
            Peer = peer;
            InitDataConnection(Peer.Connect(targetId, options));
        }
        public override async Task<object?> Call(MethodInfo methodInfo, object?[]? args = null)
        {
            if (DataConnection == null || !DataConnection.Open) throw new Exception("Not connected");
            object? retValue = null;
            var methodInfoSerialized = SerializableMethodInfoSlim.SerializeMethodInfo(methodInfo);
            var msgId = Guid.NewGuid().ToString();
            var outArgs = await PreSerializeArgs(msgId, methodInfo, args);
            var peerVoidCallAttr = methodInfo.GetCustomAttribute<PeerCallableAttribute>(true);
            var sendResult = peerVoidCallAttr == null || !peerVoidCallAttr.NoReply;
            Send(new object?[] { sendResult ? "?" : ".", msgId, methodInfoSerialized, outArgs });
            if (sendResult)
            {
                var tcs = new TaskCompletionSource<Array>();
                waitingResponse.Add(msgId, tcs);
                var finalReturnType = methodInfo.GetFinalReturnType();
                var ret = await tcs.Task;
                if (finalReturnType != typeof(void))
                {
                    string? retError = ret.Shift<string?>();
                    if (!string.IsNullOrEmpty(retError))
                    {
                        var error = DeserializeException(retError);
                        throw error;
                    }
                    // custom deserialization of result if needed
                    retValue = ret.Shift(finalReturnType);
                }
            }
            // cleanup any request resources
            return retValue;
        }
        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            if (DataConnection != null)
            {
                DataConnection.OnOpen -= DataConnection_OnOpen;
                DataConnection.OnClose -= DataConnection_OnClose;
                DataConnection.OnError -= DataConnection_OnError;
                DataConnection.Off<Array>("data", DataConnection_OnData);
                DataConnection.Dispose();
            }
            ServiceProviderScope.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="peerCallableAttr"></param>
        /// <param name="info"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected virtual async Task<string?> CanCallCheck(MethodInfo methodInfo, PeerCallableAttribute? peerCallableAttr, ServiceDescriptor? info, object? instance)
        {
            await Task.Delay(1);
            if (!AllowNonServiceStaticMethods && info == null)
            {
                return "HandleCallError: Service not found";
            }
            if (!AllowSpecialMethods && methodInfo.IsSpecialName)
            {
                return "HandleCallError: Access denied to special methods";
            }
            if (!AllowPrivateMethods && methodInfo.IsPrivate)
            {
                return "HandleCallError: Access denied private method";
            }
            if (!AllowStaticMethods && methodInfo.IsStatic)
            {
                return "HandleCallError: Access denied static method";
            }
            if (RequirePeerCallableAttribute && peerCallableAttr == null)
            {
                return "HandleCallError: Access denied not PeerCallable";
            }
            return null;
        }
        private void InitDataConnection(DataConnection dataConnection)
        {
            DataConnection = dataConnection;
            DataConnection.OnOpen += DataConnection_OnOpen;
            DataConnection.OnClose += DataConnection_OnClose;
            DataConnection.OnError += DataConnection_OnError;
            DataConnection.On<Array>("data", DataConnection_OnData);
        }
        private async Task<object?> CallSideArgumentResolver(Type parameterType)
        {
            if (parameterType == this.GetType() || parameterType == typeof(WebPeer))
            {
                return this;
            }
            var ret = ScopedServiceProvider.GetService(parameterType);
            if (ret != null)
            {
                return ret;
            }
            return parameterType.GetDefaultValue();
        }
        private async Task<object?[]> PreSerializeArgs(string msgId, MethodInfo methodInfo, object?[]? args)
        {
            var methodsParamTypes = methodInfo.GetParameters();
            var callArgsLength = args == null ? 0 : args.Length;
            var ret = new object?[callArgsLength];
            for (var i = 0; i < callArgsLength; i++)
            {
                var methodParam = methodsParamTypes[i];
                var methodParamType = methodParam.ParameterType;
#if NET8_0_OR_GREATER
                var fromKeyedServicesAttr = methodParam.GetCustomAttribute<FromKeyedServicesAttribute>(InheritAttributes);
                if (fromKeyedServicesAttr != null) continue;
#endif
                var fromServicesAttr = methodParam.GetCustomAttribute<FromServicesAttribute>(InheritAttributes);
                if (fromServicesAttr != null) continue;
                var callSideAttr = methodParam.GetCustomAttribute<CallSideAttribute>(InheritAttributes);
                if (callSideAttr != null) continue;
                if (i < callArgsLength)
                {
                    // custom serialization can be done here tp package args![i]
                    ret[i] = args![i];
                }
            }
            return ret;
        }
        private async Task<object?[]?> PostDeserializeArgs(string msgId, MethodInfo methodInfo, Array? callArgs)
        {
            var methodsParamTypes = methodInfo.GetParameters();
            var ret = new object?[methodsParamTypes.Length];
            var callArgsLength = callArgs == null ? 0 : callArgs.Length;
            for (var i = 0; i < methodsParamTypes.Length; i++)
            {
                var methodParam = methodsParamTypes[i];
                var methodParamType = methodParam.ParameterType;
#if NET8_0_OR_GREATER
                var fromKeyedServicesAttr = methodParam.GetCustomAttribute<FromKeyedServicesAttribute>(InheritAttributes);
                if (fromKeyedServicesAttr != null)
                {
                    ret[i] = ScopedServiceProvider.GetRequiredKeyedService(methodParamType, fromKeyedServicesAttr.Key);
                    continue;
                }
#endif
                var fromServicesAttr = methodParam.GetCustomAttribute<FromServicesAttribute>(InheritAttributes);
                if (fromServicesAttr != null)
                {
                    ret[i] = ScopedServiceProvider.GetRequiredService(methodParamType);
                    continue;
                }
                var callSideAttr = methodParam.GetCustomAttribute<CallSideAttribute>(InheritAttributes);
                if (callSideAttr != null)
                {
                    ret[i] = await CallSideArgumentResolver(methodParamType);
                }
                else if (i < callArgsLength)
                {
                    // custom deserialization can be done here to get type methodParamType from callArgs Array
                    ret[i] = callArgs!.GetItem(methodParamType, i);
                }
                else if (methodParam.HasDefaultValue)
                {
                    ret[i] = methodParam.DefaultValue;
                }
            }
            return ret;
        }
        private async Task HandleCall(Array msg, bool sendResult)
        {
            object? instance = null;
            object? retValue = null;
            string? retError = null;
            string? msgId = null;
            MethodInfo? methodInfo = null;
            object?[]? args = null;
            PeerCallableAttribute? peerCallableAttr = null;
            ServiceDescriptor? info = null;
            // rebuild request MethodInfo and arguments
            Array? argsPreDeser = null;
            try
            {
                msgId = msg.Shift<string>();
                if (!ServeEnabled)
                {
                    retError = "HandleCallError: Offline";
                    goto SendResponse;
                }
                var methodInfoSerialized = msg.Shift<string>();
                methodInfo = SerializableMethodInfoSlim.DeserializeMethodInfo(methodInfoSerialized);
                if (methodInfo == null)
                {
                    retError = "HandleCallError: Method not found";
                    goto SendResponse;
                }
                peerCallableAttr = methodInfo.GetCustomAttribute<PeerCallableAttribute>();
                info = ServiceDescriptors.FindServiceDescriptors(methodInfo.ReflectedType)!.FirstOrDefault();
                // what is left in `msg` is the call arguments
                argsPreDeser = msg.Shift<Array>();
                args = argsPreDeser == null ? null : await PostDeserializeArgs(msgId, methodInfo, argsPreDeser);
            }
            catch (Exception ex)
            {
                //JS.Log($"HandleCall failed to rebuild the request method or args: {ex.Message}");
                retError = $"HandleCallError: Failed to rebuild the request method or args: {ex.Message}";
                goto SendResponse;
            }
            finally
            {
                argsPreDeser?.Dispose();
                msg.Dispose();
            }
            // get the instance for this call (if non-static)
            if (!methodInfo.IsStatic)
            {
                instance = info == null ? null : await ScopedServiceProvider.GetServiceAsync(info!.ServiceType);
                if (instance == null)
                {
                    retError = "HandleCallError: Service not found";
                    goto SendResponse;
                }
            }
            var deniedError = await CanCallCheck(methodInfo, peerCallableAttr, info, instance);
            if (!string.IsNullOrEmpty(deniedError))
            {
                retError = deniedError;
                goto SendResponse;
            }
            // invoke the call capturing the result or exception
            try
            {
                retValue = await methodInfo.InvokeAsync(instance, args);
            }
            catch (Exception ex)
            {
                retError = SerializeException(ex);
                goto SendResponse;
            }
        // pre-serialize the retValue (if needed) capturing exceptions
        // ................
        // Send the result or error
        SendResponse:
            if (!sendResult || string.IsNullOrEmpty(msgId)) return;
            if (peerCallableAttr != null && peerCallableAttr.NoReply) return;
            try
            {
                Send(new object?[] { "=", msgId, retError, retValue });
            }
            catch (Exception ex)
            {
                JS.Log($"DataConnection.Send failed: {ex.Message}");
            }
        }
        private void Send(object?[] args) => DataConnection!.Send(args);
        private void HandleReply(Array? msg)
        {
            if (msg == null) return;
            var msgId = msg.Shift<string>();
            if (waitingResponse.TryGetValue(msgId, out var waiter))
            {
                waitingResponse.Remove(msgId);
                waiter.TrySetResult(msg);
            }
        }
        private void DataConnection_OnData(Array msg)
        {
            if (msg != null && Array.IsArray(msg) && msg.Length > 0)
            {
                try
                {
                    var msgType = msg.Shift<string>();
                    switch (msgType)
                    {
                        case ".": // message
                            _ = HandleCall(msg, false);
                            return;
                        case "?": // call
                            _ = HandleCall(msg, true);
                            return;
                        case "=": // response
                            HandleReply(msg);
                            return;
                    }
                }
                catch (Exception ex)
                {
                    JS.Log($"DataConnection_OnData: {ex.Message}");
                }
            }
            msg?.Dispose();
        }
        private void DataConnection_OnOpen()
        {
            //Log("DataConnection_OnOpen");
            //Send($"Hello from {id}");
        }
        private void DataConnection_OnClose()
        {
            //Log("DataConnection_OnClose");
            //DisposeDataConnection();
        }
        private void DataConnection_OnError(PeerError error)
        {
            //Log($"DataConnection_OnError: {error.Type}");
        }
        private static string SerializeException(Exception exception)
        {
            return exception.Message;
        }
        private static Exception DeserializeException(string exception)
        {
            var ret = new Exception(exception);
            return ret;
        }
    }
}
