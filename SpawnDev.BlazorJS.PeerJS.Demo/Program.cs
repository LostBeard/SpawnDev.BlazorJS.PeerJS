
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.PeerJS;
using SpawnDev.BlazorJS.PeerJS.Demo;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add SpawnDev.BlazorJS interop
builder.Services.AddBlazorJSRuntime();

// Load the PeerJS Javascript library. 
// Can be called in a component instead if desired, or loaded using a <script> tag in the index.html
await Peer.Init();

// Run app using BlazorJSRunAsync extension method
await builder.Build().BlazorJSRunAsync();