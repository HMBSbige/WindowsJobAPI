using Microsoft.Win32.SafeHandles;
using System;

namespace WindowsJobAPI
{
	public sealed class SafeJobHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeJobHandle(IntPtr handle) : base(true)
		{
			SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			WinApi.TerminateJobObject(handle, 0);

			return WinApi.CloseHandle(handle);
		}
	}
}