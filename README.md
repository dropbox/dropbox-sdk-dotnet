Dropbox .NET SDK
========================

This repository is used to generate the Dropbox .NET SDK
This repository contains no auto-generated code.

Basic Setup
-----------
1. Prerequisites:
  - Visual Studio 2013 or above.
  - Python 2.7 or above.
  - Sandcastle Help File Builder installed (https://github.com/EWSoftware/SHFB/releases).

2. Run ``git submodule init`` followed by a
   ``git submodule update`` to pull in the ``spec`` and ``stone`` sub repos.

4. Run
   ```
   cd stone
   python setup.py install
   ```

Updating the SDK for a new spec
-------------------------------

1. Go into the `spec` folder and update it to the desired commit. To update to
   the latest, simply use:

   ```
   $ git pull
   ```

2. Run `generate.py` script to generatedi class for latest data types.

   ```
   $ python generate.py
   ```

3. Open up the `Dropbox.Api.sln` in Visual Studio and run
   the included examples as a sanity check.

Create nuget package (This needs to be done on Windows)
--------------------
1. From the private repo, edit `dropbox-sdk-dotnet/Dropbox.Api/Dropbox.Api.nuspec` and update release note.
2. Edit buildall.ps1 and update major version and release version.
3. In Visual Studio Developer Command Prompt run
   ```
   powershell -ExecutionPolicy Bypass -File buildall.ps1 -testSettings:<PATH_TO_TEST_SETTINGS> 
   ```
   A .nukpg file will be generated in `Dropbox.Api` directory.

Generating Docs
---------------
1. In Visual Studio Developer Command Prompt, run
   ```
   powershell -ExecutionPolicy Bypass -File buildall.ps1 -testSettings:<PATH_TO_TEST_SETTINGS> -doc:True
   ```
