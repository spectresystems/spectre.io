var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

////////////////////////////////////////////////////////////////
// Tasks

Task("Build")
    .Does(context => 
{
    DotNetBuild("./src/Spectre.IO.slnx", new DotNetBuildSettings {
        Configuration = configuration,
        NoIncremental = context.HasArgument("rebuild"),
        MSBuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(context => 
{
    DotNetTest("./src/Spectre.IO.Tests/Spectre.IO.Tests.csproj", new DotNetTestSettings {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
    });
});

Task("Package")
    .IsDependentOn("Test")
    .Does(context => 
{
    var outputPath = MakeAbsolute(Directory("./.artifacts"));

    context.CleanDirectory(outputPath.FullPath);
    context.DotNetPack($"./src/Spectre.IO.slnx", new DotNetPackSettings {
        ArgumentCustomization = args => args.Append($"--property:PackageOutputPath={outputPath.FullPath}"),
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
        MSBuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
            .WithProperty("SymbolPackageFormat", "snupkg")
    });
});

Task("Publish-NuGet")
    .WithCriteria(ctx => BuildSystem.IsRunningOnGitHubActions, "Not running on GitHub Actions")
    .IsDependentOn("Package")
    .Does(context => 
{
    var apiKey = Argument<string>("nuget-key", null);
    if(string.IsNullOrWhiteSpace(apiKey)) {
        throw new CakeException("No NuGet API key was provided.");
    }

    // Publish to GitHub Packages
    foreach(var file in context.GetFiles("./.artifacts/*.nupkg")) 
    {
        context.Information("Publishing {0}...", file.GetFilename().FullPath);
        DotNetNuGetPush(file.FullPath, new DotNetNuGetPushSettings
        {
            Source = "https://api.nuget.org/v3/index.json",
            ApiKey = apiKey,
        });
    }
});

////////////////////////////////////////////////////////////////
// Targets

Task("Publish")
    .IsDependentOn("Publish-NuGet");

Task("Default")
    .IsDependentOn("Package");

////////////////////////////////////////////////////////////////
// Execution

RunTarget(target)