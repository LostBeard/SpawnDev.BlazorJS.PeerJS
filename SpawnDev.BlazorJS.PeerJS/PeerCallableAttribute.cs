namespace SpawnDev.BlazorJS.PeerJS
{
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
}
