using System;
using System.Diagnostics;
using System.Threading;

namespace Cuteribs.JSDevServerMiddleware;

public sealed class NodeScriptRunner : IDisposable
{
	private readonly Process _process;

	public NodeScriptRunner(string workingDirectory, string scriptName, string pkgManagerCommand)
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

		_process = new()
		{
			StartInfo = startInfo,
			EnableRaisingEvents = true
		};
	}

	public void Start(CancellationToken cancellationToken)
	{
		_process.Start();
		cancellationToken.Register(((IDisposable)this).Dispose);
	}

	public void Dispose()
	{
		if (!_process.HasExited)
		{
			_process.Kill(true);
			_process.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
