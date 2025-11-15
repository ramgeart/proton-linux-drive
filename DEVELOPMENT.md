# Proton Drive Linux Development Guide

This guide covers development-specific information for the Linux port of Proton Drive.

## Development Environment Setup

### Prerequisites

1. **Operating System**: Ubuntu 24.04+ or equivalent Linux distribution
2. **.NET SDK**: Version 9.0 or later
3. **Development Tools**:
   - Git
   - Your preferred IDE (Rider, VS Code, or Visual Studio)
   - Flatpak and flatpak-builder (for Flatpak builds)

### Installing .NET SDK

```bash
# Ubuntu/Debian
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0
```

### IDE Setup

**JetBrains Rider** (Recommended)
- Download from: https://www.jetbrains.com/rider/
- Open the `ProtonDrive.slnx` solution file
- Rider will automatically restore NuGet packages

**Visual Studio Code**
```bash
# Install VS Code
sudo snap install code --classic

# Install C# extension
code --install-extension ms-dotnettools.csharp
```

## Project Structure

```
src/
├── ProtonDrive.App/                 # Cross-platform core application
├── ProtonDrive.App.Linux/           # Linux-specific UI (Avalonia)
├── ProtonDrive.App.Windows/         # Windows-specific UI (WPF)
├── ProtonDrive.Native.Linux/        # Linux native operations
├── ProtonDrive.Native.Windows/      # Windows native operations
├── ProtonDrive.Sync.Linux/          # Linux file sync
├── ProtonDrive.Sync.Windows/        # Windows file sync
└── [shared libraries]/              # Platform-independent code
```

## Building

### Quick Build

```bash
# Using the build script
./build-linux.sh

# Manual build
dotnet build src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj
```

### Development Build

```bash
# Debug configuration
dotnet build src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj \
    --configuration Debug

# Run directly
dotnet run --project src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj
```

### Release Build

```bash
# Release configuration with single-file publish
dotnet publish src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj \
    --configuration Release \
    --runtime linux-x64 \
    --self-contained true \
    -p:PublishSingleFile=true
```

## Working with Proprietary Dependencies

The project depends on two proprietary Proton packages:
- `Proton.Cryptography` (v0.22.0)
- `Proton.Drive.Sdk` (v0.1.0-alpha.3)

### Options for Development

**Option 1: Mock Implementation**
Create mock implementations of these packages for local development:
```bash
# Create local NuGet packages directory
mkdir -p ~/.nuget/local-packages
```

**Option 2: Private NuGet Feed**
Configure access to Proton's private NuGet feed in `NuGet.config`:
```xml
<add key="proton-private" value="https://your-feed-url/v3/index.json" />
```

## UI Development

### Avalonia UI

The Linux port uses Avalonia UI instead of WPF. Key differences:

1. **XAML Namespace**: `https://github.com/avaloniaui` instead of WPF namespace
2. **Controls**: Most WPF controls have Avalonia equivalents
3. **Styling**: Uses Fluent theme by default

### Converting WPF to Avalonia

Example conversion:
```xml
<!-- WPF -->
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
    <TextBlock Text="Hello" />
</Window>

<!-- Avalonia -->
<Window xmlns="https://github.com/avaloniaui">
    <TextBlock Text="Hello" />
</Window>
```

### Live Preview

Avalonia supports design-time preview:
```bash
dotnet tool install --global Avalonia.Designer.HostApp
```

## Platform-Specific Code

### Conditional Compilation

Use preprocessor directives for platform-specific code:
```csharp
#if LINUX
    // Linux-specific code
#elif WINDOWS
    // Windows-specific code
#endif
```

### Dependency Injection

Platform-specific services are registered at startup:
```csharp
// In Program.cs (Linux)
services.AddSingleton<IDataProtectionProvider, 
    ProtonDrive.Sync.Linux.Security.Cryptography.DataProtectionProvider>();
```

## Testing

### Unit Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test src/ProtonDrive.App.Linux.Tests/
```

### Manual Testing

```bash
# Run with verbose logging
export DOTNET_ENVIRONMENT=Development
dotnet run --project src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj
```

## Debugging

### Visual Studio Code

Create `.vscode/launch.json`:
```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/src/ProtonDrive.App.Linux/bin/Debug/net9.0/ProtonDrive",
            "args": [],
            "cwd": "${workspaceFolder}",
            "stopAtEntry": false,
            "console": "internalConsole"
        }
    ]
}
```

### JetBrains Rider

1. Open solution
2. Set `ProtonDrive.App.Linux` as startup project
3. Press F5 to debug

## Common Issues

### Missing Dependencies

**Issue**: `NU1101: Unable to find package Proton.Cryptography`

**Solution**: These are proprietary packages. Either:
- Obtain access to Proton's private NuGet feed
- Create mock implementations for development

### Build Errors on Linux

**Issue**: Cannot build Windows-specific projects on Linux

**Solution**: This is expected. Build only Linux projects:
```bash
dotnet build src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj
```

### Avalonia Designer Issues

**Issue**: XAML preview not working

**Solution**: Install Avalonia XAML Intelligence:
```bash
# VS Code
code --install-extension AvaloniaTeam.vscode-avalonia

# Or use Rider which has built-in support
```

## Contributing to Linux Port

### Code Style

- Follow existing .editorconfig settings
- Use meaningful variable names
- Add XML documentation comments for public APIs
- Keep platform-specific code in platform-specific projects

### Pull Request Checklist

- [ ] Code builds without errors
- [ ] Code follows project style guidelines
- [ ] Added/updated unit tests
- [ ] Updated documentation
- [ ] Tested on Linux

### Areas Needing Contribution

1. **UI Conversion**: Convert remaining WPF XAML to Avalonia
2. **WebAuthn**: Implement FIDO2 support for Linux
3. **Keyring Integration**: Integrate with GNOME Keyring/KDE Wallet
4. **File Manager Integration**: Add Nautilus/Dolphin extensions
5. **System Tray**: Improve system tray integration

## Resources

- [Avalonia Documentation](https://docs.avaloniaui.net/)
- [.NET on Linux](https://docs.microsoft.com/en-us/dotnet/core/install/linux)
- [Flatpak Documentation](https://docs.flatpak.org/)
- [D-Bus Specification](https://dbus.freedesktop.org/doc/dbus-specification.html)

## Getting Help

- Open an issue on GitHub
- Check existing issues and documentation
- Join the community discussions
