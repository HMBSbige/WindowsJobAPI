namespace WindowsJobAPI;

public sealed class SafeJobHandle : SafeHandleZeroOrMinusOneIsInvalid
{
	public SafeJobHandle(nint handle) : base(true)
	{
		SetHandle(handle);
	}

	protected override bool ReleaseHandle()
	{
		WinApi.TerminateJobObject(handle, 0);

		return WinApi.CloseHandle(handle);
	}
}
