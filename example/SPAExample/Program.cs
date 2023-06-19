using Cuteribs.JSDevServerMiddleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
	endpoints.MapDefaultControllerRoute();
});

if (app.Environment.IsDevelopment())
{
	app.UseSpa(spa =>
	{
		spa.Options.SourcePath = "ClientApp";

		spa.Options.DevServerPort = 3000;
		spa.UseReactDevelopmentServer("startcra");
	});
}
else
{
	app.MapFallbackToFile("index.html");
}

app.Run();
