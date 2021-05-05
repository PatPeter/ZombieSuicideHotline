using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(ZombieSuicideHotline.AssemblyInfo.Name)]
[assembly: AssemblyDescription(ZombieSuicideHotline.AssemblyInfo.Description)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Universal Gaming Alliance")]
[assembly: AssemblyProduct(ZombieSuicideHotline.AssemblyInfo.Name)]
[assembly: AssemblyCopyright("Copyright ©  2018-2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("2db81fe7-9927-4591-9f4c-21a61516a2d5")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(ZombieSuicideHotline.AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(ZombieSuicideHotline.AssemblyInfo.Version)]

namespace ZombieSuicideHotline
{
	static internal class AssemblyInfo
	{
		internal const string Author = "PatPeter and Babyboucher20";
		internal const string Name = "ZombieSuicideHotline";
		internal const string Description = "Plugin that prevents SCP-049-2 and other SCPs from committing suicide.";
		internal const string Id = "patpeter.zombie.suicide.hotline";
		internal const string ConfigPrefix = "zombie_suicide_hotline";
		internal const string LangFile = "zombie_suicide_hotline";

		/// <summary>
		/// The AssemblyFileVersion of this web part
		/// </summary>
		internal const string Version = "1.6.6.60";
	}
}
