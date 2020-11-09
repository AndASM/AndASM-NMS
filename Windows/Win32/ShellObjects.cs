using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace AndASM_NMS.Windows.Win32
{
	[SuppressUnmanagedCodeSecurity]
	[ComImport]
	[Guid("000214F9-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[CoClass(typeof(ShellLinkClass))]
	public interface IShellLink
	{
		void GetPath([MarshalAs(UnmanagedType.LPWStr)] [Out]
					 StringBuilder pszFile, [In] int cch, [In] [Out] IntPtr pfd, [In] uint fFlags);

		void GetIDList([Out] out IntPtr itemIdlListPtrPtr);
		void SetIDList([In]      IntPtr itemIdListPtr);

		void GetDescription([MarshalAs(UnmanagedType.LPWStr)] [Out]
							StringBuilder pszName, int cch);

		void SetDescription([MarshalAs(UnmanagedType.LPWStr)] [In] string pszName);

		void GetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] [Out]
								 StringBuilder pszDir, int cch);

		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] [In] string pszDir);

		void GetArguments([MarshalAs(UnmanagedType.LPWStr)] [Out]
						  StringBuilder pszArgs, [In] int cch);

		void SetArguments([MarshalAs(UnmanagedType.LPWStr)] [In] string pszArgs);
		void GetHotkey(out                                       ushort pwHotKey);
		void SetHotkey([In]                                      ushort wHotKey);
		void GetShowCmd(out                                      int    piShowCmd);
		void SetShowCmd([In]                                     int    iShowCmd);

		void GetIconLocation([MarshalAs(UnmanagedType.LPWStr)] [Out]
							 StringBuilder pszIconPath, [In] int cch, out int piIcon);

		void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] [In] string pszIconPath, [In] int  iIcon);
		void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] [In] string pszPathRel,  [In] uint dwReserved);
		void Resolve([In]                                           IntPtr hwnd,        [In] uint fFlags);
		void SetPath([MarshalAs(UnmanagedType.LPWStr)] [In]         string pszFile);
	}

	[ComImport]
	[Guid("00021401-0000-0000-C000-000000000046")]
	[ClassInterface(ClassInterfaceType.None)]
	public class ShellLinkClass
	{
	}
}