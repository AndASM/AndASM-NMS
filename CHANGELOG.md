# Changelog
Changelog format: [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
Version scheme: [Semantic Versioning](https://semver.org/spec/v2.0.0.html)
Versioning is handled by GitVersion

## [Unreleased]
### Added
- EnableMSStoreMods project
  * This is a GUI replacement for ANMSMEMSPC. A mod enabler for the Microsoft Store / GamePass PC edition.
  * Using WPF XAML for the GUI
- Windows project
  * All Windows-specific code. Especially the UWP/AppX/Microsoft Store mess.
  * Color Console
- Util project
  * Contains some shims
  * The hashing algorithm for the 3 letter codes in the JSON save files.
- PSArc project
  * Kaitai Struct based parser for the PSArc PAK archive files
- SaveNodeNameHasher
  * A commandline utility that takes names as arguments and outputs each name{tab}hash on a line.

[Unreleased]: https://github.com/AndASM/AndASM-NMS
[0.1.0]: https://github.com/AndASM/AndASM-NMS/releases/v0.1.0