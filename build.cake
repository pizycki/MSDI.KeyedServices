//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var sln = File("./MS.DI.KeyedServices.sln");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() => {
        CleanDirectories("./src/**/netcoreapp*");
        CleanDirectories("./tests/**/netcoreapp*");
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() => {
        NuGetRestore(sln);
    });

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
        if(IsRunningOnWindows()) {
            // Use MSBuild
            MSBuild(sln, settings =>
                settings.SetConfiguration(configuration));
        }
        else {
            // Use XBuild
            XBuild(sln, settings =>
                settings.SetConfiguration(configuration));
        }
    });

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetCoreTest("./tests/MS.DI.KeyedServices.Tests/MS.DI.KeyedServices.Tests.csproj");
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
