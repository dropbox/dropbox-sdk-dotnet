## Important: Use v7.0.0 or newer of this SDK for compatibility with the Dropbox API servers. Older versions stopped working in January 2026. Please refer to this blog post for more information: https://dropbox.tech/developers/api-server-certificate-changes

[![Logo][logo]][repo]

[![NuGet](https://img.shields.io/badge/Frameworks-.NetStandard%202.0-blue)](https://www.nuget.org/packages/Dropbox.Api)
[![NuGet](https://img.shields.io/nuget/v/Dropbox.Api.svg)](https://www.nuget.org/packages/Dropbox.Api)
[![codecov](https://codecov.io/gh/dropbox/dropbox-sdk-dotnet/branch/main/graph/badge.svg)](https://codecov.io/gh/dropbox/dropbox-sdk-dotnet)

The official Dropbox SDK for DotNet.

Documentation can be found on [GitHub Pages][documentation]

## Installation

Create an app via the [Developer Console][devconsole]

Install via [NuGet](https://www.nuget.org/)

```
PM> Install-Package Dropbox.Api
```

After installation, follow one of our [Examples][examples] or read the [Documentation][documentation].

You can also view our [OAuth guide][oauthguide].

## Examples

We provide [Examples][examples] to help get you started with a lot of the basic functionality in the SDK.

- **OAuth**
    - [OAuth Basic](https://github.com/dropbox/dropbox-sdk-dotnet/tree/main/dropbox-sdk-dotnet/Examples/OauthBasic) - Simple application that runs through a basic OAuth flow to acquire a token and make a call to users/get_current_account.
    - [OAuth PKCE](https://github.com/dropbox/dropbox-sdk-dotnet/tree/main/dropbox-sdk-dotnet/Examples/OAuthPKCE) - Simple application that runs through an OAuth flow using [PKCE](https://oauth.net/2/pkce/) and acquires a token to make a call to users/get_current_account.
- **Other Examples**
    - [Simple Test](https://github.com/dropbox/dropbox-sdk-dotnet/tree/main/dropbox-sdk-dotnet/Examples/SimpleTest) - This is a simple test which validates some simple functionality (This is a good place to start to see how the SDK can be used)
    - [Simple Blog Demo](https://github.com/dropbox/dropbox-sdk-dotnet/tree/main/dropbox-sdk-dotnet/Examples/SimpleBlogDemo) - This is a simple demo of how the Dropbox SDK can be used to create a simple blog with backed up blog posts
    - [Simple Business Dashboard](https://github.com/dropbox/dropbox-sdk-dotnet/tree/main/dropbox-sdk-dotnet/Examples/SimpleBusinessDashboard) - This is a demo of a business dashboard
    - [Universal Demo](https://github.com/dropbox/dropbox-sdk-dotnet/tree/main/dropbox-sdk-dotnet/Examples/UniversalDemo/UniversalDemo) - This is an example of how to use the SDK across multiple platforms

## Upload integrity

For file upload calls whose request type includes `ContentHash`, the SDK automatically computes and sends a Dropbox `content_hash` when the upload stream is seekable. The server rejects the upload if the hash does not match the bytes it receives.

Because `content_hash` is part of the request header, this default-on validation reads the stream once to compute the hash, seeks back to the original position, and then reads it again for the upload body. That is two reads (2×) of I/O for seekable uploads. For local files the second read is often served from OS cache; for slow, network-backed, or encrypted streams it can materially increase upload time. Non-seekable streams are uploaded without automatic hashing. A manually supplied `contentHash` argument always wins. If a seekable stream has unusual read or rewind behavior, disable automatic hashing for that upload or client.

You can also compute a content hash yourself, for example to verify a downloaded file against its `ContentHash` metadata:

```csharp
using Dropbox.Api.Files;

using (var stream = File.OpenRead("local-file.txt"))
{
    var contentHash = ContentHasher.ComputeHash(stream);
    // Compare against metadata.ContentHash returned by the server.
}
```

To disable automatic hashing for one upload, wrap the stream:

```csharp
using Dropbox.Api.Files;

await client.Files.UploadAsync(
    new UploadArg("/remote-file.txt"),
    ContentHasher.WithoutAutoContentHash(stream));
```

To disable automatic hashing for a client:

```csharp
var config = new DropboxClientConfig
{
    AutoContentHash = false,
};
```

## Reusing a cached access token

`DropboxClient.GetAccessTokenSnapshot()` and `DropboxTeamClient.GetAccessTokenSnapshot()`
read local cached credentials without HTTP requests or refresh. The immutable snapshot
is null until an access token is available. Its nullable `ExpiresAt` is UTC; an unknown
expiration does not imply an unlimited lifetime. Cached tokens may be expired or revoked.
Access tokens are sensitive: store them securely and never log them.

After an ordinary SDK call (which refreshes automatically when needed), reuse the
snapshot with an existing constructor. Keep refresh credentials separately:

```csharp
await client.Users.GetCurrentAccountAsync();
var snapshot = client.GetAccessTokenSnapshot();
if (snapshot != null)
{
    // refreshToken, appKey and appSecret come from the application's secure storage.
    using (var nextClient = snapshot.ExpiresAt.HasValue
        ? new DropboxClient(snapshot.AccessToken, refreshToken,
            snapshot.ExpiresAt.Value.LocalDateTime, appKey, appSecret)
        : new DropboxClient(snapshot.AccessToken, refreshToken,
            appKey, appSecret, new DropboxClientConfig()))
    {
        await nextClient.Users.GetCurrentAccountAsync();
    }
}
```

The example converts known expiration to local `DateTime` for the existing constructors,
consistent with the SDK's refresh timing convention. An existing explicit
`await client.RefreshAccessToken(null)` also updates the cache; snapshot reads themselves
do not require refresh. Clients created with `WithPathRoot`, `AsMember`, and `AsAdmin`
share cached credentials and see refreshes performed by any of those clients.

## HTTP error metadata

Catch `DropboxException` to read the request ID, actual HTTP status, and a read-only
snapshot of response and content headers. Header lookup is case-insensitive and preserves
multiple values, even after the response is disposed. Without an HTTP response, the status
is null and the headers are empty. Optional headers such as `X-Server-Response-Time`
may be absent.

```csharp
try
{
    await client.Files.GetMetadataAsync("/file.txt");
}
catch (Dropbox.Api.DropboxException ex)
{
    var requestId = ex.RequestId;
    var status = ex.ResponseStatusCode;
    if (ex.ResponseHeaders.TryGetValue("X-Server-Response-Time", out var values))
    {
        // Use all values as needed; this optional header is not interpreted by the SDK.
    }
}
```

Malformed structured error responses raise `DropboxException` with the decoding failure
in `InnerException` and the HTTP metadata preserved. Existing structured exception types
and retry behavior are unchanged for valid error responses. `RateLimitException.RetryAfter`
continues to come from the structured error body.

## Getting Help

If you find a bug, please see [CONTRIBUTING.md][contributing] for information on how to report it.

If you need help that is not specific to this SDK, please reach out to [Dropbox Support][support].

## License

This SDK is distributed under the MIT license, please see [LICENSE][license] for more information.

[logo]: https://cfl.dropboxstatic.com/static/images/sdk/dotnet_banner.png
[repo]: https://github.com/dropbox/dropbox-sdk-dotnet
[documentation]: https://dropbox.github.io/dropbox-sdk-dotnet/gh-pages/obj/api/Dropbox.Api.html
[examples]: https://github.com/dropbox/dropbox-sdk-dotnet/tree/main/dropbox-sdk-dotnet/Examples
[license]: https://github.com/dropbox/dropbox-sdk-dotnet/blob/main/LICENSE
[contributing]: https://github.com/dropbox/dropbox-sdk-dotnet/blob/main/CONTRIBUTING.md
[devconsole]: https://dropbox.com/developers/apps
[oauthguide]: https://www.dropbox.com/lp/developers/reference/oauth-guide
[support]: https://www.dropbox.com/developers/contact
