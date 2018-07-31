//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var sln = File("./MSDI.KeyedServices.sln");
var artifacts = Directory("./artifacts");

//////////////////////////////////////////////////////////////////////
// Methods
//////////////////////////////////////////////////////////////////////

void RestoreNugets() => NuGetRestore(sln);

void BuildOnWindows() => MSBuild(sln, settings => settings.SetConfiguration(configuration));

void BuildWithMono() => XBuild(sln, settings => settings.SetConfiguration(configuration));

void RunUnitTests() => DotNetCoreTest("./tests/MSDI.KeyedServices.Tests/MSDI.KeyedServices.Tests.csproj");

void CreateNuget() {
    DotNetCorePack("./src/MSDI.KeyedServices",
        new DotNetCorePackSettings {
            Configuration = "Release",
            OutputDirectory = artifacts.Path,
            NoBuild = true
        });
}

void PushPackageToNuget() {

    var packageFiles = GetFiles(artifacts.Path + "/*.nupkg");
    Information("Found {0} .nupkg file(s).", packageFiles.Count());
    foreach (var nupkg in packageFiles)
    {       
        DotNetCoreNuGetPush(nupkg.FullPath,
            new DotNetCoreNuGetPushSettings {
                Source = "https://www.nuget.org/api/v2/package/",
                ApiKey = GetNugetApiKey()
            });
    }

}

string GetNugetApiKey() => EnvironmentVariable("MS_DI_KEYED_SERVICES_NUGET_APIKEY");

void CleanArtifactsDirectory() => CleanDirectory(artifacts.Path);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() => {
        CleanDirectories("./src/**/netcoreapp*");
        CleanDirectories("./tests/**/netcoreapp*");
    });

Task("Restore-NuGet-Packages").Does(RestoreNugets);

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
        if (IsRunningOnWindows()) BuildOnWindows();
        else BuildWithMono();
    });

Task("Run-Unit-Tests").Does(RunUnitTests);

Task("Run-Tests")
    .IsDependentOn("Run-Unit-Tests");

Task("Create-Nuget-Package").Does(CreateNuget);

Task("Push-Package-ToNuget").Does(PushPackageToNuget);

Task("Print-Nuget-ApiKey").Does(() => Information(GetNugetApiKey()));

Task("Clean-Artifacts").Does(CleanArtifactsDirectory);

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Rebuild")
    .IsDependentOn("Clean")
    .IsDependentOn("Build");

Task("Default")
    .IsDependentOn("Rebuild")
    .IsDependentOn("Run-Unit-Tests");

Task("Publish-ToNuget")
    .IsDependentOn("Rebuild")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Clean-Artifacts")
    .IsDependentOn("Create-Nuget-Package")
    .IsDependentOn("Push-Package-ToNuget");

Task("Publish-ToNuget-AppVeyor")    
    .IsDependentOn("Clean-Artifacts")
    .IsDependentOn("Create-Nuget-Package")
    .IsDependentOn("Push-Package-ToNuget");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
