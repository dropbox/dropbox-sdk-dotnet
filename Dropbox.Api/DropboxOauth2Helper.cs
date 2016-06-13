//-----------------------------------------------------------------------------
// <copyright file="DropboxOAuth2Helper.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
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
    /// Contains methods that make authorizing with Dropbox easier.
    /// </summary>
    /// <example>
    /// <para>
    /// This shows an example of how to use the token, or implicit grant, flow.
    /// This code is part of a XAML window that contains a WebBrowser object as <c>this.Browser</c>
    /// </para>
    /// <para>
    /// The <c>Start</c> method calls <see cref="DropboxOAuth2Helper.GetAuthorizeUri" /> to create the URI that the browser component
    /// navigate to; the response type is set to <see cref="OAuthResponseType.Token"/> to create a URI for the token flow.
    /// </para>
    /// <para>
    /// The exact value of the redirect URI is not important with the code flow, only that it is registered in the
    /// <a href="https://www.dropbox.com/developers/apps">App Console</a>; it is common to use a <c>localhost</c>
    /// URI for use within a client token flow like this.
    /// </para>
    /// <para>
    /// The <c>BrowserNavigating</c> method has been attached to the <c>Navigating</c> event on the <c>WebBrowser</c> object.
    /// It first checks if the URI to which the browser is navigating starts with the redirect uri provided in the call to
    /// <see cref="DropboxOAuth2Helper.GetAuthorizeUri" /> &#x2014; it is important not to prevent other navigation which may happen within the 
    /// authorization flow &#x2014; if the URI matches, then the code uses <see cref="DropboxOAuth2Helper.ParseTokenFragment"/> to parse the 
    /// <see cref="OAuth2Response"/> from the fragment component of the redirected URI. The <see cref="OAuth2Response.AccessToken" />
    /// will then be used to construct an instance of <see cref="DropboxClient"/>.
    /// </para>
    /// <code>
    /// private void Start(string appKey)
    /// {
    ///     this.oauth2State = Guid.NewGuid().ToString("N");
    ///     Uri authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OauthResponseType.Token, appKey, RedirectUri, state: oauth2State);
    ///     this.Browser.Navigate(authorizeUri);
    /// }
    ///
    /// private void BrowserNavigating(object sender, NavigatingCancelEventArgs e)
    /// {
    ///     if (!e.Uri.ToString().StartsWith(RedirectUri, StringComparison.OrdinalIgnoreCase))
    ///     {
    ///         // we need to ignore all navigation that isn't to the redirect uri.
    ///         return;
    ///     }
    ///
    ///     try
    ///     {
    ///         OAuth2Response result = DropboxOAuth2Helper.ParseTokenFragment(e.Uri);
    ///         if (result.State != this.oauth2State)
    ///         {
    ///             // The state in the response doesn't match the state in the request.
    ///             return;
    ///         }
    ///
    ///         this.AccessToken = result.AccessToken;
    ///         this.Uid = result.Uid;
    ///         this.Result = true;
    ///     }
    ///     catch (ArgumentException)
    ///     {
    ///         // There was an error in the URI passed to ParseTokenFragment
    ///     }
    ///     finally
    ///     {
    ///         e.Cancel = true;
    ///         this.Close();
    ///     }
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
        /// <paramref name="redirectUri"/>. If <c>true</c>, the user will not be automatically redirected and will have to approve
        /// the app again.</param>
        /// <param name="disableSignup">When <c>true</c> (default is <c>false</c>) users will not be able to sign up for a
        /// Dropbox account via the authorization page. Instead, the authorization page will show a link to the Dropbox
        /// iOS app in the App Store. This is only intended for use when necessary for compliance with App Store policies.</param>
        /// <returns>The uri of a web page which must be displayed to the user in order to authorize the app.</returns>
        public static Uri GetAuthorizeUri(OAuthResponseType oauthResponseType, string clientId, string redirectUri = null, string state = null, bool forceReapprove = false, bool disableSignup = false)
        {
            var uri = string.IsNullOrEmpty(redirectUri) ? null : new Uri(redirectUri);

            return GetAuthorizeUri(oauthResponseType, clientId, uri, state, forceReapprove, disableSignup);
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
        /// <paramref name="redirectUri"/>. If <c>true</c>, the user will not be automatically redirected and will have to approve
        /// the app again.</param>
        /// <param name="disableSignup">When <c>true</c> (default is <c>false</c>) users will not be able to sign up for a
        /// Dropbox account via the authorization page. Instead, the authorization page will show a link to the Dropbox
        /// iOS app in the App Store. This is only intended for use when necessary for compliance with App Store policies.</param>
        /// <returns>The uri of a web page which must be displayed to the user in order to authorize the app.</returns>
        public static Uri GetAuthorizeUri(OAuthResponseType oauthResponseType, string clientId, Uri redirectUri = null, string state = null, bool forceReapprove = false, bool disableSignup = false)
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

            var uriBuilder = new UriBuilder("https://www.dropbox.com/1/oauth2/authorize")
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
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="redirectUri">The redirect URI that was provided in the initial authorize URI,
        /// this is only used to validate that it matches the original request, it is not used to redirect
        /// again.</param>
        /// <param name="client">An optional http client instance used to make requests.</param>
        /// <returns>The authorization response, containing the access token and uid of the authorized user</returns>
        public static async Task<OAuth2Response> ProcessCodeFlowAsync(string code, string appKey, string appSecret, string redirectUri = null, HttpClient client = null)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("code");
            }
            else if (string.IsNullOrEmpty(appKey))
            {
                throw new ArgumentNullException("appKey");
            }
            else if (string.IsNullOrEmpty(appSecret))
            {
                throw new ArgumentNullException("appSecret");
            }

            var httpClient = client ?? new HttpClient();
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "code", code },
                    { "grant_type", "authorization_code" },
                    { "client_id", appKey },
                    { "client_secret", appSecret }
                };

                if (!string.IsNullOrEmpty(redirectUri))
                {
                    parameters["redirect_uri"] = redirectUri;
                }

                var content = new FormUrlEncodedContent(parameters);
                var response = await httpClient.PostAsync("https://api.dropbox.com/1/oauth2/token", content);

                var raw = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(raw);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new OAuth2Exception(json["error"].ToString(), json.Value<string>("error_description"));
                }

                return new OAuth2Response(
                    json["access_token"].ToString(),
                    json["uid"].ToString(),
                    null,
                    json["token_type"].ToString());
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
        /// <returns>The authorization response, containing the access token and uid of the authorized user</returns>
        public static Task<OAuth2Response> ProcessCodeFlowAsync(Uri responseUri, string appKey, string appSecret, string redirectUri = null, string state = null, HttpClient client = null)
        {
            if (responseUri == null)
            {
                throw new ArgumentNullException("responseUri");
            }
            else if (string.IsNullOrEmpty(appKey))
            {
                throw new ArgumentNullException("appKey");
            }
            else if (string.IsNullOrEmpty(appSecret))
            {
                throw new ArgumentNullException("appSecret");
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

            return ProcessCodeFlowAsync(code, appKey, appSecret, redirectUri, client);
        }
    }

    /// <summary>
    /// Contains the parameters passed in a successful authorization response. 
    /// </summary>
    public sealed class OAuth2Response
    {
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
