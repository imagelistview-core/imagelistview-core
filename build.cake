var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var version = Argument("package-version", "");

var solution = "./Source/ImageListView.sln";
var artifacts = "./artifacts";

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifacts);
    CleanDirectory($"./Source/ImageListView/bin/{configuration}");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    MSBuild(solution, new MSBuildSettings
    {
        Target = "Restore",
        Verbosity = Verbosity.Minimal,
        Configuration = configuration,
    });
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    MSBuild(solution, new MSBuildSettings
    {
        Target = "Build",
        Verbosity = Verbosity.Minimal,
        Configuration = configuration,
    });
});

Task("Pack")
    .IsDependentOn("Restore")
    .Does(context =>
{
    var apiKey = context.EnvironmentVariable("NUGET_API_KEY");

    if (string.IsNullOrWhiteSpace(apiKey))
    {
        throw new CakeException("No NuGet API key specified.");
    }

    if (string.IsNullOrWhiteSpace(version))
    {
        throw new CakeException("No package version specified.");
    }

    string actualVersion = version;

    if (version.StartsWith("v"))
    {
        actualVersion = version.Substring(1);
    }

    MSBuild(solution, s => s
            .SetVerbosity(Verbosity.Minimal)
            .SetConfiguration(configuration)
            .WithProperty("PackageOutputPath", "../../artifacts")
            .WithProperty("GeneratePackageOnBuild", "true")
            .WithProperty("Version", actualVersion)
            .WithTarget("Build")
    );

    var files = GetFiles($"{artifacts}/*.nupkg");

    context.NuGetPush(files, new NuGetPushSettings
    {
        ApiKey = apiKey,
        Source = "https://api.nuget.org/v3/index.json",
    });
});

RunTarget(target);
