namespace WindowsJobAPI.Models;

[StructLayout(LayoutKind.Sequential)]
internal struct JOBOBJECT_BASIC_LIMIT_INFORMATION
{
	public long PerProcessUserTimeLimit;
	public long PerJobUserTimeLimit;
	public JOBOBJECTLIMIT LimitFlags;
	public nuint MinimumWorkingSetSize;
	public nuint MaximumWorkingSetSize;
	public uint ActiveProcessLimit;
	public nuint Affinity;
	public uint PriorityClass;
	public uint SchedulingClass;
}
