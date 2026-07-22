namespace WindowsJobAPI.Models;

[StructLayout(LayoutKind.Sequential)]
internal struct JobObjectBasicLimitInformation
{
	public long PerProcessUserTimeLimit;
	public long PerJobUserTimeLimit;
	public JobObjectLimit LimitFlags;
	public nuint MinimumWorkingSetSize;
	public nuint MaximumWorkingSetSize;
	public uint ActiveProcessLimit;
	public nuint Affinity;
	public uint PriorityClass;
	public uint SchedulingClass;
}
