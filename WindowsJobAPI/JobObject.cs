using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static PInvoke.Kernel32;

namespace WindowsJobAPI
{
	/// <summary>
	/// https://stackoverflow.com/a/9164742
	/// </summary>
	public sealed class JobObject : IDisposable
	{
		private readonly SafeObjectHandle _handle;

		public JobObject()
		{
			_handle = CreateJobObject((nint)0, null);

			nint extendedInfoPtr = 0;

			var info = new JOBOBJECT_BASIC_LIMIT_INFORMATION
			{
				LimitFlags = JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
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

				if (!SetInformationJobObject(_handle, JOBOBJECTINFOCLASS.JobObjectExtendedLimitInformation, extendedInfoPtr, (uint)length))
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

		public bool AddProcess(nint processHandle)
		{
			if (_disposedValue)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			var safeHandle = new SafeObjectHandle(processHandle, false);
			return AssignProcessToJobObject(_handle, safeHandle);
		}

		public bool AddProcess(Process process)
		{
			return AddProcess(process.Handle);
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
