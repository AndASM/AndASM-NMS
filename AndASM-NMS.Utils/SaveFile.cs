using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using B2R2;
using B2R2.FrontEnd;
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

		public static HashSet<string> ReadNMS()
		{
			var isIdentifier = new Regex(@"^[A-Z]{1,4}([a-z][a-z0-9_-]*([A-Z][a-z0-9_-]*)*)?$",
				RegexOptions.CultureInvariant | RegexOptions.Compiled);
			var handler = BinHandler.Init(ISA.OfString("amd64"),
				@"E:\SteamLibrary\steamapps\common\No Man's Sky\Binaries\NMS.exe");

			var rdata = handler.FileInfo.GetSections(".rdata").First();

			var address = rdata.Address;
			var endAddr = rdata.Address + rdata.Size;

			var stringList = new List<string>();
			var listList = new List<List<string>> {stringList};
			while (address < endAddr)
			{
				string str;
				try
				{
					str = handler.ReadASCII(address);
				}
				catch
				{
					break;
				}

				if (str.Length > 0)
				{
					address += (ulong)str.Length;
					if (isIdentifier.IsMatch(str))
						stringList.Add(str);
					else if (stringList.Count > 0)
					{
						stringList = new List<string>();
						listList.Add(stringList);
					}
				}
				else
					address += 1;
			}

			return new HashSet<string>(listList
				.SelectMany(L => L));
		}
	}
}