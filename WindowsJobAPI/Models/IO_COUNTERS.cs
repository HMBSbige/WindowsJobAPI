﻿using System.Runtime.InteropServices;

namespace WindowsJobAPI.Models
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct IO_COUNTERS
	{
		public ulong ReadOperationCount;
		public ulong WriteOperationCount;
		public ulong OtherOperationCount;
		public ulong ReadTransferCount;
		public ulong WriteTransferCount;
		public ulong OtherTransferCount;
	}
}