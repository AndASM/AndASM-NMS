using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Windows.Storage;
using AndASM_NMS.Windows.Win32;

namespace AndASM_NMS.Windows
{
	public static class Shims
	{
		private static readonly char[] DirectorySeparators =
			{Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};

		public static StorageFolder CreateFolderPath(this StorageFolder parent, string path)
		{
			var directories = path.Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries);
			var current     = parent;
			foreach (var directory in directories)
				current = current.CreateFolderAsync(directory, CreationCollisionOption.OpenIfExists).GetAwaiter()
								 .GetResult();
			return current;
		}

		public static DirectoryInfo GetDirectoryInfo(this StorageFolder folder)
		{
			return new DirectoryInfo(folder.Path);
		}

		public static void OpenExplorer(this StorageFolder folder)
		{
			Process.Start("explorer.exe", folder.Path);
		}

		public static void CreateDesktopShortcut(this StorageFolder folder, string linkName,
												 string             linkDescription = null)
		{
			var lnkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{linkName}.lnk");
			var lnk     = new IShellLink();
			var lnkFile = (IPersistFile)lnk;

			lnk.SetPath(folder.Path);
			lnk.SetDescription(linkDescription ?? linkName);
			lnkFile.Save(lnkPath, true);
		}
	}
}