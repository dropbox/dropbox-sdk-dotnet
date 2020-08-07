
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
$majorVersion = "4.0"
$releaseVersion = "4.10.0"
$assemblyInfoPath = "$sourceDir\AppProperties\AssemblyInfo.cs"
$signKeyPath = "$sourceDir\dropbox_api_key.snk"
$releaseNotes = @'
Change Notes:
- Move officially supported versions to Net45 and NetStandard2.0
- Migrate from VS2015 to VS2017 build system
- Move all build targets into single CSProj file

- Add support for PKCE oauth flow
- Add PKCE Oauth Example
- Add System.Security.Cryptography as dependency

(49cee186b12822462483dbca08dfb1335f55efbf)
Files Namespace
  - Add internal_error to SearchError union.
  - Add locked to LookupError union.
  - Add cant_move_into_vault to RelocationError union.
  - Add MoveIntoVaultError union.
  - Add SearchMatchFieldOptions struct.
  - Add optional match_field_options to SearchV2Arg struct.
  - Doc/example changes.

Sharing Namespace
  - Add is_vault to SharePathError union.
  - Add invalid_shared_folder to AddFolderMemberError union.

Team Namespace
  - Make members field of LegalHoldsPolicyUpdateArg struct optional.
  - Add app_folder_removal_not_supported to RevokeLinkedAppError union.
  - Doc/example changes.

Team Log Namespace
  - Add auto_approve to InviteMethod union.
  - Add moved_from_another_team to InviteMethod union.
  - Add moved_from_another_team to MemberStatus union.
  - Add no_one to SharedLinkVisibility union.
  - Add optional new_team to MemberChangeStatusDetails struct.
  - Add optional previous_team to MemberChangeStatusDetails struct.
  - Add external_sharing_create_report_details to EventDetails union.
  - Add external_sharing_report_failed to EventDetails union.
  - Add content_administration_policy_changed_details to EventDetails
union.
  - Add external_sharing_create_report to EventType union.
  - Add external_sharing_report_failed to EventType union.
  - Add content_administration_policy_changed to EventType union.
  - Add send_for_signature_policy_changed_details to EventDetails union.
  - Add external_sharing_create_report to EventTypeArgs union.
  - Add external_sharing_report_failed to EventTypeArgs union.
  - Add content_administration_policy_changed to EventTypeArgs union.
  - Add send_for_signature_policy_changed to EventTypeArgs union.
  - Add SendForSignaturePolicy union.
  - Add ExternalSharingCreateReportDetails struct.
  - Add ExternalSharingReportFailedDetails struct.
  - Add ContentAdministrationPolicyChangedDetails struct.
  - Add SendForSignaturePolicyChangedDetails struct.
  - Add ExternalSharingCreateReportType struct.
  - Add ExternalSharingReportFailedType struct.
  - Add ContentAdministrationPolicyChangedType struct.
  - Add SendForSignaturePolicyChangedType struct.
'@

$builds = @(
    @{Name = "Dropbox.Api"; Configuration="Release"; SignAssembly=$true; TestsName="Dropbox.Api.Tests"})

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

function EnsurePipExists()
{
    Write-Host "Checking pip..."
    if (!(Get-Command "pip" -ErrorAction SilentlyContinue)) {
        Write-Error "Cannot find pip.exe, do you need to check your path?" -ErrorAction Stop
    }
}

function InstallStoneDependency()
{
    RunCommand { pip "install" "ply>=3.4" "six>=1.3.0" "typing>=3.5.2" }
}

function GenerateProjectFiles()
{
    Write-Host "Generating Dropbox.Api..."

    [environment]::SetEnvironmentVariable("PYTHONPATH", $pythonPath)

    RunCommand { python 'generate.py' }
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
    $xml.SelectNodes('/package/metadata/releaseNotes')[0].InnerXml = "`n" + $releaseNotes + "`n    "
    $xml.Save($nugetSpecPath)
}

EnsureNuGetExists
EnsurePythonExists
EnsurePipExists
InstallStoneDependency
EnsureVisualStudioVersion
GenerateProjectFiles
RestoreNugetPackages
UpdateAssemblyInfo
BuildPackages
BuildDoc
UpdateNugetSpec
PackNugetPackage
