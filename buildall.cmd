@echo off

IF NOT "%VisualStudioVersion%" == "12.0" (
    echo This requires a visual studio version 12.0 "VS2013" to be installed.
    echo Did you run vcvarsall.exe?
    goto :eof
)

where msbuild /q
IF NOT "%ERRORLEVEL%" == "0" (
    echo "Cannot find msbuild.exe?"
    GOTO :eof
)

where python /q
IF NOT "%ERRORLEVEL%" == "0" (
    echo "Cannot find python.exe, do you need to check your path?"
    GOTO :eof
)

where babelapi /q
IF NOT "%ERRORLEVEL%" == "0" (
    echo "Cannot find babelapi.exe, do you need to check your path?"
    GOTO :eof
)

echo Generating Dropbox.Api...
babelapi generator\csharp.babelg.py ..\spec\files.babel ..\spec\users.babel Dropbox.Api --clean-build

echo Building...
msbuild babel.sln

:eof
    echo Done
