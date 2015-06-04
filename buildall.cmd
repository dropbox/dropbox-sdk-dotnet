@echo off

SETLOCAL

SET PRE=
SET GENERATE=""
SET RESTORE=""

:process_arg
IF "%~1"=="" GOTO :done_args
IF "%~1"=="/nogen" (
    SHIFT
    SET GENERATE=NOGENERATE
    GOTO :process_arg
)
IF "%~1"=="/norest" (
    SHIFT
    SET RESTORE=NORESTORE
    GOTO :process_arg
)
SET NAME=%~1
SHIFT
IF "%~1"=="" (
    echo %NAME% is missing a parameter
    GOTO :eof
)
SET ARG_%NAME%=%~1
SHIFT
GOTO :process_arg

:done_args

IF "%ARG_spec_dir%"=="" (
    echo Missing 'Spec_Dir' argument
    GOTO :usage
)

GOTO :run

:usage
    echo.
    echo Usage:
    echo ^   buildall Spec_Dir ^<specs^> [/nogen] [/norest]
    echo. 
    echo ^   Args:
    echo ^       Spec_Dir ^<specs^>       The directory containing babel specifications
    echo ^       /nogen                 Don't run the python code generator
    echo ^       /norest                Don't restore nuget packages
    echo. 

GOTO :eof

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

IF NOT "%GENERATE%" == "NOGENERATE" (
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
    %PRE% babelapi generator\csharp.babelg.py "%ARG_SPEC_DIR%\files.babel" "%ARG_SPEC_DIR%\users.babel" Dropbox.Api --clean-build
)

if NOT "%RESTORE%" =="NORESTORE" (
    echo Restoring packages using nuget.exe ...
    IF EXIST .nuget\nuget.exe (
        %PRE% .nuget\nuget.exe restore babel.sln
    ) ELSE (
        where nuget /q
        if NOT "%ERRORLEVEL%" == "0" (
            echo Cannot find nuget.exe, install from nuget.org
            GOTO :eof
        )
        %PRE% nuget.exe restore babel.sln
    )
)

echo Building...
%PRE% msbuild /verbosity:minimal /m babel.sln

:eof
    ENDLOCAL
    echo Done
