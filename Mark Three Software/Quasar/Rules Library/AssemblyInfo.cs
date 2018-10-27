using System.Reflection;
using System.Runtime.CompilerServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Rules Library")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Mark Three Software")]
[assembly: AssemblyProduct("Quasar")]
[assembly: AssemblyCopyright("Copyright (C) 2003 - All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

// Version information:
[assembly: AssemblyVersion("1.0.0.0")]

// Delayed Signing.  Provide a public key and a placeholder for the strong name.
// This allows other assemblies to reference this one, but allows the cryptography
// methods to ignore this module until a release.
[assembly:AssemblyDelaySignAttribute(true)]
[assembly:AssemblyKeyFileAttribute(@"..\..\public.snk")]
