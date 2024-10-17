using Ecommerce.WebAssembly;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Blazored.LocalStorage;
using Blazored.Toast;

using Ecommerce.WebAssembly.Servicios.Contrato;
using Ecommerce.WebAssembly.Servicios.Implementacion;

using CurrieTechnologies.Razor.SweetAlert2;

using Microsoft.AspNetCore.Components.Authorization;
using Ecommerce.WebAssembly.Extensiones;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5041/api/") });



builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();
builder.Services.AddScoped<ICategoriaServicio, CategoriaServicio>();
builder.Services.AddScoped<IProductoServicio, ProductoServicio>();
builder.Services.AddScoped<ICarritoServicio, CarritoServicio>();
builder.Services.AddScoped<IVentaServicio, VentaServicio>();
builder.Services.AddScoped<IDashboardServicio, DashboardServicio>();
builder.Services.AddScoped<IMensajeServicio, MensajeServicio>();
builder.Services.AddScoped<IFiltroServicio, FiltroServicio>();

builder.Services.AddSweetAlert2();

builder.Services.AddAuthorizationCore();    
builder.Services.AddScoped<AuthenticationStateProvider,AutenticacionExtension>();

await builder.Build().RunAsync();
