﻿//-----------------------------------------------------------------------------
// <copyright file="UnitTests.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Unit.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// All the unit tests.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// Test get authorization url.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1000:KeywordsMustBeSpacedCorrectly", Justification = "False positive.")]
        [TestMethod]
        public void TestGetAuthorizationUri()
        {
            string clientId = "myclientid";
            string[] redirectUris = new[] { string.Empty, "http://127.0.0.1:52475/" };
            string[] states = new[] { string.Empty, "state" };
            bool[] forceReapproves = new[] { false, true };
            bool[] disableSignups = new[] { false, true };
            string[] requireRoles = new[] { string.Empty, "role" };
            bool[] forceReauthentications = new[] { false, true };
            List<string[]> scopes = new()
            {
                null,
                new string[] { "files.metadata.read", "files.content.read" },
            };
            IncludeGrantedScopes[] includeGrantedScopes = new[] { IncludeGrantedScopes.None, IncludeGrantedScopes.User, IncludeGrantedScopes.Team };

            TokenAccessType[] tokenAccessTypes = new[]
                {
                    TokenAccessType.Legacy, TokenAccessType.Offline, TokenAccessType.Online,
                };
            foreach (string redirectUri in redirectUris)
            {
                foreach (var state in states)
                {
                    foreach (var forceReapprove in forceReapproves)
                    {
                        foreach (var disableSignup in disableSignups)
                        {
                            foreach (var requireRole in requireRoles)
                            {
                                foreach (var forceReauthentication in forceReauthentications)
                                {
                                    foreach (var tokenAccessType in tokenAccessTypes)
                                    {
                                        foreach (var scope in scopes)
                                        {
                                            foreach (var includeGrantedScope in includeGrantedScopes)
                                            {
                                                var authUri = DropboxOAuth2Helper.GetAuthorizeUri(
                                                    OAuthResponseType.Code,
                                                    clientId,
                                                    redirectUri,
                                                    state,
                                                    forceReapprove,
                                                    disableSignup,
                                                    requireRole,
                                                    forceReauthentication,
                                                    tokenAccessType,
                                                    scope,
                                                    includeGrantedScope)
                                                .ToString();

                                                Assert.IsTrue(authUri.StartsWith("https://www.dropbox.com/oauth2/authorize"));
                                                Assert.IsTrue(authUri.Contains("response_type=code"));
                                                Assert.IsTrue(authUri.Contains("client_id=" + clientId));

                                                if (string.IsNullOrWhiteSpace(state))
                                                {
                                                    Assert.IsFalse(authUri.Contains("&state="));
                                                }
                                                else
                                                {
                                                    Assert.IsTrue(authUri.Contains("&state=" + state));
                                                }

                                                if (string.IsNullOrWhiteSpace(redirectUri))
                                                {
                                                    Assert.IsFalse(authUri.Contains("&redirect_uri="));
                                                }
                                                else
                                                {
                                                    Assert.IsTrue(authUri.Contains("&redirect_uri=" + Uri.EscapeDataString(redirectUri)));
                                                }

                                                if (forceReapprove)
                                                {
                                                    Assert.IsTrue(authUri.Contains("&force_reapprove=true"));
                                                }
                                                else
                                                {
                                                    Assert.IsFalse(authUri.Contains("&force_reapprove="));
                                                }

                                                if (disableSignup)
                                                {
                                                    Assert.IsTrue(authUri.Contains("&disable_signup=true"));
                                                }
                                                else
                                                {
                                                    Assert.IsFalse(authUri.Contains("&disable_signup="));
                                                }

                                                if (string.IsNullOrWhiteSpace(requireRole))
                                                {
                                                    Assert.IsFalse(authUri.Contains("&require_role="));
                                                }
                                                else
                                                {
                                                    Assert.IsTrue(authUri.Contains("&require_role=" + requireRole));
                                                }

                                                if (forceReauthentication)
                                                {
                                                    Assert.IsTrue(authUri.Contains("&force_reauthentication=true"));
                                                }
                                                else
                                                {
                                                    Assert.IsFalse(authUri.Contains("&force_reauthentication="));
                                                }

                                                Assert.IsTrue(authUri.Contains("&token_access_type=" +
                                                                                tokenAccessType.ToString().ToLower()));

                                                if (scope != null)
                                                {
                                                    Assert.IsTrue(authUri.Contains("&scope=" + string.Join(" ", scope)));
                                                }
                                                else
                                                {
                                                    Assert.IsFalse(authUri.Contains("&scope="));
                                                }

                                                if (includeGrantedScope != IncludeGrantedScopes.None)
                                                {
                                                    Assert.IsTrue(authUri.Contains("&include_granted_scopes=" +
                                                                                    includeGrantedScope.ToString().ToLower()));
                                                }
                                                else
                                                {
                                                    Assert.IsFalse(authUri.Contains("&include_granted_scopes="));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
