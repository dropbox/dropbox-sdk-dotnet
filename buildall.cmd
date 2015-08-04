@echo off

SETLOCAL EnableDelayedExpansion

SET PRE=

:run

IF NOT "%VisualStudioVersion%" == "12.0" (
    echo This requires a visual studio version 12.0 "VS2013" to be installed.
    echo Did you run vcvarsall.bat?
    goto :eof
)

where msbuild /q
IF NOT "%ERRORLEVEL%" == "0" (
    echo Cannot find msbuild.exe?
    GOTO :eof
)

where python /q
IF NOT "%ERRORLEVEL%" == "0" (
    echo Cannot find python.exe, do you need to check your path?
    GOTO :eof
)

where babelapi /q
IF NOT "%ERRORLEVEL%" == "0" (
    echo Cannot find babelapi.exe, do you need to check your path?
    GOTO :eof
)

echo Generating Dropbox.Api...

SET BABEL_LIST=

FOR %%f IN (spec\*.babel) DO SET BABEL_LIST=%%f !BABEL_LIST!

%PRE% babelapi generator\csharp.babelg.py !BABEL_LIST! dropbox-sdk-dotnet\Dropbox.Api --clean-build

SET SOLUTION_DIR="dropbox-sdk-dotnet"

echo Restoring packages using nuget.exe ...

IF EXIST %SOLUTION_DIR%\.nuget\nuget.exe (
    %PRE% %SOLUTION_DIR%\.nuget\nuget.exe restore %SOLUTION_DIR%\Dropbox.Api.sln
) ELSE (
    where nuget /q
    if NOT "%ERRORLEVEL%" == "0" (
        echo Cannot find nuget.exe, install from nuget.org
        GOTO :eof
    )
    %PRE% nuget.exe restore %SOLUTION_DIR%\Dropbox.Api.sln
)

echo Building...
%PRE% msbuild /verbosity:minimal /m %SOLUTION_DIR%\Dropbox.Api.sln
%PRE% msbuild /verbosity:minimal /m %SOLUTION_DIR%\Dropbox.Api\Dropbox.Api.Doc.csproj
%PRE% msbuild /verbosity:minimal /m doc\BabelDocs.shfbproj

:eof
    ENDLOCAL
    echo Done
