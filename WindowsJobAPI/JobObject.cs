namespace WindowsJobAPI;

/// <summary>
/// https://stackoverflow.com/a/9164742
/// </summary>
public sealed class JobObject : IDisposable
{
	private static readonly JOBOBJECT_EXTENDED_LIMIT_INFORMATION Info;
	private static readonly int InfoLength;

	static JobObject()
	{
		JOBOBJECT_BASIC_LIMIT_INFORMATION info = new()
		{
			LimitFlags = JOBOBJECTLIMIT.KillOnJobClose
		};

		Info = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
		{
			BasicLimitInformation = info
		};

		InfoLength = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
	}

	private readonly SafeJobHandle _handle;

	public JobObject()
	{
		_handle = new SafeJobHandle(WinApi.CreateJobObject(default, default));

		nint extendedInfoPtr = default;
		try
		{
			extendedInfoPtr = Marshal.AllocCoTaskMem(InfoLength);
			Marshal.StructureToPtr(Info, extendedInfoPtr, false);

			if (!WinApi.SetInformationJobObject(_handle, JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)InfoLength))
			{
				throw new Win32Exception($@"Unable to set JobObject information. Error: {Marshal.GetLastWin32Error()}");
			}
		}
		finally
		{
			Marshal.FreeCoTaskMem(extendedInfoPtr);
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
		using Process process = Process.GetProcessById(processId);
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
