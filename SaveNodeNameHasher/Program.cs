using AndASM_NMS.Util;
using AndASM_NMS.Windows;

namespace AndASM_NMS.SaveNodeNameHasher
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			foreach (var word in args) ColorConsole.WriteKeyTabValue(word, SaveFile.HashName(word));
		}
	}
}