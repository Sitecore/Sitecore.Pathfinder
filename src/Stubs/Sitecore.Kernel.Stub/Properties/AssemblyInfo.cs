using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.CompilerServices;

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

[assembly: InternalsVisibleTo("Sitecore.Nexus")]
[assembly: InternalsVisibleTo("Sitecore.Client")]
[assembly: InternalsVisibleTo("Sitecore.UnitTest")]
[assembly: InternalsVisibleTo("Sitecore.IsolatedTests")]
[assembly: InternalsVisibleTo("Sitecore.Kernel.FunctionalTests")]
[assembly: InternalsVisibleTo("Sitecore.Speak.ContentTesting")]
#if DEBUG
[assembly: InternalsVisibleTo("Sitecore.Kernel.IntegrationTests")]
#endif

[assembly: AssemblyTitle("Sitecore.Kernel")]
[assembly: AssemblyDescription("Sitecore CMS Kernel Library.")]
[assembly: AssemblyVersion("8.1.0.0")]

[assembly: InternalsVisibleTo("Sitecore.Analytics")]
[assembly: Guid("BF380DA4-62B5-4E04-AEB6-312B66B6B5CD")]

