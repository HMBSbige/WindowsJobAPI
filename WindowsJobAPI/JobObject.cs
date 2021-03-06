using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WindowsJobAPI.Models;

namespace WindowsJobAPI
{
	/// <summary>
	/// https://stackoverflow.com/a/9164742
	/// </summary>
	public sealed class JobObject : IDisposable
	{
		private readonly SafeJobHandle _handle;

		public JobObject()
		{
			_handle = new SafeJobHandle(WinApi.CreateJobObject(0, null));

			nint extendedInfoPtr = 0;

			var info = new JOBOBJECT_BASIC_LIMIT_INFORMATION
			{
				LimitFlags = JOBOBJECTLIMIT.KillOnJobClose
			};

			var extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
			{
				BasicLimitInformation = info
			};

			try
			{
				var length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
				extendedInfoPtr = Marshal.AllocHGlobal(length);
				Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

				if (!WinApi.SetInformationJobObject(_handle, JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)length))
				{
					throw new Win32Exception($@"Unable to set information. Error: {Marshal.GetLastWin32Error()}");
				}
			}
			finally
			{
				if (extendedInfoPtr != 0)
				{
					Marshal.FreeHGlobal(extendedInfoPtr);
				}
			}
		}

		#region AddProcess

		public bool AddProcess(SafeProcessHandle processHandle)
		{
			if (_disposedValue)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			return WinApi.AssignProcessToJobObject(_handle, processHandle);
		}

		public bool AddProcess(Process process)
		{
			return AddProcess(process.SafeHandle);
		}

		public bool AddProcess(int processId)
		{
			using var process = Process.GetProcessById(processId);
			return AddProcess(process);
		}

		#endregion

		#region IDisposable

		private volatile bool _disposedValue;

		public void Dispose()
		{
			if (_disposedValue)
			{
				return;
			}

			_handle.Dispose();

			_disposedValue = true;
		}

		#endregion
	}
}
