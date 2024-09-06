using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using WindowsJobAPI;

namespace UnitTest;

[TestClass]
public class UnitTest
{
	private static Process CreateCmd()
	{
		Process process = new() { StartInfo = { UseShellExecute = false, FileName = @"cmd" } };
		process.Start();
		return process;
	}

	[TestMethod]
	public void TestDispose()
	{
		Process process = CreateCmd();

		JobObject job = new();
		Assert.IsTrue(job.AddProcess(process));

		Assert.IsFalse(process.HasExited);

		job.Dispose();
		Assert.IsTrue(process.HasExited);
	}

	[TestMethod]
	public void TestAddSafeHandle()
	{
		Process process = CreateCmd();

		JobObject job = new();
		Assert.IsTrue(job.AddProcess(process.SafeHandle));

		Assert.IsFalse(process.HasExited);

		job.Dispose();
		Assert.IsTrue(process.HasExited);
	}

	[TestMethod]
	public void TestAddId()
	{
		Process process = CreateCmd();

		JobObject job = new();
		Assert.IsTrue(job.AddProcess(process.Id));

		Assert.IsFalse(process.HasExited);

		job.Dispose();
		Assert.IsTrue(process.HasExited);
	}

	[TestMethod]
	public void TestInstance()
	{
		Process process = CreateCmd();
		JobObject job = new();
		Assert.IsTrue(job.AddProcess(process));
		Assert.IsFalse(process.HasExited);

		Process process2 = CreateCmd();
		JobObject job2 = new();
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
		Process process = CreateCmd();
		Process process2 = CreateCmd();

		using JobObject job = new();
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
		Process process = new() { StartInfo = { UseShellExecute = true, FileName = @"cmd" } };
		process.Start();

		/*using*/
		JobObject job = new();
		Assert.IsTrue(job.AddProcess(process));
		Assert.IsFalse(process.HasExited);
		process.WaitForExit((int)TimeSpan.FromSeconds(5).TotalMilliseconds);
		Assert.IsTrue(process.HasExited);
	}
}
