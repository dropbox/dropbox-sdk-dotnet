//-----------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Dropbox.Api")]
[assembly: AssemblyDescription("Official Dropbox .Net v2 SDK")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Dropbox Inc.")]
[assembly: AssemblyProduct("Dropbox.Api")]
[assembly: AssemblyCopyright("Copyright © Dropbox Inc")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("5.0.0")]
[assembly: AssemblyFileVersion("5.0.7591")]

#if DEBUG
[assembly: InternalsVisibleTo("Dropbox.Api.Integration.Tests")]
[assembly: InternalsVisibleTo("Dropbox.Api.Unit.Tests")]
#endif
