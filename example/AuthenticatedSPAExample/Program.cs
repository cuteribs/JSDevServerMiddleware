using Cuteribs.JSDevServerMiddleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddControllers();

services.AddAuthentication(o =>
{
	o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie()
	.AddOpenIdConnect(o => configuration.Bind("Oidc", o));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication()
	.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapDefaultControllerRoute();
});

app.Use((context, next) =>
{
	if (context.User.Identity?.IsAuthenticated != true
		&& !context.Request.Path.StartsWithSegments("/signin-oidc")
	)
	{
		return context.ChallengeAsync();
	}

	return next();
});

if (app.Environment.IsDevelopment())
{
	app.UseSpa(spa =>
	{
		spa.Options.SourcePath = "ClientApp";
		spa.Options.DevServerPort = 3000;
		spa.UseJSDevServer();
	});
}
else
{
	app.MapFallbackToFile("index.html");
}

app.Run();
