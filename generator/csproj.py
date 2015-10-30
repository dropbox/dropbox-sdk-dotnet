from __future__ import unicode_literals

from StringIO import StringIO


COMPILE_INCLUDES = [
    "Babel\\Empty.cs",
    "Babel\\IDecoder.cs",
    "Babel\\IEncodable.cs",
    "Babel\\IEncoder.cs",
    "Babel\\ITransport.cs",
    "Babel\\JsonDecoder.cs",
    "Babel\\JsonEncoder.cs",
    "Babel\\Json\\JsonArray.cs",
    "Babel\\Json\\JsonObject.cs",
    "Babel\\Json\\JsonParser.cs",
    "ApiException.cs",
    "Babel\\Util.cs",
    "DropboxCertHelper.cs",
    "DropboxClient.common.cs",
    "DropboxOauth2Helper.cs",
    "DropboxRequestHandler.cs",
    "Properties\\AssemblyInfo.cs",
]

NONE_INCLUDES = [
    "babel_summaries.xml",
    "app.config",
    "packages.config",        
]

CSPROJ_START_BLOCK = r"""<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{D7B167CE-3AF8-478E-82F2-684D38F1DF98}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>Dropbox.Api</RootNamespace>
    <AssemblyName>Dropbox.Api</AssemblyName>
    <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
    <TargetFrameworkProfile>Profile344</TargetFrameworkProfile>
    <FileAlignment>512</FileAlignment>
    <ProjectTypeGuids>{786C830F-07A1-408B-BD7F-6EE04809D6DB};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
    <MinimumVisualStudioVersion>10.0</MinimumVisualStudioVersion>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <DocumentationFile>bin\Debug\Dropbox.Api.XML</DocumentationFile>
    <RunCodeAnalysis>true</RunCodeAnalysis>
    <NoWarn>419</NoWarn>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <DocumentationFile>bin\Release\Dropbox.Api.XML</DocumentationFile>
    <NoWarn>419</NoWarn>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="Microsoft.Threading.Tasks">
      <HintPath>..\packages\Microsoft.Bcl.Async.1.0.168\lib\portable-net40+sl4+win8+wp71+wpa81\Microsoft.Threading.Tasks.dll</HintPath>
    </Reference>
    <Reference Include="Microsoft.Threading.Tasks.Extensions">
      <HintPath>..\packages\Microsoft.Bcl.Async.1.0.168\lib\portable-net40+sl4+win8+wp71+wpa81\Microsoft.Threading.Tasks.Extensions.dll</HintPath>
    </Reference>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="Microsoft.CSharp" />
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
  </ItemGroup>
"""


CSPROJ_END_BLOCK = r"""  <ItemGroup>
    <None Include="namespace_summaries.xml" />
  </ItemGroup>
  <Import Project="$(MSBuildExtensionsPath32)\Microsoft\Portable\$(TargetFrameworkVersion)\Microsoft.Portable.CSharp.targets" />
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


DOC_CSPROJ_START_BLOCK = r"""<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{0E57A534-F4CA-402B-88F4-0B43E55264BA}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>Dropbox.Api</RootNamespace>
    <AssemblyName>Dropbox.Api</AssemblyName>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <SolutionDir Condition="$(SolutionDir) == '' Or $(SolutionDir) == '*Undefined*'">..\</SolutionDir>
    <RestorePackages>true</RestorePackages>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>docbin\Debug\</OutputPath>
    <IntermediateOutputPath>docobj\Debug\</IntermediateOutputPath>
    <DefineConstants>TRACE;DEBUG;DOCUMENTATION_BUILD</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <DocumentationFile>docbin\Debug\Dropbox.Api.XML</DocumentationFile>
    <NoWarn>419</NoWarn>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>docbin\Release\</OutputPath>
    <IntermediateOutputPath>docobj\Release\</IntermediateOutputPath>
    <DefineConstants>TRACE;DOCUMENTATION_BUILD</DefineConstants>
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
  </ItemGroup>
"""


DOC_CSPROJ_END_BLOCK = r"""  <ItemGroup>
    <None Include="namespace_summaries.xml" />
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

LINK_PREFIX = "..\\..\\generator\\common\\"

def _include_items(buffer, item_type, paths, link):
    prefix =  LINK_PREFIX if link else ""
    buffer.write('  <ItemGroup>\n')
    for path in paths:
    	buffer.write('    <{0} Include="{1}{2}"'.format(item_type, prefix, path))
    	if link:
            buffer.write('>\n      <Link>{0}</Link>\n'.format(path))
            buffer.write('    </{0}>\n'.format(item_type))
        else:
            buffer.write(' />\n')
    buffer.write('  </ItemGroup>\n')

def make_csproj_file(files, is_doc=False, link=False):
    compile = []
    compile.extend(COMPILE_INCLUDES)
    
    buffer = StringIO()
    buffer.write(DOC_CSPROJ_START_BLOCK if is_doc else CSPROJ_START_BLOCK)
    
    _include_items(buffer, 'Compile', COMPILE_INCLUDES, link)
    _include_items(buffer, 'Compile', sorted(files), False)
    _include_items(buffer, 'None', NONE_INCLUDES, link)
 
    buffer.write(DOC_CSPROJ_END_BLOCK if is_doc else CSPROJ_END_BLOCK)

    return buffer.getvalue()
