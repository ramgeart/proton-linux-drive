# Proton Drive for Linux

This is a Linux port of the Proton Drive Windows application, built using Avalonia UI for cross-platform compatibility.

## Overview

The Linux port includes:
- **ProtonDrive.App.Linux**: Main Avalonia-based application
- **ProtonDrive.Native.Linux**: Linux-specific native functionality
- **ProtonDrive.Sync.Linux**: File synchronization for Linux

## Architecture

The application is built on:
- **.NET 9.0** runtime
- **Avalonia UI 11.2** for cross-platform UI
- **D-Bus** for Linux system integration
- Standard Linux file system APIs

## Current Status

This is an initial port with the following features:

### Implemented
- ✅ Basic application structure
- ✅ Avalonia UI framework integration
- ✅ Linux-specific project structure
- ✅ Flatpak packaging configuration
- ✅ GitHub Actions CI/CD pipeline

### In Progress / Planned
- ⏳ Full UI conversion from WPF to Avalonia
- ⏳ Linux file system integration
- ⏳ D-Bus notification support
- ⏳ GNOME/KDE desktop integration
- ⏳ WebAuthn/FIDO2 support for Linux
- ⏳ Secret storage integration (libsecret)
- ⏳ File watching with inotify

## Requirements

### Runtime Requirements
- Ubuntu 24.04+ or equivalent Linux distribution
- X11 or Wayland display server
- .NET 9.0 runtime (included in self-contained builds)

### Build Requirements
- .NET 9.0 SDK
- Linux development tools (build-essential)
- Flatpak and flatpak-builder (for Flatpak builds)

## Installation

### From Release (Recommended)

Download the latest release from the [Releases](../../releases) page:

**Option 1: Standalone Binary**
```bash
# Download and extract
unzip ProtonDrive-linux-x64.zip -d ~/ProtonDrive
cd ~/ProtonDrive

# Make executable
chmod +x ProtonDrive

# Run
./ProtonDrive
```

**Option 2: Flatpak**
```bash
# Install the Flatpak
flatpak install ProtonDrive.flatpak

# Run
flatpak run ch.proton.drive
```

### From Source

See the main [README.md](../README.md) for build instructions.

## Platform-Specific Features

### File System Integration
- Uses standard .NET file I/O APIs
- inotify-based file watching for real-time sync
- XDG Base Directory specification compliance

### Desktop Integration
- Desktop notifications via D-Bus
- System tray integration (where supported)
- Native file manager integration

### Security
- Integration with GNOME Keyring / KDE Wallet for credential storage
- Support for system certificate stores

## Known Limitations

1. **Proprietary Dependencies**: Requires `Proton.Cryptography` and `Proton.Drive.Sdk` packages from Proton AG
2. **UI Migration**: Full WPF-to-Avalonia UI conversion is ongoing
3. **Cloud Files API**: Windows Cloud Files API features not available on Linux
4. **WebAuthn**: FIDO2/WebAuthn support requires additional implementation

## Development

### Project Structure
```
src/
├── ProtonDrive.App.Linux/        # Main Linux application (Avalonia)
├── ProtonDrive.Native.Linux/     # Linux-specific native code
├── ProtonDrive.Sync.Linux/       # Linux file sync implementation
└── [shared projects]              # Platform-independent code
```

### Building for Development
```bash
# Restore dependencies
dotnet restore

# Build
dotnet build src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj

# Run
dotnet run --project src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj
```

## Contributing

While the original Proton Drive for Windows does not accept contributions, this Linux port may have different guidelines. Please check the repository for the latest contribution information.

## License

This project is licensed under the GNU General Public License v3.0 or later. See [LICENSE](../LICENSE.md) for details.

Some portions of code are licensed under the MIT License. See [LICENSE-MIT](../LICENSE-MIT.md) for details.

## Support

For issues specific to the Linux port, please open an issue on this repository.

For general Proton Drive support, visit [proton.me/support](https://proton.me/support).
