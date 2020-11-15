using System;
using System.IO;
using AndASM_NMS.PSArc;

namespace APakTool
{
	internal static class Program
    {
		private static void Main(string[] args)
		{
			var pakFileStream = new FileStream(@"E:\No Man's Sky\AMUMSS-3.6.0W-957-3-6-0W\Builds\ZZZCombinedMod__201010-132032.pak", FileMode.Open, FileAccess.Read, FileShare.Read);
			var pakFile = new PakArchive(pakFileStream);

			Console.WriteLine("Hello World!");
        }
    }
}
