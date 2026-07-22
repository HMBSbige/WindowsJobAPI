using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using WindowsJobAPI.Models;

namespace WindowsJobAPI.Tests;

internal sealed class JobObjectTests
{
	private static readonly TimeSpan ProcessExitTimeout = TimeSpan.FromSeconds(5);

	[SuppressMessage("ReSharper", "DisposeOnUsingVariable")]
	[Test]
	public async Task DisposeTerminatesProcess(CancellationToken cancellationToken)
	{
		using JobObject job = new();
		using Process process = CreateLongRunningProcess();

		try
		{
			await Assert.That(job.AddProcess(process)).IsTrue();
			await Assert.That(process.HasExited).IsFalse();

			job.Dispose();
			await WaitForExitAsync(process, cancellationToken);

			await Assert.That(process.HasExited).IsTrue();
		}
		finally
		{
			job.Dispose();
			TerminateIfRunning(process);
		}
	}

	[SuppressMessage("ReSharper", "DisposeOnUsingVariable")]
	[Test]
	public async Task AddProcessAcceptsSafeHandle(CancellationToken cancellationToken)
	{
		using JobObject job = new();
		using Process process = CreateLongRunningProcess();

		try
		{
			await Assert.That(job.AddProcess(process.SafeHandle)).IsTrue();
			await Assert.That(process.HasExited).IsFalse();

			job.Dispose();
			await WaitForExitAsync(process, cancellationToken);

			await Assert.That(process.HasExited).IsTrue();
		}
		finally
		{
			job.Dispose();
			TerminateIfRunning(process);
		}
	}

	[SuppressMessage("ReSharper", "DisposeOnUsingVariable")]
	[Test]
	public async Task AddProcessAcceptsProcessId(CancellationToken cancellationToken)
	{
		using JobObject job = new();
		using Process process = CreateLongRunningProcess();

		try
		{
			await Assert.That(job.AddProcess(process.Id)).IsTrue();
			await Assert.That(process.HasExited).IsFalse();

			job.Dispose();
			await WaitForExitAsync(process, cancellationToken);

			await Assert.That(process.HasExited).IsTrue();
		}
		finally
		{
			job.Dispose();
			TerminateIfRunning(process);
		}
	}

	[SuppressMessage("ReSharper", "DisposeOnUsingVariable")]
	[Test]
	public async Task JobsAreIndependent(CancellationToken cancellationToken)
	{
		using JobObject firstJob = new();
		using JobObject secondJob = new();
		(Process First, Process Second) processes = CreateLongRunningProcessPair();
		using Process firstProcess = processes.First;
		using Process secondProcess = processes.Second;

		try
		{
			await Assert.That(firstJob.AddProcess(firstProcess)).IsTrue();
			await Assert.That(secondJob.AddProcess(secondProcess)).IsTrue();
			await Assert.That(secondProcess.HasExited).IsFalse();

			secondJob.Dispose();
			await WaitForExitAsync(secondProcess, cancellationToken);

			await Assert.That(secondProcess.HasExited).IsTrue();
			await Assert.That(firstProcess.HasExited).IsFalse();

			firstJob.Dispose();
			await WaitForExitAsync(firstProcess, cancellationToken);
			await Assert.That(firstProcess.HasExited).IsTrue();
		}
		finally
		{
			secondJob.Dispose();
			firstJob.Dispose();
			TerminateIfRunning(secondProcess);
			TerminateIfRunning(firstProcess);
		}
	}

	[SuppressMessage("ReSharper", "DisposeOnUsingVariable")]
	[Test]
	public async Task KillingOneProcessDoesNotTerminateOthers(CancellationToken cancellationToken)
	{
		using JobObject job = new();
		(Process First, Process Second) processes = CreateLongRunningProcessPair();
		using Process firstProcess = processes.First;
		using Process secondProcess = processes.Second;

		try
		{
			await Assert.That(job.AddProcess(firstProcess)).IsTrue();
			await Assert.That(job.AddProcess(secondProcess)).IsTrue();

			firstProcess.Kill();
			await WaitForExitAsync(firstProcess, cancellationToken);

			await Assert.That(firstProcess.HasExited).IsTrue();
			await Assert.That(secondProcess.HasExited).IsFalse();
		}
		finally
		{
			job.Dispose();
			TerminateIfRunning(firstProcess);
			TerminateIfRunning(secondProcess);
		}
	}

	[Test]
	[Explicit]
	public async Task ProcessCanExitManuallyWhileAssignedToJob(CancellationToken cancellationToken)
	{
		using JobObject job = new();
		using Process process = new();
		process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");
		process.StartInfo.UseShellExecute = true;

		if (!process.Start())
		{
			throw new InvalidOperationException("Unable to start the manual test process.");
		}

		try
		{
			await Assert.That(job.AddProcess(process)).IsTrue();
			await Assert.That(process.HasExited).IsFalse();
			await WaitForExitAsync(process, cancellationToken);
			await Assert.That(process.HasExited).IsTrue();
		}
		finally
		{
			TerminateIfRunning(process);
		}
	}

	[SuppressMessage("ReSharper", "DisposeOnUsingVariable")]
	[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
	[Test]
	public async Task AddProcessOverloadsThrowAfterDispose()
	{
		using JobObject job = new();
		using Process process = CreateLongRunningProcess();
		job.Dispose();

		try
		{
			await Assert.That(() => job.AddProcess(process.SafeHandle)).Throws<ObjectDisposedException>();
			await Assert.That(() => job.AddProcess(process)).Throws<ObjectDisposedException>();
			await Assert.That(() => job.AddProcess(-1)).Throws<ObjectDisposedException>();
		}
		finally
		{
			TerminateIfRunning(process);
		}
	}

	[Test]
	public async Task NativeStructuresMatchWindowsLayout()
	{
		int expectedBasicSize = nint.Size == 8 ? 64 : 48;
		int expectedExtendedSize = nint.Size == 8 ? 144 : 112;

		await Assert.That(Marshal.SizeOf<JobObjectBasicLimitInformation>()).IsEqualTo(expectedBasicSize);
		await Assert.That(Marshal.SizeOf<IoCounters>()).IsEqualTo(48);
		await Assert.That(Marshal.SizeOf<JobObjectExtendedLimitInformation>()).IsEqualTo(expectedExtendedSize);
	}

	private static Process CreateLongRunningProcess()
	{
		ProcessStartInfo startInfo = new()
		{
			FileName = Path.Combine(Environment.SystemDirectory, "PING.EXE"),
			CreateNoWindow = true,
			RedirectStandardError = true,
			RedirectStandardOutput = true,
			UseShellExecute = false,
		};
		startInfo.ArgumentList.Add("-t");
		startInfo.ArgumentList.Add("127.0.0.1");

		Process process = new() { StartInfo = startInfo };
		if (process.Start())
		{
			return process;
		}

		process.Dispose();
		throw new InvalidOperationException("Unable to start the test process.");
	}

	private static (Process First, Process Second) CreateLongRunningProcessPair()
	{
		Process firstProcess = CreateLongRunningProcess();
		try
		{
			return (firstProcess, CreateLongRunningProcess());
		}
		catch
		{
			TerminateIfRunning(firstProcess);
			firstProcess.Dispose();
			throw;
		}
	}

	private static async Task WaitForExitAsync(Process process, CancellationToken cancellationToken)
	{
		await process.WaitForExitAsync(cancellationToken).WaitAsync(ProcessExitTimeout, cancellationToken);
	}

	private static void TerminateIfRunning(Process process)
	{
		if (process.HasExited)
		{
			return;
		}

		try
		{
			process.Kill(entireProcessTree: true);
		}
		catch (InvalidOperationException)
		{
			return;
		}
		catch (Win32Exception)
		{
			if (process.WaitForExit((int)ProcessExitTimeout.TotalMilliseconds))
			{
				return;
			}

			throw;
		}

		if (!process.WaitForExit((int)ProcessExitTimeout.TotalMilliseconds))
		{
			throw new TimeoutException($"Process {process.Id} did not exit during test cleanup.");
		}
	}
}
