const string version = "1.1.2";

const string defaultTarget = "Default";
const string solutionPath = "./MugenMvvmToolkit.DryIoc.sln";
const string projectPath = "./MugenMvvmToolkit.DryIoc/MugenMvvmToolkit.DryIoc.csproj";
const string testProjectPath = "./MugenMvvmToolkit.DryIoc.Tests/MugenMvvmToolkit.DryIoc.Tests.csproj";
const string configuration = "Release";

var target = Argument("target", defaultTarget);

var packSettings = new DotNetCorePackSettings
{
    Configuration = configuration,
    IncludeSymbols = true,
    ArgumentCustomization = args => args.Append("/p:PackageVersion=" + version)
};

Task(defaultTarget)
  .Does(() =>
{
  DotNetCoreRestore(solutionPath);
  DotNetCoreBuild(solutionPath);
  DotNetCoreTest(testProjectPath);
  DotNetCorePack(projectPath, packSettings);
});

RunTarget(target);
