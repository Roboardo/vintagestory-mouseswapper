using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Vintagestory.API.Common;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("mouseswapper")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("mouseswapper")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c3c36b49-3352-413c-a278-00491e9c3ea7")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0")]
[assembly: AssemblyFileVersion("1.0")]

[assembly: ModDependency("game")]
[assembly: ModInfo(
    "MouseSwapper", "mouseswapper",
    Version = "1.0.0",
    Authors = new string[] { "Nexrem", "Lyrthras" },
    Description = "Swap left and right mouse buttons as well as their corresponding prompts",
    Website = "https://github.com/SexualReptilians/vintagestory-mouseswapper",
    Side = "client")]
