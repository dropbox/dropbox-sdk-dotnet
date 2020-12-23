function EnsureCommand()
{
    param (
        [string] $commandName
    )

    if (!(Get-Command "$commandName" -ErrorAction SilentlyContinue)) {
        Write-Error "Cannot find $commandName, please ensure it exists on your PATH" -ErrorAction Stop
    }
}
