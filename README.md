# WindowsJobAPI

[![NuGet](https://img.shields.io/nuget/v/WindowsJobAPI.svg?logo=nuget)](https://www.nuget.org/packages/WindowsJobAPI/)

A small .NET library for grouping processes in a Windows Job Object. Disposing the job terminates every process assigned to it.

## Usage

```csharp
using System.Diagnostics;
using WindowsJobAPI;

using Process process = Process.Start(new ProcessStartInfo
{
    FileName = "cmd.exe",
    UseShellExecute = false,
}) ?? throw new InvalidOperationException("Unable to start the process.");

using JobObject job = new();
if (!job.AddProcess(process))
{
    throw new InvalidOperationException("Unable to add the process to the job.");
}

// Disposing job terminates process and any child processes in the same job.
```

## License

[MIT](LICENSE)
