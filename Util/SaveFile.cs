using System.Text;
using SpookilySharp;

namespace AndASM_NMS.Util
{
	public class SaveFile
	{
		public static string HashName(string name)
		{
			var output = new byte[3];
			var message = Encoding.UTF8.GetBytes(name);
			var hash = message.SpookyHash128(0, message.Length, 8268756125562466087, 8268756125562466087);

			// Character set starts at '0' UTF-8, 68 characters, with an offset of +6 after 'Z'
			// Character set: "0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxy"
			output[0] = (byte)(hash.UHash1 % 68 + '0');
			output[1] = (byte)((hash.UHash1 >> 21) % 68 + '0');
			output[2] = (byte)((hash.UHash1 >> 42) % 68 + '0');

			for (var i = 0; i < output.Length; i++)
				if (output[i] > 'Z')
					output[i] += 6;

			return Encoding.UTF8.GetString(output);
		}
	}
}