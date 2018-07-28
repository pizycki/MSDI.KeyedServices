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
// Methods
//////////////////////////////////////////////////////////////////////

void BuildOnWindows() => MSBuild(sln, settings => settings.SetConfiguration(configuration));

void BuildWithMono() => XBuild(sln, settings => settings.SetConfiguration(configuration));

void RunUnitTests() => DotNetCoreTest("./tests/MS.DI.KeyedServices.Tests/MS.DI.KeyedServices.Tests.csproj");

void CreateNuget() {
    DotNetCorePack("./src/MS.DI.KeyedServices",
        new DotNetCorePackSettings {
            Configuration = "Release",
            OutputDirectory = "./artifacts/",
            NoBuild = true
        });
}

void PublishNuget() {
    DotNetCoreNuGetPush("./artifacts/MS.DI.KeyedServices.*.nupkg",
        new DotNetCoreNuGetPushSettings {
            Source = "https://www.nuget.org/api/v2/package/",
            ApiKey = GetNugetApiKey()
        });
}

string GetNugetApiKey() => EnvironmentVariable("MS_DI_KEYED_SERVICES_NUGET_APIKEY");

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
    .Does(() => NuGetRestore(sln));

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
        if (IsRunningOnWindows()) BuildOnWindows();
        else BuildWithMono();
    });

Task("Run-Unit-Tests").Does(RunUnitTests);

Task("Create-Nuget").Does(CreateNuget);

Task("Publish-Nuget").Does(PublishNuget);

Task("Print-Nuget-ApiKey").Does(() => Information(GetNugetApiKey()));

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
