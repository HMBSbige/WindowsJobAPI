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
	public class JobObject : IDisposable
	{
		private IntPtr _handle;

		public JobObject()
		{
			_handle = WinApi.CreateJobObject(IntPtr.Zero, null);

			var extendedInfoPtr = IntPtr.Zero;
			var info = new JOBOBJECT_BASIC_LIMIT_INFORMATION
			{
				LimitFlags = 0x2000
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
				if (extendedInfoPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(extendedInfoPtr);
				}
			}
		}

		public bool AddProcess(IntPtr processHandle)
		{
			return WinApi.AssignProcessToJobObject(_handle, processHandle);
		}

		public bool AddProcess(int processId)
		{
			return AddProcess(Process.GetProcessById(processId).Handle);
		}

		#region IDisposable

		private volatile bool _disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (_disposedValue)
			{
				return;
			}

			if (disposing)
			{
				// 释放托管状态(托管对象)
			}

			// 释放未托管的资源(未托管的对象)并替代终结器
			// 将大型字段设置为 null

			if (_handle != IntPtr.Zero)
			{
				WinApi.CloseHandle(_handle);
				_handle = IntPtr.Zero;
			}

			_disposedValue = true;
		}

		~JobObject()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
