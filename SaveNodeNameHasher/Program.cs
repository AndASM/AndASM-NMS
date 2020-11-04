using System;
using System.Linq;
using AndASM_NMS.Util;

namespace AndASM_NMS.SaveNodeNameHasher
{
	internal class Program
	{
		private static void Main()
		{
			var foundWords = SaveFile.ReadNMS().Select(word => (SaveFile.HashName(word), word)).GroupBy(x => x.Item1)
				.Where(g => g.Count() > 1);

			Console.WriteLine(foundWords);
		}
	}
}