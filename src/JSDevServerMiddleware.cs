using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Cuteribs.JSDevServerMiddleware;
public static class JSDevServerMiddleware
{
	private const string LogCategoryName = "Cuteribs.JSDevServerMiddleware";

	public static void Attach(ISpaBuilder spaBuilder, Uri devServerUri)
	{
		var options = spaBuilder.Options;
		var sourcePath = options.SourcePath;

		if (string.IsNullOrEmpty(sourcePath))
		{
			throw new ArgumentException("Property 'SourcePath' cannot be null or empty", nameof(spaBuilder));
		}

		var pkgManagerCommand = options.PackageManagerCommand;
		var appBuilder = spaBuilder.ApplicationBuilder;
		var applicationStoppingToken = appBuilder
			.ApplicationServices
			.GetRequiredService<IHostApplicationLifetime>()
			.ApplicationStopping;
		var logger = GetOrCreateLogger(appBuilder);
		var diagnosticSource = appBuilder.ApplicationServices.GetRequiredService<DiagnosticSource>();
		var timeout = options.StartupTimeout;
		var startTask = StartJSDevServer(
			sourcePath,
			"start",
			pkgManagerCommand,
			devServerUri,
			(int)timeout.TotalSeconds,
			logger,
			diagnosticSource,
			applicationStoppingToken
		);

		spaBuilder.UseProxyToSpaDevelopmentServer(async () =>
		{
			if (await startTask) return devServerUri;

			throw new TimeoutException($"Failed to access {devServerUri}");
		});

		static ILogger GetOrCreateLogger(IApplicationBuilder appBuilder)
		{
			var loggerFactory = appBuilder.ApplicationServices.GetService<ILoggerFactory>();
			var logger = loggerFactory != null ? loggerFactory.CreateLogger(LogCategoryName) : NullLogger.Instance;
			return logger;
		}

		static async Task<bool> StartJSDevServer(
			string sourcePath,
			string scriptName,
			string pkgManagerCommand,
			Uri uri,
			int timeout,
			ILogger logger,
			DiagnosticSource diagnosticSource,
			CancellationToken applicationStoppingToken
		)
		{
			logger.LogInformation($"Starting dev server...");
			var scriptRunner = new NodeScriptRunner(sourcePath, scriptName, pkgManagerCommand);
			scriptRunner.Start(applicationStoppingToken);
			var isReady = await PingUri(logger, uri, timeout);
			return isReady;
		}
	}

	private static async Task<bool> PingUri(ILogger logger, Uri uri, int timeout = 60)
	{
		logger.LogInformation($"Trying to access {uri}");
		timeout *= 1000;
		var watch = new Stopwatch();
		watch.Start();
		using var client = new HttpClient();

		while (watch.ElapsedMilliseconds < timeout)
		{
			try
			{
				await client.GetAsync(uri);
				return true;
			}
			catch (SocketException) { }
			await Task.Delay(1000);
		}

		watch.Stop();
		return false;
	}
}