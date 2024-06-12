using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.PeerJS.Demo;
using SpawnDev.BlazorJS.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
// Add SpawnDev.BlazorJS interop
builder.Services.AddBlazorJSRuntime();

//builder.Services.AddJSRuntimeJsonOptions(o => o.ReferenceHandler  = ReferenceHandler.Preserve);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


var host = builder.Build();
await host.StartBackgroundServices();

var JS = BlazorJSRuntime.JS;
var claimsIdentity = new ClaimsIdentity("Custom", ClaimTypes.Name, ClaimTypes.Role);
claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, "Sterling"));
claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "User"));
claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Moderator"));

var token = claimsIdentity.ToBase64();
var claimsIdentityRB = token.Base64ToClaimsIdentity();

// Run app using BlazorJSRunAsync
await host.BlazorJSRunAsync();
