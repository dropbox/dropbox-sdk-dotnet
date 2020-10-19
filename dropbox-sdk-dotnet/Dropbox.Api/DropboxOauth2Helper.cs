//-----------------------------------------------------------------------------
// <copyright file="DropboxOAuth2Helper.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Used by <see cref="DropboxOAuth2Helper.GetAuthorizeUri" /> to specify which OAuth 2.0 flow to use.
    /// </summary>
    public enum OAuthResponseType
    {
        /// <summary>
        /// This represents the OAuth 2.0 token or implicit grant flow. The server will return the bearer token via
        /// the <c>redirectUri</c> callback, rather than requiring your app to make a second call to a server.
        /// This is useful for pure client-side apps, such as mobile apps or JavaScript-based apps.
        /// </summary>
        Token,

        /// <summary>
        /// This represents the OAuth 2.0 code flow. The server will return a code via the <c>redirectUri</c>
        /// callback which should be converted into a bearer token using the <see cref="DropboxOAuth2Helper.ProcessCodeFlowAsync"/>
        /// method. This is the recommended flow for apps that are running on a server.
        /// </summary>
        Code
    }

    /// <summary>
    /// Used by <see cref="DropboxOAuth2Helper.GetAuthorizeUri" /> to specify which type of OAuth token to request.
    /// </summary>
    public enum TokenAccessType
    {
        /// <summary>
        /// Creates one long-lived token with no expiration
        /// </summary>
        Legacy,

        /// <summary>
        /// Create one short-lived token with an expiration with a refresh token
        /// </summary>
        Offline,

        /// <summary>
        /// Create one short-lived token with an expiration
        /// </summary>
        Online
    }

    /// <summary>
    /// Which scopes that have already been granted to include when requesting scopes
    /// </summary>
    public enum IncludeGrantedScopes
    {
        /// <summary>
        /// Default, requests only the scopes passed in to the oauth request
        /// </summary>
        None,
        /// <summary>
        /// Token should include all previously granted user scopes as well as requested scopes
        /// </summary>
        User, 
        /// <summary>
        /// Token should include all previously granted team scopes as well as requested scopes
        /// </summary>
        Team
    }

    /// <summary>
    /// Contains methods that make authorizing with Dropbox easier.
    /// </summary>
    /// <example>
    /// <para>
    /// This shows an example of how to use the token flow. This is part of a Windows Console or WPF app.
    /// </para>
    /// <para>
    /// The <c>GetAccessToken()</c> method calls <see cref="DropboxOAuth2Helper.GetAuthorizeUri" /> to create the URI with response type
    /// set to <see cref="OAuthResponseType.Token"/> for token flow.
    /// </para>
    /// <para>
    /// <see cref="Guid.NewGuid"/> is called to generate a random string to use as the state argument, this value can also be used to
    /// store application context and prevent cross-site request forgery.
    /// </para>
    /// <para>
    /// A <see cref="HttpListener"/> is created to listen to the <c>RedirectUri</c> which will later receive redirect callback from
    /// the server. <see cref="System.Diagnostics.Process.Start"/> is called to launch a native browser and navigate user to the authorize
    /// URI. The <c>RedirectUri</c> needs to be registered at <a href="https://www.dropbox.com/developers/apps">App Console</a>. It's
    /// common to use value like <c>http://127.0.0.1:{some_avaialble_port}</c>.
    /// </para>
    /// <para>
    /// After user successfully authorizes the request, <c>HandleOAuth2Redirect</c> receives the redirect callback which contains state
    /// and access token as URL fragment. Since the server cannot receive URL fragment directly, it calls <c>RespondPageWithJSRedirect</c>
    /// to respond with a HTML page which runs JS code and sends URL fragment as query string parameter to a separate <c>JSRedirect</c> endpoint.
    /// </para>
    /// <para>
    /// <c>HandleJSRedirect</c> is called to handle redirect from JS code and processes OAuth response from query string.
    /// This returns an <see cref="OAuth2Response"/> containing the access token that will be passed to the <see cref="DropboxClient"/> constructor.
    /// </para>
    /// <code>
    /// private async Task HandleOAuth2Redirect(HttpListener http)
    /// {
    ///     var context = await http.GetContextAsync();
    ///
    ///     // We only care about request to RedirectUri endpoint.
    ///     while (context.Request.Url.AbsolutePath != RedirectUri.AbsolutePath)
    ///     {
    ///         context = await http.GetContextAsync();
    ///     }
    ///
    ///     // Respond with a HTML page which runs JS to send URl fragment.
    ///     RespondPageWithJSRedirect();
    /// }
    ///
    ///
    /// private async Task&lt;OAuth2Response&gt; HandleJSRedirect(HttpListener http)
    /// {
    ///     var context = await http.GetContextAsync();
    ///
    ///     // We only care about request to TokenRedirectUri endpoint.
    ///     while (context.Request.Url.AbsolutePath != JSRedirectUri.AbsolutePath)
    ///     {
    ///         context = await http.GetContextAsync();
    ///     }
    ///
    ///     var redirectUri = new Uri(context.Request.QueryString["url_with_fragment"]);
    ///
    ///     var result = DropboxOAuth2Helper.ParseTokenFragment(redirectUri);
    ///
    ///     return result;
    /// }
    ///
    /// private async Task GetAccessToken() {
    ///     var state = Guid.NewGuid().ToString("N");
    ///     var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, ApiKey, new Uri(RedirectUri), state: state);
    ///
    ///     var http = new HttpListener();
    ///     http.Prefixes.Add(RedirectUri);
    ///     http.Start();
    ///
    ///     System.Diagnostics.Process.Start(authorizeUri.ToString());
    ///
    ///     // Handle OAuth redirect and send URL fragment to local server using JS.
    ///     await HandleOAuth2Redirect(http);
    ///
    ///     // Handle redirect from JS and process OAuth response.
    ///     var result = await HandleJSRedirect(http);
    ///
    ///     if (result.State != state)
    ///     {
    ///         // The state in the response doesn't match the state in the request.
    ///         return null;
    ///     }
    ///
    ///     Settings.Default.AccessToken = result.AccessToken;
    /// }
    /// </code>
    /// <para>
    /// This shows an example of how to use the code flow. This is part of a controller class on an ASP.Net MVC website.
    /// </para>
    /// <para>
    /// The <c>Connect()</c> method calls <see cref="DropboxOAuth2Helper.GetAuthorizeUri" /> to create the URI that the browser component
    /// navigate to; the response type is set to <see cref="OAuthResponseType.Code"/> to create a URI for the code flow.
    /// </para>
    /// <para>
    /// <see cref="Guid.NewGuid"/> is called to generate a random string to use as the state argument, this value is stored
    /// on a field in the web app's user database associated with the current user, this helps prevent cross-site request forgery.
    /// </para>
    /// <para>
    /// The <c>AuthAsync</c> method handles the route represented by the <c>RedirectUri</c>. The ASP.Net infrastructure has already 
    /// parsed the query string and extracted the <c>code</c> and <c>state</c> arguments. After validating that the <c>state</c>
    /// matches the value stored in the user record in the <c>Connect</c> method, authorization is completed by calling
    /// <see cref="DropboxOAuth2Helper.ProcessCodeFlowAsync"/>. This returns an <see cref="OAuth2Response"/> containing the access token
    /// that will be passed to the <see cref="DropboxClient"/> constructor.
    /// </para>
    /// <code>
    /// // GET: /Home/Connect
    /// public ActionResult Connect()
    /// {
    ///     var user = this.store.CurrentUser();
    ///     user.ConnectState = Guid.NewGuid().ToString("N");
    ///     this.store.SaveChanges();
    /// 
    ///     var redirect = DropboxOAuth2Helper.GetAuthorizeUri(OauthResponseType.Code, AppKey, RedirectUri, user.ConnectState);
    ///     return Redirect(redirect.ToString());
    /// }
    /// 
    /// // GET: /Home/Auth
    /// public async Task&lt;ActionResult&gt; AuthAsync(string code, string state)
    /// {
    ///     var user = this.store.CurrentUser();
    ///     
    ///     if (user.ConnectState != state)
    ///     {
    ///         this.Flash("There was an error connecting to Dropbox.");
    ///         return this.RedirectToAction("Index");
    ///     }
    ///     
    ///     OAuth2Response response = await DropboxOAuth2Helper.ProcessCodeFlowAsync(code, AppKey, AppSecret, RedirectUri);
    ///     
    ///     user.DropboxAccessToken = response.AccessToken;
    ///     await this.store.SaveChangesAsync();
    ///     
    ///     this.Flash("This account has been connected to Dropbox.");
    ///     return this.RedirectToAction("Profile");
    /// }
    /// </code>
    /// </example>
    public static class DropboxOAuth2Helper
    {
        /// <summary>
        /// Length of Code Verifier for PKCE to be used in OAuth Flow
        /// </summary>
        public static int PKCEVerifierLength = 128;

        /// <summary>
        /// Gets the URI used to start the OAuth2.0 authorization flow.
        /// </summary>
        /// <param name="oauthResponseType">The grant type requested, either <c>Token</c> or <c>Code</c>.</param>
        /// <param name="clientId">The apps key, found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="redirectUri">Where to redirect the user after authorization has completed. This must be the exact URI
        /// registered in the <a href="https://www.dropbox.com/developers/apps">App Console</a>; even <c>localhost</c>
        /// must be listed if it is used for testing. A redirect URI is required for a token flow, but optional for code. 
        /// If the redirect URI is omitted, the code will be presented directly to the user and they will be invited to enter
        /// the information in your app.</param>
        /// <param name="state">Up to 500 bytes of arbitrary data that will be passed back to <paramref name="redirectUri"/>.
        /// This parameter should be used to protect against cross-site request forgery (CSRF).</param>
        /// <param name="forceReapprove">Whether or not to force the user to approve the app again if they've already done so.
        /// If <c>false</c> (default), a user who has already approved the application may be automatically redirected to
        /// <paramref name="redirectUri"/>If <c>true</c>, the user will not be automatically redirected and will have to approve
        /// the app again.</param>
        /// <param name="disableSignup">When <c>true</c> (default is <c>false</c>) users will not be able to sign up for a
        /// Dropbox account via the authorization page. Instead, the authorization page will show a link to the Dropbox
        /// iOS app in the App Store. This is only intended for use when necessary for compliance with App Store policies.</param>
        /// <param name="requireRole">If this parameter is specified, the user will be asked to authorize with a particular
        /// type of Dropbox account, either work for a team account or personal for a personal account. Your app should still
        /// verify the type of Dropbox account after authorization since the user could modify or remove the require_role
        /// parameter.</param>
        /// <param name="forceReauthentication"> If <c>true</c>, users will be signed out if they are currently signed in.
        /// This will make sure the user is brought to a page where they can create a new account or sign in to another account.
        /// This should only be used when there is a definite reason to believe that the user needs to sign in to a new or
        /// different account.</param>
        /// <param name="tokenAccessType">Determines the type of token to request.  See <see cref="TokenAccessType" /> 
        /// for information on specific types available.  If none is specified, this will use the legacy type.</param>
        /// <param name="scopeList">list of scopes to request in base oauth flow.  If left blank, will default to all scopes for app</param>
        /// <param name="includeGrantedScopes">which scopes to include from previous grants. Note: if this user has never linked the app, include_granted_scopes must be None</param>
        /// <param name="codeChallenge">If using PKCE, please us the PKCEOAuthFlow object</param>
        /// <returns>The uri of a web page which must be displayed to the user in order to authorize the app.</returns>
        public static Uri GetAuthorizeUri(OAuthResponseType oauthResponseType, string clientId, string redirectUri = null, string state = null, bool forceReapprove = false, bool disableSignup = false, string requireRole = null, bool forceReauthentication = false, TokenAccessType tokenAccessType = TokenAccessType.Legacy, string[] scopeList = null, IncludeGrantedScopes includeGrantedScopes = IncludeGrantedScopes.None, string codeChallenge = null)
        {
            var uri = string.IsNullOrEmpty(redirectUri) ? null : new Uri(redirectUri);

            return GetAuthorizeUri(oauthResponseType, clientId, uri, state, forceReapprove, disableSignup, requireRole, forceReauthentication, tokenAccessType, scopeList, includeGrantedScopes, codeChallenge);
        }

        /// <summary>
        /// Gets the URI used to start the OAuth2.0 authorization flow.
        /// </summary>
        /// <param name="oauthResponseType">The grant type requested, either <c>Token</c> or <c>Code</c>.</param>
        /// <param name="clientId">The apps key, found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="redirectUri">Where to redirect the user after authorization has completed. This must be the exact URI
        /// registered in the <a href="https://www.dropbox.com/developers/apps">App Console</a>; even <c>localhost</c>
        /// must be listed if it is used for testing. A redirect URI is required for a token flow, but optional for code. 
        /// If the redirect URI is omitted, the code will be presented directly to the user and they will be invited to enter
        /// the information in your app.</param>
        /// <param name="state">Up to 500 bytes of arbitrary data that will be passed back to <paramref name="redirectUri"/>.
        /// This parameter should be used to protect against cross-site request forgery (CSRF).</param>
        /// <param name="forceReapprove">Whether or not to force the user to approve the app again if they've already done so.
        /// If <c>false</c> (default), a user who has already approved the application may be automatically redirected to
        /// <paramref name="redirectUri"/>If <c>true</c>, the user will not be automatically redirected and will have to approve
        /// the app again.</param>
        /// <param name="disableSignup">When <c>true</c> (default is <c>false</c>) users will not be able to sign up for a
        /// Dropbox account via the authorization page. Instead, the authorization page will show a link to the Dropbox
        /// iOS app in the App Store. This is only intended for use when necessary for compliance with App Store policies.</param>
        /// <param name="requireRole">If this parameter is specified, the user will be asked to authorize with a particular
        /// type of Dropbox account, either work for a team account or personal for a personal account. Your app should still
        /// verify the type of Dropbox account after authorization since the user could modify or remove the require_role
        /// parameter.</param>
        /// <param name="forceReauthentication"> If <c>true</c>, users will be signed out if they are currently signed in.
        /// This will make sure the user is brought to a page where they can create a new account or sign in to another account.
        /// This should only be used when there is a definite reason to believe that the user needs to sign in to a new or
        /// different account.</param>
        /// <param name="tokenAccessType">Determines the type of token to request.  See <see cref="TokenAccessType" /> 
        /// for information on specific types available.  If none is specified, this will use the legacy type.</param>
        /// <param name="scopeList">list of scopes to request in base oauth flow.  If left blank, will default to all scopes for app</param>
        /// <param name="includeGrantedScopes">which scopes to include from previous grants. Note: if this user has never linked the app, include_granted_scopes must be None</param>
        /// <param name="codeChallenge">If using PKCE, please us the PKCEOAuthFlow object</param>
        /// <returns>The uri of a web page which must be displayed to the user in order to authorize the app.</returns>
        public static Uri GetAuthorizeUri(OAuthResponseType oauthResponseType, string clientId, Uri redirectUri = null, string state = null, bool forceReapprove = false, bool disableSignup = false, string requireRole = null, bool forceReauthentication = false, TokenAccessType tokenAccessType = TokenAccessType.Legacy, string[] scopeList = null, IncludeGrantedScopes includeGrantedScopes = IncludeGrantedScopes.None, string codeChallenge = null)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException("clientId");
            }
            
            if (redirectUri == null && oauthResponseType != OAuthResponseType.Code)
            {
                throw new ArgumentNullException("redirectUri");
            }

            var queryBuilder = new StringBuilder();

            queryBuilder.Append("response_type=");
            switch (oauthResponseType)
            {
                case OAuthResponseType.Token:
                    queryBuilder.Append("token");
                    break;
                case OAuthResponseType.Code:
                    queryBuilder.Append("code");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("oauthResponseType");
            }

            queryBuilder.Append("&client_id=").Append(Uri.EscapeDataString(clientId));

            if (redirectUri != null)
            {
                queryBuilder.Append("&redirect_uri=").Append(Uri.EscapeDataString(redirectUri.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                queryBuilder.Append("&state=").Append(Uri.EscapeDataString(state));
            }

            if (forceReapprove)
            {
                queryBuilder.Append("&force_reapprove=true");
            }

            if (disableSignup)
            {
                queryBuilder.Append("&disable_signup=true");
            }

            if (!string.IsNullOrWhiteSpace(requireRole))
            {
                queryBuilder.Append("&require_role=").Append(requireRole);
            }

            if (forceReauthentication)
            {
                queryBuilder.Append("&force_reauthentication=true");
            }

            queryBuilder.Append("&token_access_type=").Append(tokenAccessType.ToString().ToLower());
            

            if (scopeList != null)
            {
                queryBuilder.Append("&scope=").Append(String.Join(" ", scopeList));
            }

            if(includeGrantedScopes != IncludeGrantedScopes.None)
            {
                queryBuilder.Append("&include_granted_scopes=").Append(includeGrantedScopes.ToString().ToLower());
            }

            if (codeChallenge != null)
            {
                queryBuilder.Append("&code_challenge_method=S256&code_challenge=").Append(codeChallenge);
            }

            var uriBuilder = new UriBuilder("https://www.dropbox.com/oauth2/authorize")
            {
                Query = queryBuilder.ToString()
            };

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Gets the URI used to start the OAuth2.0 authorization flow which doesn't require a redirect URL.
        /// </summary>
        /// <param name="clientId">The apps key, found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="disableSignup">When <c>true</c> (default is <c>false</c>) users will not be able to sign up for a
        /// Dropbox account via the authorization page. Instead, the authorization page will show a link to the Dropbox
        /// iOS app in the App Store. This is only intended for use when necessary for compliance with App Store policies.</param>
        /// <returns>The uri of a web page which must be displayed to the user in order to authorize the app.</returns>
        public static Uri GetAuthorizeUri(string clientId, bool disableSignup = false)
        {
            return GetAuthorizeUri(OAuthResponseType.Code, clientId, (Uri)null, disableSignup: disableSignup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GeneratePKCECodeVerifier()
        {
            var bytes = new byte[PKCEVerifierLength];
            RandomNumberGenerator.Create().GetBytes(bytes);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_')
                .Substring(0, 128);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codeVerifier"></param>
        /// <returns></returns>
        public static string GeneratePKCECodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                return Convert.ToBase64String(challengeBytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
        }

        /// <summary>
        /// Parses the token fragment. When using the OAuth 2.0 token or implicit grant flow, the 
        /// user will be redirected to a URI with a fragment containing the authorization token.
        /// </summary>
        /// <param name="redirectedUri">The redirected URI.</param>
        /// <returns>The authorization response, containing the access token and uid of the authorized user</returns>
        public static OAuth2Response ParseTokenFragment(Uri redirectedUri)
        {
            if (redirectedUri == null)
            {
                throw new ArgumentNullException("redirectedUri");
            }

            var fragment = redirectedUri.Fragment;
            if (string.IsNullOrWhiteSpace(fragment))
            {
                throw new ArgumentException("The supplied uri doesn't contain a fragment", "redirectedUri");
            }

            fragment = fragment.TrimStart('#');

            string accessToken = null;
            string uid = null;
            string state = null;
            string tokenType = null;

            foreach (var pair in fragment.Split('&'))
            {
                var elements = pair.Split('=');
                if (elements.Length != 2)
                {
                    continue;
                }

                switch (elements[0])
                {
                    case "access_token":
                        accessToken = Uri.UnescapeDataString(elements[1]);
                        break;
                    case "uid":
                        uid = Uri.UnescapeDataString(elements[1]);
                        break;
                    case "state":
                        state = Uri.UnescapeDataString(elements[1]);
                        break;
                    case "token_type":
                        tokenType = Uri.UnescapeDataString(elements[1]);
                        break;
                }
            }

            return new OAuth2Response(accessToken, uid, state, tokenType);
        }

        /// <summary>
        /// Processes the second half of the OAuth 2.0 code flow.
        /// </summary>
        /// <param name="code">The code acquired in the query parameters of the redirect from the initial authorize url.</param>
        /// <param name="appKey">The application key, found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="appSecret">The application secret, found in the 
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a> This is optional if using PKCE.</param>
        /// <param name="redirectUri">The redirect URI that was provided in the initial authorize URI,
        /// this is only used to validate that it matches the original request, it is not used to redirect
        /// again.</param>
        /// <param name="client">An optional http client instance used to make requests.</param>
        /// <param name="codeVerifier">The code verifier for PKCE flow.  If using PKCE, please us the PKCEOauthFlow object</param>
        /// <returns>The authorization response, containing the access token and uid of the authorized user</returns>
        public static async Task<OAuth2Response> ProcessCodeFlowAsync(string code, string appKey, string appSecret = null, string redirectUri = null, HttpClient client = null, string codeVerifier = null)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("code");
            }
            else if (string.IsNullOrEmpty(appKey))
            {
                throw new ArgumentNullException("appKey");
            }
            else if (string.IsNullOrEmpty(appSecret) && string.IsNullOrEmpty(codeVerifier))
            {
                throw new ArgumentNullException("appSecret or codeVerifier");
            }

            var httpClient = client ?? new HttpClient();

            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "code", code },
                    { "grant_type", "authorization_code" },
                    { "client_id", appKey }
                };

                if(!string.IsNullOrEmpty(appSecret))
                {
                    parameters["client_secret"] = appSecret;
                }

                if (!string.IsNullOrEmpty(codeVerifier))
                {
                    parameters["code_verifier"] = codeVerifier;
                }

                if (!string.IsNullOrEmpty(redirectUri))
                {
                    parameters["redirect_uri"] = redirectUri;
                }
                var content = new FormUrlEncodedContent(parameters);
                var response = await httpClient.PostAsync("https://api.dropbox.com/oauth2/token", content).ConfigureAwait(false);

                var raw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                JObject json = JObject.Parse(raw);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new OAuth2Exception(json["error"].ToString(), json.Value<string>("error_description"));
                }

                string refreshToken = null;
                if (json.Value<string>("refresh_token") != null)
                {
                    refreshToken = json["refresh_token"].ToString();
                }

                int expiresIn = -1;
                if (json.Value<string>("expires_in") != null)
                {
                    expiresIn = json["expires_in"].ToObject<int>();
                }

                string[] scopeList = null;
                if (json.Value<string>("scope") != null)
                {
                    scopeList = json["scope"].ToString().Split(' ');
                }

                if (expiresIn == -1)
                {
                    return new OAuth2Response(
                        json["access_token"].ToString(),
                        json["uid"].ToString(),
                        null,
                        json["token_type"].ToString());
                }
                
                return new OAuth2Response(
                    json["access_token"].ToString(),
                    refreshToken,
                    json["uid"].ToString(),
                    null,
                    json["token_type"].ToString(),
                    expiresIn,
                    scopeList);
            }
            finally
            {
                if (client == null)
                {
                    httpClient.Dispose();
                }
            }
        }

        /// <summary>
        /// Processes the second half of the OAuth 2.0 code flow.
        /// </summary>
        /// <param name="responseUri">The URI to which the user was redirected after the initial authorization request.</param>
        /// <param name="appKey">The application key, found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="appSecret">The application secret, found in the 
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="redirectUri">The redirect URI that was provided in the initial authorize URI,
        /// this is only used to validate that it matches the original request, it is not used to redirect
        /// again.</param>
        /// <param name="state">The state parameter (if any) that matches that used in the initial authorize URI.</param>
        /// <param name="client">An optional http client instance used to make requests.</param>
        /// <param name="codeVerifier">The code verifier for PKCE flow.  If using PKCE, please us the PKCEOauthFlow object</param>
        /// <returns>The authorization response, containing the access token and uid of the authorized user</returns>
        public static Task<OAuth2Response> ProcessCodeFlowAsync(Uri responseUri, string appKey, string appSecret, string redirectUri = null, string state = null, HttpClient client = null, string codeVerifier = null)
        {
            if (responseUri == null)
            {
                throw new ArgumentNullException("responseUri");
            }
            else if (string.IsNullOrEmpty(appKey))
            {
                throw new ArgumentNullException("appKey");
            }
            else if (string.IsNullOrEmpty(appSecret) && string.IsNullOrEmpty(codeVerifier))
            {
                throw new ArgumentNullException("appSecret or codeVerifier");
            }

            var query = responseUri.Query;
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("The redirect uri is missing expected query arguments.", "responseUri");
            }

            query = query.TrimStart('?');
            string code = null;
            foreach (var pair in query.Split('&'))
            {
                var elements = pair.Split('=');
                if (elements.Length != 2)
                {
                    continue;
                }

                switch (elements[0])
                {
                    case "code":
                        code = Uri.UnescapeDataString(elements[1]);
                        break;
                    case "state":
                        if (state == null)
                        {
                            throw new ArgumentNullException("state", "The state argument is expected.");
                        }
                        else if (state != Uri.UnescapeDataString(elements[1]))
                        {
                            throw new ArgumentException("The state in the responseUri does not match the provided value.", "responseUri");
                        }

                        break;
                }
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("The responseUri is missing a code value in the query component.", "responseUri");
            }

            return ProcessCodeFlowAsync(code, appKey, appSecret, redirectUri, client, codeVerifier);
        }
    }

    /// <summary>
    /// Object used to execute OAuth through PKCE
    /// Use this object to maintain code verifier and challenge using S256 method
    /// </summary>
    public class PKCEOAuthFlow
    {
        /// <summary>
        /// Default constructor that also generates code verifier and code challenge to be used in PKCE flow
        /// </summary>
        public PKCEOAuthFlow()
        {
            this.CodeVerifier = DropboxOAuth2Helper.GeneratePKCECodeVerifier();
            this.CodeChallenge = DropboxOAuth2Helper.GeneratePKCECodeChallenge(CodeVerifier);
        }

        private string CodeVerifier { get; set;  }
        private string CodeChallenge { get; set; }

        /// <summary>
        /// Gets the URI used to start the OAuth2.0 authorization flow.  Passes in codeChallenge generated in this class
        /// </summary>
        /// <param name="oauthResponseType">The grant type requested, either <c>Token</c> or <c>Code</c>.</param>
        /// <param name="clientId">The apps key, found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="redirectUri">Where to redirect the user after authorization has completed. This must be the exact URI
        /// registered in the <a href="https://www.dropbox.com/developers/apps">App Console</a>; even <c>localhost</c>
        /// must be listed if it is used for testing. A redirect URI is required for a token flow, but optional for code. 
        /// If the redirect URI is omitted, the code will be presented directly to the user and they will be invited to enter
        /// the information in your app.</param>
        /// <param name="state">Up to 500 bytes of arbitrary data that will be passed back to <paramref name="redirectUri"/>.
        /// This parameter should be used to protect against cross-site request forgery (CSRF).</param>
        /// <param name="forceReapprove">Whether or not to force the user to approve the app again if they've already done so.
        /// If <c>false</c> (default), a user who has already approved the application may be automatically redirected to
        /// <paramref name="redirectUri"/>If <c>true</c>, the user will not be automatically redirected and will have to approve
        /// the app again.</param>
        /// <param name="disableSignup">When <c>true</c> (default is <c>false</c>) users will not be able to sign up for a
        /// Dropbox account via the authorization page. Instead, the authorization page will show a link to the Dropbox
        /// iOS app in the App Store. This is only intended for use when necessary for compliance with App Store policies.</param>
        /// <param name="requireRole">If this parameter is specified, the user will be asked to authorize with a particular
        /// type of Dropbox account, either work for a team account or personal for a personal account. Your app should still
        /// verify the type of Dropbox account after authorization since the user could modify or remove the require_role
        /// parameter.</param>
        /// <param name="forceReauthentication"> If <c>true</c>, users will be signed out if they are currently signed in.
        /// This will make sure the user is brought to a page where they can create a new account or sign in to another account.
        /// This should only be used when there is a definite reason to believe that the user needs to sign in to a new or
        /// different account.</param>
        /// <param name="tokenAccessType">Determines the type of token to request.  See <see cref="TokenAccessType" /> 
        /// for information on specific types available.  If none is specified, this will use the legacy type.</param>
        /// <param name="scopeList">list of scopes to request in base oauth flow.  If left blank, will default to all scopes for app</param>
        /// <param name="includeGrantedScopes">which scopes to include from previous grants. Note: if this user has never linked the app, include_granted_scopes must be None</param>
        /// <returns>The uri of a web page which must be displayed to the user in order to authorize the app.</returns>
        public Uri GetAuthorizeUri(OAuthResponseType oauthResponseType, string clientId, string redirectUri = null, string state = null, bool forceReapprove = false, bool disableSignup = false, string requireRole = null, bool forceReauthentication = false, TokenAccessType tokenAccessType = TokenAccessType.Legacy, string[] scopeList = null, IncludeGrantedScopes includeGrantedScopes = IncludeGrantedScopes.None)
        {
            return DropboxOAuth2Helper.GetAuthorizeUri(oauthResponseType, clientId, redirectUri, state, forceReapprove, disableSignup, requireRole, forceReauthentication, tokenAccessType, scopeList, includeGrantedScopes, this.CodeChallenge);
        }

        /// <summary>
        /// Processes the second half of the OAuth 2.0 code flow.  Uses the codeVerifier created in this class to execute the second half.
        /// </summary>
        /// <param name="code">The code acquired in the query parameters of the redirect from the initial authorize url.</param>
        /// <param name="appKey">The application key, found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="redirectUri">The redirect URI that was provided in the initial authorize URI,
        /// this is only used to validate that it matches the original request, it is not used to redirect
        /// again.</param>
        /// <param name="client">An optional http client instance used to make requests.</param>
        /// <returns>The authorization response, containing the access token and uid of the authorized user</returns>
        public Task<OAuth2Response> ProcessCodeFlowAsync(string code, string appKey, string redirectUri = null, HttpClient client = null)
        {
            return DropboxOAuth2Helper.ProcessCodeFlowAsync(code, appKey, null, redirectUri, client, this.CodeVerifier);
        }

        /// <summary>
        /// Processes the second half of the OAuth 2.0 code flow.  Uses the codeVerifier created in this class to execute second half.
        /// </summary>
        /// <param name="responseUri">The URI to which the user was redirected after the initial authorization request.</param>
        /// <param name="appKey">The application key, found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="redirectUri">The redirect URI that was provided in the initial authorize URI,
        /// this is only used to validate that it matches the original request, it is not used to redirect
        /// again.</param>
        /// <param name="state">The state parameter (if any) that matches that used in the initial authorize URI.</param>
        /// <param name="client">An optional http client instance used to make requests.</param>
        /// <returns>The authorization response, containing the access token and uid of the authorized user</returns>
        public Task<OAuth2Response> ProcessCodeFlowAsync(Uri responseUri, string appKey, string redirectUri = null, string state = null, HttpClient client = null)
        {
            return DropboxOAuth2Helper.ProcessCodeFlowAsync(responseUri, appKey, null, redirectUri, state, client, this.CodeVerifier);
        }

    }

    /// <summary>
    /// Contains the parameters passed in a successful authorization response. 
    /// </summary>
    public sealed class OAuth2Response
    {
        internal OAuth2Response(string accessToken, string refreshToken, string uid, string state, string tokenType, int expiresIn, string[] scopeList)
        {
            if (string.IsNullOrEmpty(accessToken) || uid == null)
            {
                throw new ArgumentException("Invalid OAuth 2.0 response, missing access_token and/or uid.");
            }

            this.AccessToken = accessToken;
            this.Uid = uid;
            this.State = state;
            this.TokenType = tokenType;
            this.RefreshToken = refreshToken;
            this.ExpiresAt = DateTime.Now.AddSeconds(expiresIn);
            this.ScopeList = scopeList;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Response"/> class.
        /// </summary>
        /// <param name="accessToken">The access_token.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="state">The state.</param>
        /// <param name="tokenType">The token_type.</param>
        internal OAuth2Response(string accessToken, string uid, string state, string tokenType)
        {
            if (string.IsNullOrEmpty(accessToken) || uid == null)
            {
                throw new ArgumentException("Invalid OAuth 2.0 response, missing access_token and/or uid.");
            }

            this.AccessToken = accessToken;
            this.Uid = uid;
            this.State = state;
            this.TokenType = tokenType;
            this.RefreshToken = null;
            this.ExpiresAt = null;
            this.ScopeList = null;
        }

        /// <summary>
        /// Gets the access token, a token which can be used to make calls to the Dropbox API
        /// </summary>
        /// <remarks>
        /// Pass this as the <c>oauth2AccessToken</c> argument when creating an instance
        /// of <see cref="DropboxClient"/>.</remarks>
        /// <value>
        /// A token which can be used to make calls to the Dropbox API.
        /// </value>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the Dropbox user ID of the authorized user.
        /// </summary>
        /// <value>
        /// The Dropbox user ID of the authorized user.
        /// </value>
        public string Uid { get; private set; }

        /// <summary>
        /// Gets the state content, if any, originally passed to the authorize URI.
        /// </summary>
        /// <value>
        /// The state content, if any, originally passed to the authorize URI.
        /// </value>
        public string State { get; private set; }

        /// <summary>
        /// Gets the type of the token, which will always be <c>bearer</c> if set.
        /// </summary>
        /// <value>
        /// This will always be <c>bearer</c> if set.
        /// </value>
        public string TokenType { get; private set; }

        /// <summary>
        /// Gets the refresh token, if offline or online access type was selected.
        /// </summary>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Gets the time of expiration of the access token, if the token will expire. 
        /// This is only filled if offline or online access type was selected.
        /// </summary>
        public Nullable<DateTime> ExpiresAt { get; private set; }

        /// <summary>
        /// List of scopes this oauth2 request granted the user
        /// </summary>
        public string[] ScopeList { get; private set; }
    }

    /// <summary>
    /// Exception when error occurs during oauth2 flow.
    /// </summary>
    public sealed class OAuth2Exception : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Exception"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorDescription">The error description</param>
        public OAuth2Exception(string message, string errorDescription = null) : base(message)
        {
            this.ErrorDescription = errorDescription;
        }

        /// <summary>
        /// Gets the error description.
        /// </summary>
        public string ErrorDescription { get; private set; }
    }
}
