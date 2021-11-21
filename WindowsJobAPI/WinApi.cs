namespace WindowsJobAPI;

internal static class WinApi
{
	private const string Kernel32 = @"kernel32.dll";

	[DllImport(Kernel32, EntryPoint = @"CreateJobObjectW", CharSet = CharSet.Unicode)]
	public static extern nint CreateJobObject(nint a, string? lpName);

	[DllImport(Kernel32, SetLastError = true)]
	public static extern bool SetInformationJobObject(SafeJobHandle hJob, JobObjectInfoType infoType, nint lpJobObjectInfo, uint cbJobObjectInfoLength);

	[DllImport(Kernel32)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AssignProcessToJobObject(SafeJobHandle job, SafeProcessHandle process);

	[DllImport(Kernel32, SetLastError = true)]
#if NETSTANDARD
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif
	[SuppressUnmanagedCodeSecurity]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CloseHandle(nint hObject);

	[DllImport(Kernel32,
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
