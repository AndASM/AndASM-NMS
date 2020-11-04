using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage;
using Vanara.Windows.Shell;

namespace AndASM_NMS.Windows
{
	public static class Shims
	{
		private static readonly char[] DirectorySeparators =
			{Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};

		public static StorageFolder CreateFolderPath(this StorageFolder parent, string path)
		{
			var directories = path.Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries);
			var current = parent;
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

		public static void CreateDesktopShortcut(this StorageFolder folder)
		{
			ShellLink.Create(Path.Combine(ShellFolder.Desktop.FileSystemPath, "No Man's Sky Mods.lnk"), folder.Path,
				"No Man's Sky Mods");
		}
	}
}