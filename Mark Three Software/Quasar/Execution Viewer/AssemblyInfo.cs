using System.Reflection;
using System.Runtime.CompilerServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Execution Viewer")]
[assembly: AssemblyDescription("This Viewer is used to view and manage Executions.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Mark Three Software")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("Copyright (c) 2003 - All rights reserved")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("1.0.0.0")]

// Delayed Signing.  Provide a public key and a placeholder for the strong name.
// This allows other assemblies to reference this one, but allows the cryptography
// methods to ignore this module until the product is released.
[assembly:AssemblyDelaySignAttribute(true)]
[assembly:AssemblyKeyFileAttribute(@"..\..\public.snk")]
