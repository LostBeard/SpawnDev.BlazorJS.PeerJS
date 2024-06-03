# SpawnDev.BlazorJS.PeerJS
PeerJS simplifies peer-to-peer data, video, and audio calls.

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

Example Home.razor  
Open two windows and copy the ID from one to the other and click connect  
```cs
@page "/"
@using SpawnDev.BlazorJS.JSObjects;
@using System.Text;
@using System.Text.Json;
@using SpawnDev.BlazorJS.JSObjects.WebRTC;
@implements IDisposable

<PageTitle>PeerJS Test</PageTitle>

<h1>PeerJS Test</h1>
<small>Open two windows and copy the ID from one to the other and click connect</small>
<br />
<a href target="_blank">Open in New Window</a>
<br />

<div>
    <input style="width: 350px;" @bind=@id readonly></input>
</div>
<div>
    <input placeholder="Remote Id" style="width: 350px;" @bind=@targetId></input>
    <button disabled="@(dataConnection != null)" @onclick=@Connect>connect</button>
</div>
<div>
    <input placeholder="message" style="width: 350px;" @bind=@msg></input>
    <button @onclick=@Send>send</button>
</div>
<pre style="width: 600px; word-wrap: break-word; white-space: normal;">@((MarkupString)log)</pre>

@code {
    [Inject] BlazorJSRuntime JS { get; set; }

    Peer? peer = null;
    DataConnection? dataConnection = null;
    string id = "";
    string targetId = "";
    string msg = "";
    string log = "";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Load the PeerJS Javascript library.
            // the library can be loaded in the index.html instead
            await Peer.Init();
            peer = new Peer();
            peer.OnOpen += Peer_OnOpen;
            peer.OnClose += Peer_OnClose;
            peer.OnError += Peer_OnError;
            peer.OnConnection += Peer_OnConnection;
            peer.OnCall += Peer_OnCall;
            // sets _peer global var for debugging in dev tools
            JS.Set("_peer", peer);
        }
    }

    void Log(string msg)
    {
        log += $"{msg}<br/>";
        StateHasChanged();
    }

    void Peer_OnConnection(DataConnection dataConnection)
    {
        Log("Peer_OnConnection");
        InitDataConnection(dataConnection);
    }

    void Peer_OnCall(MediaConnection mediaConnection)
    {
        Log($"Peer_OnCall: {mediaConnection.Type}");
    }

    void Connect()
    {
        if (peer == null || dataConnection != null) return;
        Log("Connect");
        InitDataConnection(peer.Connect(targetId));
    }

    void InitDataConnection(DataConnection dataConnection)
    {
        if (this.dataConnection != null) return;
        this.dataConnection = dataConnection;
        Log($"InitDataConnection: {dataConnection.Label}");
        dataConnection.OnOpen += DataConnection_OnOpen;
        dataConnection.OnClose += DataConnection_OnClose;
        dataConnection.OnData += DataConnection_OnData;
    }

    void DataConnection_OnData(JSObject msg)
    {
        if (msg.JSRef!.PropertyInstanceOf() == "String")
        {
            Log(">> " + msg.JSRef!.As<string>());
        }
        else
        {
            Log("DataConnection_OnData: non-string data");
        }
    }

    void DataConnection_OnOpen()
    {
        Log("DataConnection_OnOpen");
        Send($"Hello from {id}");
    }

    void DataConnection_OnClose()
    {
        Log("DataConnection_OnClose");
        DisposeDataConnection();
    }

    void DisposeDataConnection()
    {
        if (dataConnection != null)
        {
            dataConnection.OnOpen -= DataConnection_OnOpen;
            dataConnection.OnClose -= DataConnection_OnClose;
            dataConnection.OnData -= DataConnection_OnData;
            dataConnection.Dispose();
            dataConnection = null;
        }
    }

    void Send(string msg)
    {
        if (dataConnection == null) return;
        dataConnection.Send(msg);
        Log($"<< {msg}");
    }

    void Send() => Send(msg);

    void Peer_OnOpen(string id)
    {
        this.id = id;
        Log($"Peer_OnOpen: {id}");
        StateHasChanged();
    }

    void Peer_OnClose()
    {
        Log("Peer_OnClose");
    }

    void Peer_OnError(JSObject error)
    {
        JS.Log("_error", error);
        JS.Set("_error", error);
        StateHasChanged();
    }

    public void Dispose()
    {
        DisposeDataConnection();
        if (peer != null)
        {
            peer.OnOpen -= Peer_OnOpen;
            peer.OnClose -= Peer_OnClose;
            peer.OnError += Peer_OnError;
            peer.OnConnection -= Peer_OnConnection;
            peer.OnCall -= Peer_OnCall;
            peer.Destroy();
            peer.Dispose();
            peer = null;
        }
    }
}
```