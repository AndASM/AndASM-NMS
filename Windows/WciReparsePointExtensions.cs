using System.IO;
using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace AndASM_NMS.Windows
{
	public static class WciReparsePointExtensions
	{
		private const uint IoReparseTagWciTombstone = 0xA000001F;

		public static void CreateWciTombstone(this FileInfo tombstoneFile)
		{
			if (tombstoneFile.Exists)
				tombstoneFile.Delete();

			using var handle = Kernel32.CreateFile(tombstoneFile.FullName, Kernel32.FileAccess.GENERIC_WRITE,
				FileShare.ReadWrite | FileShare.Delete, null, FileMode.Create,
				FileFlagsAndAttributes.FILE_ATTRIBUTE_HIDDEN | FileFlagsAndAttributes.FILE_ATTRIBUTE_SYSTEM |
				FileFlagsAndAttributes.FILE_FLAG_OPEN_REPARSE_POINT |
				FileFlagsAndAttributes.FILE_FLAG_BACKUP_SEMANTICS);

			if (Marshal.GetLastWin32Error() != 0)
				throw new IOException("Cannot create deletion tombstone.",
					Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

			var buffer = new ReparseDataBufferWciTombstone
				{ReparseTag = IoReparseTagWciTombstone, ReparseDataLength = 0};
			Kernel32.DeviceIoControl(handle, Kernel32.IOControlCode.FSCTL_SET_REPARSE_POINT, buffer);

			if (Marshal.GetLastWin32Error() != 0)
				throw new IOException("Cannot apply reparse point tag.",
					Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

			tombstoneFile.Refresh();
		}

		public static bool IsWciTombstone(this FileInfo tombstoneFile)
		{
			if (tombstoneFile.Exists)
			{
				using var handle = Kernel32.CreateFile(tombstoneFile.FullName, Kernel32.FileAccess.FILE_GENERIC_READ,
					FileShare.Read, null, FileMode.Open, FileFlagsAndAttributes.FILE_FLAG_OPEN_REPARSE_POINT);

				if (Marshal.GetLastWin32Error() != 0)
					throw new IOException($"Cannot access file. {tombstoneFile.Name}",
						Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

				Kernel32.DeviceIoControl(handle, Kernel32.IOControlCode.FSCTL_GET_REPARSE_POINT,
					out ReparseDataBufferWciTombstone buffer);

				return buffer.ReparseTag == IoReparseTagWciTombstone;
			}

			return false;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct ReparseDataBufferWciTombstone
		{
			public uint ReparseTag;
			public ushort ReparseDataLength;
			public ushort Reserved;
		}
	}
}