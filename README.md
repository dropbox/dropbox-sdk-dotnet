Private Dropbox .NET SDK
========================

This repository is used to generate the [public Dropbox .NET SDK]
(https://github.com/dropbox/dropbox-sdk-dotnet).

This repository contains no auto-generated code.

Basic Setup
-----------
1. Prerequisites:
  - Visual Studio 2013 or above.
  - Python 2.7 or above.
  - Sandcastle Help File Builder installed (https://github.com/EWSoftware/SHFB/releases).
2. Clone the public and private SDK repo into `~/src`.

3. In the private SDK repo, run ``git submodule init`` followed by a
   ``git submodule update`` to pull in the ``spec`` and ``stone`` sub repos.

4. In the private SDK repo, run
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

2. Use the `generate.py` script with the path to the local check out of the public:

   ```
   $ python generate.py -h
   $ python generate.py ../dropbox-sdk-dotnet
   ```

3. From the public repo, open up the `Dropbox.Api.sln` in Visual Studio and run
   the included examples as a sanity check.

4. Commit the update to the spec sub repo (from step 1) to the private SDK repo.
   Do this from the root of the private SDK repo:

   ```
    $ git commit -a -m "Updated spec"
    $ git push
   ```

5. Commit and push the public repo.


Publishing a new SDK (This needs to be done on Windows)
--------------------
1. From the private repo, edit `dropbox-sdk-dotnet/Dropbox.Api/Dropbox.Api.nuspec` and update release note.
2. Edit buildall.ps1 and update major version and release version.
3. In Visual Studio Developer Command Prompt run
   ```
   powershell -ExecutionPolicy Bypass -File buildall.ps1 -testSettings:<PATH_TO_TEST_SETTINGS> -signKeyPath:<PATH_TO_SIGNING_KEY>
   ```
   A .nukpg file will be generated in `Dropbox.Api` directory.
3. Go to nuget.org to publish the package. Please ask around for the account credentials.

Generating Docs
---------------
1. In Visual Studio Developer Command Prompt, run
   ```
   powershell -ExecutionPolicy Bypass -File buildall.ps1 -testSettings:<PATH_TO_TEST_SETTINGS> -signKeyPath:<PATH_TO_SIGNING_KEY> -doc:True
   ```
2. Push generated code in doc/Help to public homepage branch.

