using System;
using System.Runtime.InteropServices;

namespace AndASM_NMS.Windows.Win32
{
	internal class ReferenceStruct<TStruct> : IDisposable
		where TStruct : struct
	{
		private readonly IntPtr _ptr;

		public ReferenceStruct()
		{
			_ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TStruct>());
			Marshal.StructureToPtr<TStruct>(default, _ptr, false);
		}

		public ReferenceStruct(TStruct obj)
		{
			Copy(obj);
		}

		public uint Size => (uint)Marshal.SizeOf<TStruct>();

		public void Dispose()
		{
			Marshal.DestroyStructure<TStruct>(_ptr);
			Marshal.FreeHGlobal(_ptr);
		}

		public void Copy(TStruct obj)
		{
			Marshal.StructureToPtr(obj, _ptr, true);
		}

		public static implicit operator IntPtr(ReferenceStruct<TStruct> @this)
		{
			return @this._ptr;
		}

		public static implicit operator TStruct(ReferenceStruct<TStruct> @this)
		{
			return Marshal.PtrToStructure<TStruct>(@this._ptr);
		}
	}
}