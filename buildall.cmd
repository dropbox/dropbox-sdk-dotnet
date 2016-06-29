@echo off

SETLOCAL EnableDelayedExpansion

SET PRE=
SET PYTHONPATH=stone

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

echo Generating Dropbox.Api...

SET STONE_LIST=

FOR %%f IN (spec\*.stone) DO SET STONE_LIST=!STONE_LIST! %%f

%PRE% python -m stone.cli --filter-by-route-attr "alpha_group=null" generator\csharp.stoneg.py dropbox-sdk-dotnet\Dropbox.Api !STONE_LIST! --clean-build -- -p

SET SOLUTION_DIR="dropbox-sdk-dotnet"

echo Restoring packages using nuget.exe ...

SET NUGET_COMMAND=

IF EXIST %SOLUTION_DIR%\.nuget\nuget.exe (
    SET NUGET_COMMAND=%SOLUTION_DIR%\.nuget\nuget.exe
) ELSE (
    where nuget /q
    if NOT "%ERRORLEVEL%" == "0" (
        echo Cannot find nuget.exe, install from nuget.org
        GOTO :eof
    )
    SET NUGET_COMMAND=nuget.exe
)

%PRE% %NUGET_COMMAND% restore %SOLUTION_DIR%\Dropbox.Api.sln

echo Building...
%PRE% msbuild /verbosity:minimal /m %SOLUTION_DIR%\Dropbox.Api.sln /p:Configuration=Release
%PRE% msbuild /verbosity:minimal /m %SOLUTION_DIR%\Dropbox.Api\Dropbox.Api.Doc.csproj
%PRE% msbuild /verbosity:minimal /m doc\StoneDocs.shfbproj

echo Creating nuget package...

%PRE% %NUGET_COMMAND% pack %SOLUTION_DIR%\Dropbox.Api\Dropbox.Api.nuspec

:eof
    ENDLOCAL
    echo Done
