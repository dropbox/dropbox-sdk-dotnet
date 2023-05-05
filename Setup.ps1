[CmdletBinding()]
param (
    [Parameter(Mandatory)][string]$DbxAppKey,
    [Parameter(Mandatory)][string]$DbxAppSecret,
    [Parameter(Mandatory)][string]$DbxUserAccessToken,
    [System.EnvironmentVariableTarget]$Scope = [System.EnvironmentVariableTarget]::User
)

Function Script:Set-EnvironmentVariable {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)][string]$Variable,
        [Parameter(Mandatory)][string]$Value,
        [Parameter(Mandatory)][System.EnvironmentVariableTarget]$Scope
    )
        [System.Environment]::SetEnvironmentVariable($Variable, $Value, $Scope)
        Set-Content -Path Env:\$Variable -Value $Value

}

Function Register-DropboxSdkDotnet {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)][string]$DbxAppKey,
        [Parameter(Mandatory)][string]$DbxAppSecret,
        [Parameter(Mandatory)][string]$DbxUserAccessToken,
        [System.EnvironmentVariableTarget]$Scope = [System.EnvironmentVariableTarget]::User
    )

    PROCESS {
        Set-EnvironmentVariable 'DROPBOX_INTEGRATION_AppKey' $DbxAppKey $Scope
        Set-EnvironmentVariable 'DROPBOX_INTEGRATION_appSecret' $DbxAppSecret $Scope
        Set-EnvironmentVariable 'DROPBOX_INTEGRATION_userAccessToken' $DbxUserAccessToken $Scope
    }
}
Register-DropboxSdkDotnet @PSBoundParameters
