# SpawnDev.BlazorJS.PeerJS
PeerJS simplifies peer-to-peer data, video, and audio calls.

## WIP: Demo and Nuget will be up soon

[![NuGet version](https://badge.fury.io/nu/SpawnDev.BlazorJS.PeerJS.svg?label=SpawnDev.BlazorJS.PeerJS)](https://www.nuget.org/packages/SpawnDev.BlazorJS.PeerJS)

**SpawnDev.BlazorJS.PeerJS** brings the amazing [peerjs](https://github.com/peers/peerjs) library to Blazor WebAssembly.

**SpawnDev.BlazorJS.PeerJS** uses [SpawnDev.BlazorJS](https://github.com/LostBeard/SpawnDev.BlazorJS) for Javascript interop allowing strongly typed, full usage of the [peerjs](https://github.com/peers/peerjs) Javascript library. Voice, video and data channels are all fully supported in Blazor WebAssembly. The **SpawnDev.BlazorJS.PeerJS** API is a strongly typed version of the API found at the [peerjs](https://github.com/peers/peerjs) repo. 

### Demo (Coming soon)
[Simple Demo](https://lostbeard.github.io/SpawnDev.BlazorJS.PeerJS/)

### Getting started

Add the Nuget package `SpawnDev.BlazorJS.PeerJS` to your project using your package manager of choice.

Modify the Blazor WASM `Program.cs` to initialize SpawnDev.BlazorJS for Javascript interop.  
Example Program.cs   
```cs
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

// Run app using BlazorJSRunAsync extension method
await builder.Build().BlazorJSRunAsync();
```
