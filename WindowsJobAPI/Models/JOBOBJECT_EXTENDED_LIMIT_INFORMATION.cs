using System.Runtime.InteropServices;

namespace WindowsJobAPI.Models
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
	{
		public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
		public IO_COUNTERS IoInfo;
		public nuint ProcessMemoryLimit;
		public nuint JobMemoryLimit;
		public nuint PeakProcessMemoryUsed;
		public nuint PeakJobMemoryUsed;
	}
}
