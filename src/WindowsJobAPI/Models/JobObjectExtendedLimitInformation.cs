namespace WindowsJobAPI.Models;

[StructLayout(LayoutKind.Sequential)]
internal struct JobObjectExtendedLimitInformation
{
	public JobObjectBasicLimitInformation BasicLimitInformation;
	public IoCounters IoInfo;
	public nuint ProcessMemoryLimit;
	public nuint JobMemoryLimit;
	public nuint PeakProcessMemoryUsed;
	public nuint PeakJobMemoryUsed;
}
