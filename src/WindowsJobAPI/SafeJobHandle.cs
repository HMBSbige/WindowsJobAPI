namespace WindowsJobAPI;

public sealed class SafeJobHandle : SafeHandleZeroOrMinusOneIsInvalid
{
	public SafeJobHandle(nint handle) : base(true)
	{
		SetHandle(handle);
	}

	protected override bool ReleaseHandle()
	{
		_ = WinApi.TerminateJobObject(handle, 0);
		return WinApi.CloseHandle(handle) is not 0;
	}
}
