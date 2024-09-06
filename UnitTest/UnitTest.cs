using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WindowsJobAPI;

namespace UnitTest;

[TestClass]
public class UnitTest
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Process CreateCmd()
	{
		var process = new Process { StartInfo = { UseShellExecute = false, FileName = @"cmd" } };
		process.Start();
		return process;
	}

	[TestMethod]
	public void TestDispose()
	{
		var process = CreateCmd();

		var job = new JobObject();
		Assert.IsTrue(job.AddProcess(process));

		Assert.IsFalse(process.HasExited);

		job.Dispose();
		Assert.IsTrue(process.HasExited);
	}

	[TestMethod]
	public void TestAddSafeHandle()
	{
		var process = CreateCmd();

		var job = new JobObject();
		Assert.IsTrue(job.AddProcess(process.SafeHandle));

		Assert.IsFalse(process.HasExited);

		job.Dispose();
		Assert.IsTrue(process.HasExited);
	}

	[TestMethod]
	public void TestAddId()
	{
		var process = CreateCmd();

		var job = new JobObject();
		Assert.IsTrue(job.AddProcess(process.Id));

		Assert.IsFalse(process.HasExited);

		job.Dispose();
		Assert.IsTrue(process.HasExited);
	}

	[TestMethod]
	public void TestInstance()
	{
		var process = CreateCmd();
		var job = new JobObject();
		Assert.IsTrue(job.AddProcess(process));
		Assert.IsFalse(process.HasExited);

		var process2 = CreateCmd();
		var job2 = new JobObject();
		Assert.IsTrue(job2.AddProcess(process2));
		Assert.IsFalse(process2.HasExited);
		job2.Dispose();
		Assert.IsTrue(process2.HasExited);

		Assert.IsFalse(process.HasExited);
		job.Dispose();
		Assert.IsTrue(process.HasExited);
	}

	[TestMethod]
	public void TestManualKillChildProcess()
	{
		var process = CreateCmd();
		var process2 = CreateCmd();

		using var job = new JobObject();
		Assert.IsTrue(job.AddProcess(process));
		Assert.IsTrue(job.AddProcess(process2));

		Assert.IsFalse(process.HasExited);
		Assert.IsFalse(process2.HasExited);

		process.Kill();

		Assert.IsTrue(process.HasExited);
		Assert.IsFalse(process2.HasExited);
	}

#if !DEBUG
	[Ignore]
#endif
	[TestMethod]
	public void TestManualKillParentProcess()
	{
		var process = new Process { StartInfo = { UseShellExecute = true, FileName = @"cmd" } };
		process.Start();

		/*using*/
		var job = new JobObject();
		Assert.IsTrue(job.AddProcess(process));
		Assert.IsFalse(process.HasExited);
		process.WaitForExit((int)TimeSpan.FromSeconds(5).TotalMilliseconds);
		Assert.IsTrue(process.HasExited);
	}
}
