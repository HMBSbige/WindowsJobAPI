using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using WindowsJobAPI.Models;

namespace WindowsJobAPI
{
	internal static class WinApi
	{
		[DllImport(@"kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr CreateJobObject(IntPtr a, string? lpName);

		[DllImport(@"kernel32.dll", SetLastError = true)]
		public static extern bool SetInformationJobObject(SafeJobHandle hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

		[DllImport(@"kernel32.dll", SetLastError = true)]
		public static extern bool AssignProcessToJobObject(SafeJobHandle job, SafeProcessHandle process);

		[DllImport(@"kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern bool CloseHandle(IntPtr hObject);

		[DllImport(@"kernel32.dll",
				EntryPoint = @"TerminateJobObject",
				CharSet = CharSet.Unicode,
				ExactSpelling = true,
				SetLastError = true)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SuppressUnmanagedCodeSecurity]
		public static extern bool TerminateJobObject(
				[In] IntPtr hJob,
				[In] uint uExitCode);
	}
}
