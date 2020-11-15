using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaitai;

namespace AndASM_NMS.PSArc
{
	public class PakArchive
	{

		private static byte[] _magicHeader = Encoding.UTF8.GetBytes("PSAR");
		private        Stream _stream;
		private        byte[] _buffer;

		public PakArchive(Stream fileStream)
		{
			_stream = fileStream;
			_buffer = new byte[1024];
			var   buffReadAsync = _stream.ReadAsync(_buffer, 0, _buffer.Length);
			buffReadAsync.Wait();

			if (!_magicHeader.SequenceEqual(_buffer.Take(_magicHeader.Length)))
			{
				throw new FileLoadException("Not a PSArc PAK File.");
			}
		}
	}
}
