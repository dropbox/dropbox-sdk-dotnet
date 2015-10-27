Private Dropbox .NET SDK
========================

This repository is used to generate the [public Dropbox .NET SDK]
(https://github.com/dropbox/dropbox-sdk-dotnet).

This repository contains no auto-generated code.

Basic Setup
-----------

1. Clone the public and private SDK repo into `~/src`.

2. In the private SDK repo, run ``git submodule init`` followed by a
   ``git submodule update`` to pull in the ``spec`` and ``babel`` sub repos.

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
1. From the private repo, edit `generator/common/Dropbox.Api.nuspec` by updating version and release note.
2. Run `buildall.cmd`.
   A .nukpg file will be generated in `Dropbox.Api` directory.
3. Go to nuget.org to publish the package. Please ask around for the account credentials.

Generating Docs
---------------
1. Run `buildall.cmd`.
2. Push generated code in doc/Help to public homepage branch.

