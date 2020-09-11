
 param (
    [Parameter(Mandatory=$true)]
    [string]$testSettings,

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
$docBuildPath = Resolve-Path "doc\StoneDocs.shfbproj"
$majorVersion = "5.0"
$releaseVersion = "5.1.0"
$assemblyInfoPath = "$sourceDir\AppProperties\AssemblyInfo.cs"
$signKeyPath = "$sourceDir\dropbox_api_key.snk"
$releaseNotes = @'
Change Notes:
- Fix bug to add support for short-lived tokens to team client
- Fix typo in DropboxRequestHandler
'@

$builds = @(
    @{Name = "Dropbox.Api"; Configuration="Release"; SignAssembly=$true; TestsName="Dropbox.Api.Tests"})

function RunCommand($command)
{
    return & $command
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

function UpdateNugetSpec()
{
    Write-Host "Updating nuspec file..."
    $xml = [xml](Get-Content $nugetSpecPath)
    $xml.SelectNodes('/package/metadata/version')[0].InnerXml = $releaseVersion
    $xml.SelectNodes('/package/metadata/releaseNotes')[0].InnerXml = "`n" + $releaseNotes + "`n    "
    $xml.Save($nugetSpecPath)
}

EnsureNuGetExists
RestoreNugetPackages
BuildPackages
BuildDoc
UpdateNugetSpec
PackNugetPackage
