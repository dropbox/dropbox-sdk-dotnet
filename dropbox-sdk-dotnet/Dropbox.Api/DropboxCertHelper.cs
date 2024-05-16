//-----------------------------------------------------------------------------
// <copyright file="DropboxCertHelper.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Deprecated: certificate pinning is no longer handled by the SDK. This class is kept
    /// to not break existing code, but certificate checks are no longer done.
    /// Helper methods that can be used to implement certificate pinning.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Dropbox recommends that all clients implement certificate pinning and this class provides implementation for desktop
    /// and server application as <see cref="DropboxCertHelper.InitializeCertPinning"/>. Unfortunately it isn't currently
    /// possible to implement this in a portable assembly, so this class also provides methods to help implement this.</para>
    /// <para>
    /// For more information about certificate pinning see
    /// <a href="https://www.owasp.org/index.php/Certificate_and_Public_Key_Pinning">Certificate and Public Key Pinning</a>.
    /// </para>
    /// <para>
    /// These helper methods allow client code to check if the certificate used by a Dropbox server
    /// was issued with a certificate chain that originates with a root certificate that Dropbox
    /// either currently uses, or may use in the future. These methods would be called before calling
    /// the <see cref="DropboxClient"/> constructor.
    /// </para>
    /// </remarks>
    [Obsolete("Certificate pinning is no longer offered via DropboxCertHelper")]
    public static class DropboxCertHelper
    {
        /// <summary>
        /// Deprecated: always returns true.
        /// Determines whether the specified public key string is a known root certificate public key.
        /// </summary>
        /// <param name="publicKeyString">The public key string.</param>
        /// <returns><c>true</c>.</returns>
        [Obsolete("Certificate check is no longer performed via IsKnownRootCertPublicKey")]
        public static bool IsKnownRootCertPublicKey(string publicKeyString)
        {
            return true;
        }

        /// <summary>
        /// Deprecated: always returns true.
        /// Determines whether the specified public key is a known root certificate public key.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <returns><c>true</c>.</returns>
        [Obsolete("Certificate check is no longer performed via IsKnownRootCertPublicKey")]
        public static bool IsKnownRootCertPublicKey(byte[] publicKey)
        {
            return true;
        }

#if PORTABLE
#elif PORTABLE40
#else
        /// <summary>
        /// Deprecated: does nothing.
        /// Initializes ssl certificate pinning.
        /// </summary>
        [Obsolete("Certificate pinning is no longer performed via InitializeCertPinning")]
        public static void InitializeCertPinning()
        {
            // Do nothing
        }
#endif

    }
}
