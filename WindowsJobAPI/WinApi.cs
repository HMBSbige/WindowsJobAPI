using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using WindowsJobAPI.Models;

namespace WindowsJobAPI
{
	internal static class WinApi
	{
		[DllImport(@"kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern nint CreateJobObject(nint a, string? lpName);

		[DllImport(@"kernel32.dll")]
		public static extern bool SetInformationJobObject(SafeJobHandle hJob, JobObjectInfoType infoType, nint lpJobObjectInfo, uint cbJobObjectInfoLength);

		[DllImport(@"kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AssignProcessToJobObject(SafeJobHandle job, SafeProcessHandle process);

		[DllImport(@"kernel32.dll", SetLastError = true)]
#if NETSTANDARD
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(nint hObject);

		[DllImport(@"kernel32.dll",
				EntryPoint = @"TerminateJobObject",
				CharSet = CharSet.Unicode,
				ExactSpelling = true,
				SetLastError = true)]
#if NETSTANDARD
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif
		[SuppressUnmanagedCodeSecurity]
		public static extern bool TerminateJobObject(
				[In] nint hJob,
				[In] uint uExitCode);
	}
}
