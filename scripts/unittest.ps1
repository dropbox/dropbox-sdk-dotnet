msbuild "/t:Clean;Rebuild" "/verbosity:minimal" "/clp:ErrorsOnly" "/m" ".\dropbox-sdk-dotnet\Dropbox.Api\Dropbox.Api.csproj" "/p:Configuration=Debug"
msbuild "/t:Clean;Rebuild" "/verbosity:minimal" "/clp:ErrorsOnly" "/m" ".\dropbox-sdk-dotnet\Dropbox.Api.Unit.Tests\Dropbox.Api.Unit.Tests.csproj" "/p:Configuration=Debug"
vstest.console ".\dropbox-sdk-dotnet\Dropbox.Api.Unit.Tests\bin\Debug\Dropbox.Api.Unit.Tests.dll"