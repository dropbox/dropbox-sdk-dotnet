from __future__ import unicode_literals

from StringIO import StringIO


COMPILE_INCLUDES = [
    "Stone\\Decoder.cs",
    "Stone\\Empty.cs",
    "Stone\\Encoder.cs",
    "Stone\\IEncoder.cs",
    "Stone\\IDecoder.cs",
    "Stone\\IJsonReader.cs",
    "Stone\\IJsonWriter.cs",
    "Stone\\ITransport.cs",
    "Stone\\JsonReader.cs",
    "Stone\\JsonWriter.cs",
    "ApiException.cs",
    "StructuredException.cs",
    "Stone\\Util.cs",
    "DropboxCertHelper.cs",
    "DropboxClient.cs",
    "DropboxClientBase.cs",
    "DropboxAppClient.cs",
    "DropboxTeamClient.cs",
    "DropboxClientConfig.cs",
    "DropboxException.cs",
    "DropboxOauth2Helper.cs",
    "DropboxRequestHandler.cs",
    "AppProperties\\AssemblyInfo.cs",
]

NONE_INCLUDES = [
    "packages.config",        
]

PORTABLE40_NONE_INCLUDES = [
    "app.config",
    "packages.Dropbox.Api.Portable40.config",        
]

PORTABLE_NONE_INCLUDES = [
    "packages.Dropbox.Api.Portable.config",        
]

DOC_NONE_INCLUDES = [
    "packages.config",        
]

CSPROJ_START_BLOCK = r"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>AppProperties</AppDesignerFolder>
    <RootNamespace>Dropbox.Api</RootNamespace>
    <AssemblyName>Dropbox.Api</AssemblyName>
    <TargetFrameworks>net45;netstandard2.0</TargetFrameworks>
    <BaseIntermediateOutputPath>obj\</BaseIntermediateOutputPath>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>

  <ItemGroup Condition=" '$(TargetFramework)' == 'net45' ">
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="System.Xml.Linq" />
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="System.Net.Http" />
    <Reference Include="Microsoft.CSharp" />
    <Reference Include="System.Data" />
    <Reference Include="System.Xml" />
    <Reference Include="Newtonsoft.Json">
      <HintPath>..\packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
    </Reference>
  </ItemGroup>  
  <ItemGroup Condition=" '$(TargetFramework)' == 'net45' ">
    <None Include="packages.config" />
  </ItemGroup>

<ItemGroup Condition=" '$(TargetFramework)' == 'netstandard2.0' ">
    <None Remove="app.config" />
    <None Remove="Dropbox.Api.nuspec" />
    <None Remove="dropbox_api_key.snk" />
    <None Remove="packages.config" />
    <None Remove="packages.Dropbox.Api.Portable.config" />
    <None Remove="packages.Dropbox.Api.Portable40.config" />
    <None Remove="Settings.StyleCop" />
    <None Remove="stone_summaries.xml" />
  </ItemGroup>

  <ItemGroup Condition=" '$(TargetFramework)' == 'netstandard2.0' ">
    <PackageReference Include="Newtonsoft.Json" Version="10.0.3" />
  </ItemGroup>
"""

CSPROJ_END_BLOCK = r"""
</Project>
"""

PORTABLE40_CSPROJ_START_BLOCK = r"""<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{D7B167CE-3AF8-478E-82F2-684D38F1DF98}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>AppProperties</AppDesignerFolder>
    <RootNamespace>Dropbox.Api</RootNamespace>
    <AssemblyName>Dropbox.Api</AssemblyName>
    <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
    <TargetFrameworkProfile>Profile344</TargetFrameworkProfile>
    <FileAlignment>512</FileAlignment>
    <ProjectTypeGuids>{786C830F-07A1-408B-BD7F-6EE04809D6DB};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
    <MinimumVisualStudioVersion>10.0</MinimumVisualStudioVersion>
    <BaseIntermediateOutputPath>portable40obj\</BaseIntermediateOutputPath>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\portable40</OutputPath>
    <DefineConstants>DEBUG;TRACE;PORTABLE40</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <DocumentationFile>bin\Debug\portable40\Dropbox.Api.XML</DocumentationFile>
    <RunCodeAnalysis>true</RunCodeAnalysis>
    <NoWarn>419</NoWarn>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\portable40</OutputPath>
    <DefineConstants>TRACE;PORTABLE40</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <DocumentationFile>bin\Release\portable40\Dropbox.Api.XML</DocumentationFile>
    <NoWarn>419</NoWarn>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="Microsoft.Threading.Tasks">
      <HintPath>..\packages\Microsoft.Bcl.Async.1.0.168\lib\portable-net40+sl4+win8+wp71+wpa81\Microsoft.Threading.Tasks.dll</HintPath>
    </Reference>
    <Reference Include="Microsoft.Threading.Tasks.Extensions">
      <HintPath>..\packages\Microsoft.Bcl.Async.1.0.168\lib\portable-net40+sl4+win8+wp71+wpa81\Microsoft.Threading.Tasks.Extensions.dll</HintPath>
    </Reference>
    <Reference Include="System.IO">
      <HintPath>..\packages\Microsoft.Bcl.1.1.10\lib\portable-net40+sl5+win8+wp8+wpa81\System.IO.dll</HintPath>
    </Reference>
    <Reference Include="System.Net.Http">
      <HintPath>..\packages\Microsoft.Net.Http.2.2.29\lib\portable-net40+sl4+win8+wp71+wpa81\System.Net.Http.dll</HintPath>
    </Reference>
    <Reference Include="System.Net.Http.Extensions">
      <HintPath>..\packages\Microsoft.Net.Http.2.2.29\lib\portable-net40+sl4+win8+wp71+wpa81\System.Net.Http.Extensions.dll</HintPath>
    </Reference>
    <Reference Include="System.Net.Http.Primitives">
      <HintPath>..\packages\Microsoft.Net.Http.2.2.29\lib\portable-net40+sl4+win8+wp71+wpa81\System.Net.Http.Primitives.dll</HintPath>
    </Reference>
    <Reference Include="System.Runtime">
      <HintPath>..\packages\Microsoft.Bcl.1.1.10\lib\portable-net40+sl5+win8+wp8+wpa81\System.Runtime.dll</HintPath>
    </Reference>
    <Reference Include="System.Threading.Tasks">
      <HintPath>..\packages\Microsoft.Bcl.1.1.10\lib\portable-net40+sl5+win8+wp8+wpa81\System.Threading.Tasks.dll</HintPath>
    </Reference>
    <Reference Include="Newtonsoft.Json">
      <HintPath>..\packages\Newtonsoft.Json.7.0.1\lib\portable-net40+sl5+wp80+win8+wpa81\Newtonsoft.Json.dll</HintPath>
    </Reference>
  </ItemGroup>
"""

PORTABLE40_CSPROJ_END_BLOCK = r"""  <Import Project="$(MSBuildExtensionsPath32)\Microsoft\Portable\$(TargetFrameworkVersion)\Microsoft.Portable.CSharp.targets" />
  <Import Project="..\packages\Microsoft.Bcl.Build.1.0.14\tools\Microsoft.Bcl.Build.targets" Condition="Exists('..\packages\Microsoft.Bcl.Build.1.0.14\tools\Microsoft.Bcl.Build.targets')" />
  <Target Name="EnsureBclBuildImported" BeforeTargets="BeforeBuild" Condition="'$(BclBuildImported)' == ''">
    <Error Condition="!Exists('..\packages\Microsoft.Bcl.Build.1.0.14\tools\Microsoft.Bcl.Build.targets')" Text="This project references NuGet package(s) that are missing on this computer. Enable NuGet Package Restore to download them.  For more information, see http://go.microsoft.com/fwlink/?LinkID=317567." HelpKeyword="BCLBUILD2001" />
    <Error Condition="Exists('..\packages\Microsoft.Bcl.Build.1.0.14\tools\Microsoft.Bcl.Build.targets')" Text="The build restored NuGet packages. Build the project again to include these packages in the build. For more information, see http://go.microsoft.com/fwlink/?LinkID=317568." HelpKeyword="BCLBUILD2002" />
  </Target>
  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
       Other similar extension points exist, see Microsoft.Common.targets.
  <Target Name="BeforeBuild">
  </Target>
  <Target Name="AfterBuild">
  </Target>
  -->
</Project>
"""

PORTABLE_CSPROJ_START_BLOCK = r"""<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{786C830F-07A1-408B-BD7F-6EE04809D6DB}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>AppProperties</AppDesignerFolder>
    <RootNamespace>Dropbox.Api</RootNamespace>
    <AssemblyName>Dropbox.Api</AssemblyName>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
    <TargetFrameworkProfile>Profile111</TargetFrameworkProfile>
    <FileAlignment>512</FileAlignment>
    <ProjectTypeGuids>{786C830F-07A1-408B-BD7F-6EE04809D6DB};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
    <MinimumVisualStudioVersion>11.0</MinimumVisualStudioVersion>
    <BaseIntermediateOutputPath>portableobj\</BaseIntermediateOutputPath>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\portable</OutputPath>
    <DefineConstants>DEBUG;TRACE;PORTABLE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <DocumentationFile>bin\Debug\portable\Dropbox.Api.XML</DocumentationFile>
    <RunCodeAnalysis>true</RunCodeAnalysis>
    <NoWarn>419</NoWarn>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\portable</OutputPath>
    <DefineConstants>TRACE;PORTABLE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <DocumentationFile>bin\Release\portable\Dropbox.Api.XML</DocumentationFile>
    <NoWarn>419</NoWarn>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="Newtonsoft.Json">
      <HintPath>..\packages\Newtonsoft.Json.7.0.1\lib\portable-net45+wp80+win8+wpa81+dnxcore50\Newtonsoft.Json.dll</HintPath>
    </Reference>
  </ItemGroup>
"""

PORTABLE_CSPROJ_END_BLOCK = r"""  <Import Project="$(MSBuildExtensionsPath32)\Microsoft\Portable\$(TargetFrameworkVersion)\Microsoft.Portable.CSharp.targets" />
  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
       Other similar extension points exist, see Microsoft.Common.targets.
  <Target Name="BeforeBuild">
  </Target>
  <Target Name="AfterBuild">
  </Target>
  -->
</Project>
"""

DOC_CSPROJ_START_BLOCK = r"""<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{0E57A534-F4CA-402B-88F4-0B43E55264BA}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>AppProperties</AppDesignerFolder>
    <RootNamespace>Dropbox.Api</RootNamespace>
    <AssemblyName>Dropbox.Api</AssemblyName>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <SolutionDir Condition="$(SolutionDir) == '' Or $(SolutionDir) == '*Undefined*'">..\</SolutionDir>
    <RestorePackages>true</RestorePackages>
    <BaseIntermediateOutputPath>docobj\</BaseIntermediateOutputPath>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>docbin\Debug\</OutputPath>
    <DefineConstants>TRACE;DEBUG;DOC</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <DocumentationFile>docbin\Debug\Dropbox.Api.XML</DocumentationFile>
    <NoWarn>419</NoWarn>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>docbin\Release\</OutputPath>
    <DefineConstants>TRACE;DOC</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <DocumentationFile>docbin\Release\Dropbox.Api.XML</DocumentationFile>
    <NoWarn>419</NoWarn>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="System.Net.Http" />
    <Reference Include="System.Xml.Linq" />
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="Microsoft.CSharp" />
    <Reference Include="System.Data" />
    <Reference Include="System.Xml" />
    <Reference Include="Newtonsoft.Json">
      <HintPath>..\packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll</HintPath>
    </Reference>
   </ItemGroup>
"""

DOC_CSPROJ_END_BLOCK = r"""  <ItemGroup>
    <None Include="stone_summaries.xml" />
    <None Include="Generated\namespace_summaries.xml" />
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
       Other similar extension points exist, see Microsoft.Common.targets.
  <Target Name="BeforeBuild">
  </Target>
  <Target Name="AfterBuild">
  </Target>
  -->
</Project>
"""

def _include_items(buf, item_type, paths):
    buf.write('  <ItemGroup>\n')
    for path in paths:
        file_path = path.replace('/', '\\')
        buf.write('    <{0} Include="{1}" />\n'.format(item_type, file_path))
    buf.write('  </ItemGroup>\n')


def make_csproj_file(files, mode):
    mode = mode.lower()

    if mode == 'doc':
        start = DOC_CSPROJ_START_BLOCK
        end = DOC_CSPROJ_END_BLOCK
        none_includes = DOC_NONE_INCLUDES
    elif mode == 'portable40':
        start = PORTABLE40_CSPROJ_START_BLOCK
        end = PORTABLE40_CSPROJ_END_BLOCK
        none_includes = PORTABLE40_NONE_INCLUDES
    elif mode == 'portable':
        start = PORTABLE_CSPROJ_START_BLOCK
        end = PORTABLE_CSPROJ_END_BLOCK
        none_includes = PORTABLE_NONE_INCLUDES
    else:
        start = CSPROJ_START_BLOCK
        end = CSPROJ_END_BLOCK
        none_includes = NONE_INCLUDES

    buf = StringIO()
    buf.write(start)

    _include_items(buf, 'Compile', COMPILE_INCLUDES)
    _include_items(buf, 'Compile', sorted(files, key=lambda x: x.replace('\\', '/')))
    _include_items(buf, 'None', none_includes)

    buf.write(end)

    return buf.getvalue()
