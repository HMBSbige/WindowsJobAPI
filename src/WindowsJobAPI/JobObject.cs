namespace WindowsJobAPI;

/// <summary>
/// Groups processes into a Windows Job Object that terminates them when disposed.
/// </summary>
public sealed class JobObject : IDisposable
{
	private static readonly JobObjectExtendedLimitInformation Info = new() { BasicLimitInformation = new JobObjectBasicLimitInformation { LimitFlags = JobObjectLimit.KillOnJobClose } };

	private static readonly unsafe uint InfoLength = (uint)sizeof(JobObjectExtendedLimitInformation);

	private readonly SafeJobHandle _handle;

	public JobObject()
	{
		nint handle = WinApi.CreateJobObject(default, null);
		if (handle == nint.Zero)
		{
			throw new Win32Exception(Marshal.GetLastPInvokeError(), "Unable to create JobObject.");
		}

		_handle = new SafeJobHandle(handle);
		if (WinApi.SetInformationJobObject(_handle, JobObjectInfoType.ExtendedLimitInformation, in Info, InfoLength) is not 0)
		{
			return;
		}

		int error = Marshal.GetLastPInvokeError();
		_handle.Dispose();
		throw new Win32Exception(error, "Unable to set JobObject information.");
	}

	public bool AddProcess(SafeProcessHandle processHandle)
	{
		ThrowIfDisposed();
		ArgumentNullException.ThrowIfNull(processHandle);
		return WinApi.AssignProcessToJobObject(_handle, processHandle) is not 0;
	}

	public bool AddProcess(Process process)
	{
		ThrowIfDisposed();
		ArgumentNullException.ThrowIfNull(process);
		return AddProcess(process.SafeHandle);
	}

	public bool AddProcess(int processId)
	{
		ThrowIfDisposed();
		using Process process = Process.GetProcessById(processId);
		return AddProcess(process);
	}

	public void Dispose()
	{
		_handle.Dispose();
	}

	private void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_handle.IsClosed, this);
	}
}
