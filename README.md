# Proton Drive app for Windows and Linux

Copyright (c) 2025 Proton AG

## License

The code and data files in this repository are licensed under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. See [LICENSE](LICENSE.md) or https://www.gnu.org/licenses/ for a copy of the license.

However, some portion of code is licensed under the MIT License. See [LICENSE-MIT](LICENSE-MIT.md) for a copy of the license.

## Distribution

You can download the latest stable release from the [Proton Drive official website](https://proton.me/drive/download).

### Linux Version

The Linux version is available as:
- **Standalone binary** (.zip archive with self-contained executable)
- **Flatpak** package for easy installation across Linux distributions

Download the latest Linux builds from the [Releases](../../releases) page.

#### Building from Source (Linux)

Prerequisites:
- .NET 9.0 SDK or later
- Ubuntu 24.04+ or equivalent Linux distribution

```bash
# Clone the repository
git clone https://github.com/ramgeart/proton-linux-drive.git
cd proton-linux-drive

# Build the Linux application
dotnet publish src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true

# The built application will be in:
# src/ProtonDrive.App.Linux/bin/Release/net9.0/linux-x64/publish/
```

#### Building Flatpak

```bash
# Install flatpak-builder
sudo apt-get install flatpak flatpak-builder

# Add Flathub repository
flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo

# Install required runtimes
flatpak install flathub org.freedesktop.Platform//24.08
flatpak install flathub org.freedesktop.Sdk//24.08
flatpak install flathub org.freedesktop.Sdk.Extension.dotnet9//24.08

# Build the Flatpak
flatpak-builder --user --install --force-clean build-dir flatpak/ch.proton.drive.yml

# Run the application
flatpak run ch.proton.drive
```

**Note:** The build requires access to proprietary Proton packages (`Proton.Cryptography` and `Proton.Drive.Sdk`). 
These packages are not publicly available and must be obtained from Proton AG or configured in a private NuGet feed.

## Contributions

Contributions are not accepted at the moment.