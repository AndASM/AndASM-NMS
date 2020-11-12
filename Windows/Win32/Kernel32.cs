using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace AndASM_NMS.Windows.Win32
{
	public enum IoControl : uint
	{
		FsctlGetReparsePoint = 0x000900A8,
		FsctlSetReparsePoint = 0x000900A4
	}

	[Flags]
	public enum FileFlagsAndAttributes : uint
	{
		Readonly           = 2 ^ 0,
		Hidden             = 2 ^ 1,
		System             = 2 ^ 2,
		Directory          = 2 ^ 4,
		Archive            = 2 ^ 5,
		Device             = 2 ^ 6,
		Normal             = 2 ^ 7,
		Temporary          = 2 ^ 8,
		SparseFile         = 2 ^ 9,
		ReparsePoint       = 2 ^ 10,
		Compressed         = 2 ^ 11,
		Offline            = 2 ^ 12,
		NotContentIndexed  = 2 ^ 13,
		Encrypted          = 2 ^ 14,
		IntegrityStream    = 2 ^ 15,
		Virtual            = 2 ^ 16,
		NoScrubData        = 2 ^ 17,
		RecallOnOpen       = 2 ^ 18,
		Pinned             = 2 ^ 19,
		Unpinned           = 20 ^ 20,
		OpenReparsePoint   = 2 ^ 21,
		RecallOnDataAccess = 2 ^ 22,
		SessionAware       = 2 ^ 23,
		PosixSemantics     = 2 ^ 24,
		BackupSemantics    = 2 ^ 25,
		DeleteOnClose      = 2 ^ 26,
		SequentialScan     = 2 ^ 27,
		RandomAccess       = 2 ^ 28,
		StrictlySequential = 2 ^ 29,
		Overlapped         = 2 ^ 30,
		WriteThrough       = 2 ^ 31,
		SecurityAnonymous  = 0,
		NoBuffering        = StrictlySequential,
		OpenNoRecall       = Unpinned,
		FirstPipeInstance  = Pinned
	}

	[Flags]
	public enum FileAccess : uint
	{
		FileReadData        = 0x00000001,
		FileWriteData       = 0x00000002,
		FileAppendData      = 0x00000004,
		FileReadEa          = 0x00000008,
		FileWriteEa         = 0x00000010,
		FileExecute         = 0x00000020,
		FileDeleteChild     = 0x00000040,
		FileReadAttributes  = 0x00000080,
		FileWriteAttributes = 0x00000100,
		SpecificRightsAll   = 0x0000FFFF,
		FileGenericRead     = 0x00120089,
		FileGenericExecute  = 0x001200A0,
		FileGenericWrite    = 0x00120116,
		FileAllAccess       = 0x001F01FF,
		GenericAll          = 0x10000000,
		GenericExecute      = 0x20000000,
		GenericWrite        = 0x40000000,
		GenericRead         = 0x80000000
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct ReparseDataBufferWciTombstone
	{
		public uint   ReparseTag;
		public ushort ReparseDataLength;
		public ushort Reserved;
	}


	internal static class Kernel32
	{
		public const uint IoReparseTagWciTombstone = 0xA000001F;

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeviceIoControl(SafeHandle hDevice,         uint dwIoControlCode, IntPtr lpInBuffer,
												   uint       nInBufferSize,   IntPtr lpOutBuffer, uint nOutBufferSize,
												   out uint   lpBytesReturned, IntPtr lpOverlapped);

		public static bool DeviceIoControl<TOut>(SafeHandle hDev, IoControl ioControlCode, out TOut outVal)
			where TOut : struct
		{
			using var referenceStruct = new ReferenceStruct<TOut>();
			var retVal = DeviceIoControl(hDev, (uint)ioControlCode, IntPtr.Zero, 0U,
										 referenceStruct, referenceStruct.Size, out var _, IntPtr.Zero);
			outVal = referenceStruct;
			return retVal;
		}

		public static bool DeviceIoControl<TIn>(SafeHandle hDev, IoControl ioControlCode, TIn inVal)
			where TIn : struct
		{
			using var referenceStruct = new ReferenceStruct<TIn>(inVal);
			return DeviceIoControl(hDev, (uint)ioControlCode, referenceStruct, referenceStruct.Size,
								   IntPtr.Zero, 0U, out var _, IntPtr.Zero);
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern SafeFileHandle CreateFile(
			string                 lpFileName,
			FileAccess             dwDesiredAccess,
			FileShare              dwShareMode,
			[Optional] IntPtr      lpSecurityAttributes,
			FileMode               dwCreationDisposition,
			FileFlagsAndAttributes dwFlagsAndAttributes,
			[Optional] SafeHandle  hTemplateFile);
	}
}