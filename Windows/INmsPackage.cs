using System.ComponentModel;
using System.IO;

namespace AndASM_NMS.Windows
{
	public interface INmsPackage : INotifyPropertyChanged
	{
		public DirectoryInfo InstallDirectory      { get; }
		public DirectoryInfo PcBanksBanksDirectory { get; }
		public DirectoryInfo ModsDirectory         { get; }
		public bool          IsInstalled           { get; }
		public bool          IsModdingEnabled      { get; set; }
	}
}