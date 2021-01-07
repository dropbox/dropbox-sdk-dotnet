#!/usr/bin/env pwsh
. $PSScriptRoot/util.ps1
EnsureCommand python3
if (!(Get-Command venv/bin/python -ErrorAction SilentlyContinue)) {
    python3 -m venv venv
}
if (!(Get-Command venv/bin/stone -ErrorAction SilentlyContinue)) {
    venv/bin/pip install -r requirements-dev.txt
}
rm -rf dropbox-sdk-dotnet/Dropbox.Api/Generated
echo "Generating Stone types"
venv/bin/stone `
  --attribute :all `
  generator/csharp.stoneg.py `
  dropbox-sdk-dotnet/Dropbox.Api `
  $(Get-ChildItem spec/*.stone)
