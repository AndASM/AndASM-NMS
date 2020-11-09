using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Management.Core;
using Windows.Management.Deployment;
using Windows.Storage;
using Microsoft.Win32;

namespace AndASM_NMS.Windows
{
	public class NmsUwpPackage : INmsPackage
	{
		private readonly ApplicationData _appData;
		private readonly HashSet<string> _capabilities;
		private readonly FileInfo        _disableModsTxt;

		private readonly Package _uwpPackage;

		public NmsUwpPackage()
		{
			var packageManager = new PackageManager();
			_uwpPackage = packageManager.FindPackagesForUser("").FirstOrDefault(package =>
				package.Id.Name.StartsWith("HelloGames.NoMansSky", StringComparison.OrdinalIgnoreCase));
			if (_uwpPackage == null)
			{
				var packages = Registry.CurrentUser.OpenSubKey(
					@"SOFTWARE\Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\Repository\Packages");
				var uwpName = packages.GetSubKeyNames().First(name =>
																  name.IndexOf(
																	  "HelloGames.NoMansSky",
																	  StringComparison.OrdinalIgnoreCase) >= 0);
				_uwpPackage = packageManager.FindPackageForUser("", uwpName);
			}

			if (_uwpPackage != null)
			{
				XElement gameManifest;
				using var manifestFileStream = _uwpPackage.InstalledLocation.GetFileAsync(@"AppxManifest.xml")
														  .GetAwaiter()
														  .GetResult().OpenStreamForReadAsync().GetAwaiter()
														  .GetResult();
				gameManifest = XElement.Load(manifestFileStream);

				_capabilities = gameManifest.DescendantsAndSelf()
											.Where(element => element.Name.LocalName == "Capability")
											.Select(element => element.Attribute("Name")?.Value).ToHashSet();
				_appData = ApplicationDataManager.CreateForPackageFamily(FamilyName);

				InstallFolder = _appData.LocalCacheFolder.CreateFolderPath(@"Local\Microsoft\WritablePackageRoot");
				PcBanksFolder = InstallFolder.CreateFolderPath(@"GAMEDATA\PCBANKS");
				ModsFolder    = PcBanksFolder.CreateFolderPath(@"MODS");

				_disableModsTxt = new FileInfo(Path.Combine(PcBanksFolder.Path, @"DISABLEMODS.TXT"));

				IsInstalled = true;
			}
			else
				IsInstalled = false;
		}

		public IReadOnlyCollection<string> Capabilities => _capabilities;

		public string FamilyName => _uwpPackage.Id.FamilyName;

		public StorageFolder PcBanksFolder { get; }
		public StorageFolder InstallFolder { get; }
		public StorageFolder ModsFolder    { get; }

		public event PropertyChangedEventHandler PropertyChanged;

		public DirectoryInfo InstallDirectory => InstallFolder?.GetDirectoryInfo();

		public DirectoryInfo PcBanksBanksDirectory => PcBanksFolder?.GetDirectoryInfo();

		public DirectoryInfo ModsDirectory => ModsFolder?.GetDirectoryInfo();

		public bool IsInstalled { get; }

		public bool IsModdingEnabled {
			get {
				if (_disableModsTxt.Exists)
					return _disableModsTxt.IsWciTombstone();
				return false; // If it doesn't exist, it'll be read from the MSIXVC and so does exist.
			}
			set {
				if (value)
					_disableModsTxt.CreateWciTombstone();
				else if (_disableModsTxt.Exists)
				{
					_disableModsTxt.Delete();
					_disableModsTxt.Refresh(); // For some reason Delete doesn't invalidate the cache!
				}

				NotifyPropertyChanged();
			}
		}

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}