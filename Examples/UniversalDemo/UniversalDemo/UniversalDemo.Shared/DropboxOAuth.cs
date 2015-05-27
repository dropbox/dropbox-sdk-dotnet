//-----------------------------------------------------------------------------
// <copyright file="DropboxOAuth.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace UniversalDemo
{
    using System;
    using System.Threading.Tasks;
    using Dropbox.Api;
    using Windows.ApplicationModel.Activation;
    using Windows.Security.Authentication.Web;
    using Windows.UI.Xaml;

    /// <summary>
    /// A static class that abstracts the Dropbox OAuth logic.
    /// </summary>
    public static class DropboxOAuth
    {
#if WINDOWS_PHONE_APP
        /// <summary>
        /// Startes the authorization process which will continue through an app 
        /// activation.
        /// </summary>
        public static void AuthorizeAndContinue()
        {
            var authUri = DropboxOAuth2Helper.GetAuthorizeUri(
                OAuthResponseType.Token,
                App.AppKey,
                App.RedirectUri);

            WebAuthenticationBroker.AuthenticateAndContinue(
                authUri,
                App.RedirectUri);
        }

        /// <summary>
        /// Processes the continuation.
        /// </summary>
        /// <param name="args">The <see cref="WebAuthenticationBrokerContinuationEventArgs"/> instance containing the event data.</param>
        public static void ProcessContinuation(WebAuthenticationBrokerContinuationEventArgs args)
        {
            var result = args.WebAuthenticationResult;
            ProcessResult(result);
        }
#endif

#if WINDOWS_APP
        /// <summary>
        /// Runs the Dropbox OAuth authorization process.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        public static async Task Authorize()
        {
            var authUri = DropboxOAuth2Helper.GetAuthorizeUri(
                OAuthResponseType.Token,
                App.AppKey,
                App.RedirectUri);
            var result = await WebAuthenticationBroker.AuthenticateAsync(
                WebAuthenticationOptions.None,
                authUri,
                App.RedirectUri);

            ProcessResult(result);
        }
#endif

        /// <summary>
        /// Processes the result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="UniversalDemo.OAuthException">
        /// Raised if there was an HTTP error during the authentication process.
        /// </exception>
        /// <exception cref="UniversalDemo.OAuthUserCancelledException">
        /// Raised if the user cancelled authentication.
        /// </exception>
        private static void ProcessResult(WebAuthenticationResult result)
        {
            switch (result.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    var response = DropboxOAuth2Helper.ParseTokenFragment(
                        new Uri(result.ResponseData));
                    ((App)Application.Current).AccessToken = response.AccessToken;
                    break;

                case WebAuthenticationStatus.ErrorHttp:
                    throw new OAuthException(result.ResponseErrorDetail);

                case WebAuthenticationStatus.UserCancel:
                default:
                    throw new OAuthUserCancelledException();
            }
        }

        /// <summary>
        /// An exception raised if there was an HTTP error in the authentication process. 
        /// </summary>
        public class OAuthException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OAuthException"/> class.
            /// </summary>
            /// <param name="statusCode">The HTTP status code.</param>
            public OAuthException(uint statusCode)
            {
                this.StatusCode = statusCode;
            }

            /// <summary>
            /// Gets the HTTP status code.
            /// </summary>
            public uint StatusCode { get; private set; }
        }

        /// <summary>
        /// An exception raised if the user cancelled the authentication.
        /// </summary>
        public class OAuthUserCancelledException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OAuthUserCancelledException"/> class.
            /// </summary>
            public OAuthUserCancelledException()
            {
            }
        }
    }
}