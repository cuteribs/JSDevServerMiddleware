using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices;
using System;

namespace Cuteribs.JSDevServerMiddleware;

public static class JSDevServerMiddlewareExtensions
{
	public static void UseJSDevServer(this ISpaBuilder? spaBuilder, string npmScript = "start")
	{
		var uri = new Uri($"http://localhost:{spaBuilder?.Options.DevServerPort}");
		UseJSDevServer(spaBuilder, uri, npmScript);
	}

	public static void UseJSDevServer(this ISpaBuilder? spaBuilder, Uri devServerUri, string npmScript = "start")
	{
		var spaOptions = spaBuilder?.Options ?? throw new ArgumentNullException(nameof(spaBuilder));

		if (string.IsNullOrEmpty(spaOptions.SourcePath))
		{
			var message = $"To use {nameof(UseJSDevServer)}, you must supply a non-empty value for the {nameof(SpaOptions.SourcePath)} property of {nameof(SpaOptions)} when calling {nameof(SpaApplicationBuilderExtensions.UseSpa)}.";
			throw new InvalidOperationException(message);
		}

		JSDevServerMiddleware.Attach(spaBuilder, devServerUri, npmScript);
	}
}
