@echo off

SETLOCAL

:process_arg
IF "%~1"=="" GOTO :done_args
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
    echo ^   buildall Spec_Dir ^<specs^>
    echo. 
    echo ^   Args:
    echo ^       Spec_Dir ^<specs^>      The directory containing babel specifications
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
babelapi generator\csharp.babelg.py %ARG_SPEC_DIR%\files.babel %ARG_SPEC_DIR%\users.babel Dropbox.Api --clean-build

echo Building...
msbuild babel.sln

:eof
    ENDLOCAL
    echo Done
