using System;
using System.Diagnostics;
using System.Threading;

namespace Cuteribs.JSDevServerMiddleware;

public class NodeScriptRunner : IDisposable
{
	private Process? _npmProcess;

	public NodeScriptRunner(
		string workingDirectory,
		string scriptName,
		string pkgManagerCommand,
		CancellationToken applicationStoppingToken
	)
	{
		if (string.IsNullOrEmpty(workingDirectory))
		{
			throw new ArgumentException("Cannot be null or empty.", nameof(workingDirectory));
		}

		if (string.IsNullOrEmpty(scriptName))
		{
			throw new ArgumentException("Cannot be null or empty.", nameof(scriptName));
		}

		if (string.IsNullOrEmpty(pkgManagerCommand))
		{
			throw new ArgumentException("Cannot be null or empty.", nameof(pkgManagerCommand));
		}

		var exeToRun = pkgManagerCommand;
		var completeArguments = $"run {scriptName}";

		if (OperatingSystem.IsWindows())
		{
			exeToRun = "cmd";
			completeArguments = $"/c {pkgManagerCommand} {completeArguments}";
		}

		var startInfo = new ProcessStartInfo(exeToRun)
		{
			Arguments = completeArguments,
			UseShellExecute = false,
			WorkingDirectory = workingDirectory
		};

		try
		{
			var process = Process.Start(startInfo)!;
			process.EnableRaisingEvents = true;
			_npmProcess = process;
		}
		catch (Exception ex)
		{
			var message = $"Failed to start '{pkgManagerCommand}'. To resolve this:.\n\n"
						+ $"[1] Ensure that '{pkgManagerCommand}' is installed and can be found in one of the PATH directories.\n"
						+ $"    Current PATH enviroment variable is: {Environment.GetEnvironmentVariable("PATH")}\n"
						+ "    Make sure the executable is in one of those directories, or update your PATH.\n\n"
						+ "[2] See the InnerException for further details of the cause.";
			throw new InvalidOperationException(message, ex);
		}

		applicationStoppingToken.Register(((IDisposable)this).Dispose);
	}


	void IDisposable.Dispose()
	{
		if (_npmProcess != null && !_npmProcess.HasExited)
		{
			_npmProcess.Kill(entireProcessTree: true);
			_npmProcess = null;
		}
	}
}
