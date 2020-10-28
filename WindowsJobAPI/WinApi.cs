using System;
using System.Runtime.InteropServices;
using WindowsJobAPI.Models;

namespace WindowsJobAPI
{
	internal static class WinApi
	{
		[DllImport(@"kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr CreateJobObject(IntPtr a, string lpName);

		[DllImport(@"kernel32.dll", SetLastError = true)]
		public static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

		[DllImport(@"kernel32.dll", SetLastError = true)]
		public static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

		[DllImport(@"kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr hObject);
	}
}
