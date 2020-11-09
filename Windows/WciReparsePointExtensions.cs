using System;
using System.IO;
using System.Runtime.InteropServices;
using AndASM_NMS.Windows.Win32;
using FileAccess = AndASM_NMS.Windows.Win32.FileAccess;

namespace AndASM_NMS.Windows
{
	public static class WciReparsePointExtensions
	{
		public static void CreateWciTombstone(this FileInfo tombstoneFile)
		{
			if (tombstoneFile.Exists)
				tombstoneFile.Delete();

			using var handle = Kernel32.CreateFile(tombstoneFile.FullName, FileAccess.GenericWrite,
												   FileShare.ReadWrite | FileShare.Delete, IntPtr.Zero, FileMode.Create,
												   FileFlagsAndAttributes.Hidden | FileFlagsAndAttributes.System |
												   FileFlagsAndAttributes.OpenReparsePoint |
												   FileFlagsAndAttributes.BackupSemantics);

			if (Marshal.GetLastWin32Error() != 0)
				throw new IOException("Cannot create deletion tombstone.",
									  Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

			var buffer = new ReparseDataBufferWciTombstone
				{ReparseTag = Kernel32.IoReparseTagWciTombstone, ReparseDataLength = 0};
			Kernel32.DeviceIoControl(handle, IoControl.FsctlSetReparsePoint, buffer);

			if (Marshal.GetLastWin32Error() != 0)
				throw new IOException("Cannot apply reparse point tag.",
									  Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

			tombstoneFile.Refresh();
		}

		public static bool IsWciTombstone(this FileInfo tombstoneFile)
		{
			if (tombstoneFile.Exists)
			{
				using var handle = Kernel32.CreateFile(tombstoneFile.FullName, FileAccess.GenericRead,
													   FileShare.Read, IntPtr.Zero, FileMode.Open,
													   FileFlagsAndAttributes.OpenReparsePoint);

				if (Marshal.GetLastWin32Error() != 0)
					throw new IOException($"Cannot access file. {tombstoneFile.Name}",
										  Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

				Kernel32.DeviceIoControl(handle, IoControl.FsctlGetReparsePoint,
										 out ReparseDataBufferWciTombstone buffer);

				return buffer.ReparseTag == Kernel32.IoReparseTagWciTombstone;
			}

			return false;
		}
	}
}