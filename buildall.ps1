
 param (
    [Parameter(Mandatory=$true)]
    [string]$testSettings,

    [Parameter(Mandatory=$true)]
    [string]$signKeyPath,

    [switch]$doc = $false
 )

$visualStudioVersion = $env:VisualStudioVersion
$pythonPath = "stone"
$nugetUrl = "http://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$solutionDir = Resolve-Path "dropbox-sdk-dotnet"
$generatorDir = Resolve-Path "generator"
$sourceDir = "$solutionDir\Dropbox.Api"
$testsDir = "$solutionDir\Dropbox.Api.Tests"
$specDir = Resolve-Path "spec"
$nugetDir = "$solutionDir\.nuget"
$nugetPath = "$nugetDir\nuget.exe"
$nugetSpecPath = "$sourceDir\Dropbox.Api.nuspec"
$generatorPath = "$generatorDir\csharp.stoneg.py"
$generatorArgs = @("-m", "stone.cli", "-a:all", $generatorPath, $sourceDir)
$docBuildPath = Resolve-Path "doc\StoneDocs.shfbproj"
$majorVersion = "4.0"
$releaseVersion = "4.2.4"
$assemblyInfoPath = "$sourceDir\AppProperties\AssemblyInfo.cs"

$builds = @(
    @{Name = "Dropbox.Api"; Configuration="Release"; SignAssembly=$true; TestsName="Dropbox.Api.Tests"},
    @{Name = "Dropbox.Api.Portable"; Configuration="Release"; SignAssembly=$true; TestName=$null},
    @{Name = "Dropbox.Api.Portable40"; Configuration="Release"; SignAssembly=$true; TestName=$null},
    @{Name = "Dropbox.Api.Doc"; Configuration="Debug"; SignAssembly=$false; TestName=$null}
)

function RunCommand($command)
{
    return & $command
}

function EnsureVisualStudioVersion()
{
    Write-Host "Checking Visual Studio version..."
    if (!($visualStudioVersion -ge 12))
    {
        Write-Error "This requires a visual studio version 12.0 ""VS2013"" to be installed." -ErrorAction SilentlyContinue
        Write-Error "Did you run vcvarsall.bat?" -ErrorAction Stop
    }
}

function EnsureNuGetExists()
{
  Write-Host "Checking nuget.exe..."
  if (!(Test-Path $nugetPath))
  {
    Write-Host "Couldn't find nuget.exe. Downloading from $nugetUrl to $nugetPath"

    if (!(Test-Path $nugetDir))
    {
        New-Item $nugetDir -ItemType Directory
    }

    (New-Object System.Net.WebClient).DownloadFile($nugetUrl, $nugetPath)
  }
}

function EnsurePythonExists()
{
    Write-Host "Checking Python environment..."
    if (!(Get-Command "python" -ErrorAction SilentlyContinue)) {
        Write-Error "Cannot find python.exe, do you need to check your path?" -ErrorAction Stop
    }
}

function GenerateProjectFiles()
{
    Write-Host "Generating Dropbox.Api..."

    [environment]::SetEnvironmentVariable("PYTHONPATH", $pythonPath)
    $files = Get-ChildItem "$specDir\*" -Include *.stone | Select -ExpandProperty FullName

    RunCommand { python $generatorArgs $files }
}

function RestoreNugetPackages()
{
    Write-Host "Restoring nuget packages..."
    RunCommand  { & $nugetPath restore "$solutionDir\Dropbox.Api.sln" }
}

function RunMSBuild()
{
    param (
        [string] $buildPath,
        [string] $config = "Debug",
        [switch] $signAssembly = $false
    )

    RunCommand { msbuild "/t:Clean;Rebuild" "/verbosity:minimal" "/clp:ErrorsOnly" "/m" $buildPath "/p:Configuration=$config"  "/p:AssemblyOriginatorKeyFile=$signKeyPath" "/p:SignAssembly=$signAssembly"}
}

function BuildPackage($build)
{
    $name = $build.Name
    $config = $build.Configuration
    $testsName = $build.TestsName
    $signAssembly = $build.SignAssembly

    Write-Host "Building $name..."
    RunMSBuild "$sourceDir\$name.csproj" $config -signAssembly $signAssembly

    if ($testsName -ne $null)
    {
        RunVSUnitTests $testsName
    }
}

function BuildPackages()
{
    foreach ($build in $builds)
    {
        BuildPackage $build
    }
}

function RunVSUnitTests($testsName)
{
    Write-Host "Building $testsName..."
    RunMSBuild "$testsDir/$testsName.csproj"
    Write-Host "Running tests $testName..."
    RunCommand { vstest.console "$testsDir\bin\Debug\$testsName.dll" "/Settings:$testSettings" }
}

function BuildDoc()
{
    if ($doc)
    {
        Write-Host "Building Doc..."
        RunMSBuild $docBuildPath
    }
}

function PackNugetPackage()
{
    Write-Host "Packing Nuget package..."
    RunCommand { & $nugetPath pack $nugetSpecPath }
}

function GetAssemblyFileVersion($majorVersion)
{
    $date = Get-Date
    $start = [datetime]"01/01/2000"
    $span = New-TimeSpan -Start $start -End $date
    $minor = [int]($span.TotalDays)

    return "$majorVersion.$minor"

}

function GetAssemblyVersion($majorVersion)
{
    return "$majorVersion.0"
}

function UpdateAssemblyInfo()
{
    $assemblyVersionNumber = GetAssemblyVersion $majorVersion
    $fileVersionNumber = GetAssemblyFileVersion $majorVersion
    Write-Host $fileVersionNumber
    $assemblyVersionPattern = 'AssemblyVersion\(.*\)'
    $fileVersionPattern = 'AssemblyFileVersion\(.*\)'
    $assemblyVersion = 'AssemblyVersion("' + $assemblyVersionNumber + '")';
    $fileVersion = 'AssemblyFileVersion("' + $fileVersionNumber + '")';

    Write-Host "Updating AssemblyInfo.cs..."
    (Get-Content $assemblyInfoPath) | ForEach-Object {
        % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
        % {$_ -replace $fileVersionPattern, $fileVersion }
    } | Set-Content $assemblyInfoPath
}

function UpdateNugetSpec()
{
    Write-Host "Updating nuspec file..."
    $xml = [xml](Get-Content $nugetSpecPath)
    $xml.SelectNodes('/package/metadata/version')[0].InnerXml = $releaseVersion
    $xml.Save($nugetSpecPath)
}

EnsureNuGetExists
EnsurePythonExists
EnsureVisualStudioVersion
GenerateProjectFiles
RestoreNugetPackages
UpdateAssemblyInfo
BuildPackages
BuildDoc
UpdateNugetSpec
PackNugetPackage
