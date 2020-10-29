# WindowsJobAPI

Channel | Status
-|-
Build | [![GitHub CI](https://github.com/HMBSbige/WindowsJobAPI/workflows/GitHub%20CI/badge.svg)](https://github.com/HMBSbige/WindowsJobAPI/actions)
NuGet.org | [![NuGet.org](https://img.shields.io/nuget/v/WindowsJobAPI.svg)](https://www.nuget.org/packages/WindowsJobAPI/)

# Usage
```csharp
var process = new Process
{
    StartInfo =
    {
        UseShellExecute = true,
        FileName = @"cmd"
    }
};
process.Start();

var job = new JobObject();

job.AddProcess(process);
//job.AddProcess(process.SafeHandle);
//job.AddProcess(process.Id);

job.Dispose();
```
