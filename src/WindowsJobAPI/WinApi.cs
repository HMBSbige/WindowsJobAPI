namespace WindowsJobAPI;

internal static partial class WinApi
{
	private const string Kernel32 = "kernel32.dll";

	[LibraryImport(Kernel32, EntryPoint = "CreateJobObjectW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	public static partial nint CreateJobObject(nint jobAttributes, string? name);

	[LibraryImport(Kernel32, SetLastError = true)]
	public static partial int SetInformationJobObject(SafeJobHandle job, JobObjectInfoType infoType, in JobObjectExtendedLimitInformation jobObjectInfo, uint jobObjectInfoLength);

	[LibraryImport(Kernel32, SetLastError = true)]
	public static partial int AssignProcessToJobObject(SafeJobHandle job, SafeProcessHandle process);

	[LibraryImport(Kernel32, SetLastError = true)]
	public static partial int CloseHandle(nint handle);

	[LibraryImport(Kernel32, SetLastError = true)]
	public static partial int TerminateJobObject(nint job, uint exitCode);
}
