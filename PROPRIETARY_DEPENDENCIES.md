# Handling Proprietary Dependencies

This document explains how to work with the proprietary Proton packages required by this project.

## Required Packages

The Proton Drive application depends on two proprietary NuGet packages:

1. **Proton.Cryptography** (v0.22.0)
   - Provides cryptographic operations for Proton Drive
   - Handles encryption, decryption, and key management
   - Required by: ProtonDrive.Client

2. **Proton.Drive.Sdk** (v0.1.0-alpha.3)
   - Proton Drive API SDK
   - Handles communication with Proton Drive servers
   - Required by: ProtonDrive.Client, ProtonDrive.App

## Why These Packages Are Not Included

These packages are proprietary and owned by Proton AG. They are not available in public NuGet repositories and cannot be redistributed without permission.

## Solutions for Developers

### Option 1: Obtain Official Access (Recommended)

If you are:
- A Proton AG employee or contractor
- Working on an officially sanctioned fork
- Have permission from Proton AG

**Steps:**
1. Contact Proton AG to get access credentials
2. Add the private NuGet feed to `NuGet.config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="proton-private" value="YOUR_PRIVATE_FEED_URL" />
  </packageSources>
  <packageSourceCredentials>
    <proton-private>
      <add key="Username" value="YOUR_USERNAME" />
      <add key="ClearTextPassword" value="YOUR_PASSWORD" />
    </proton-private>
  </packageSourceCredentials>
</configuration>
```

3. Restore packages normally:
```bash
dotnet restore
```

### Option 2: Create Mock Implementations (Development Only)

For development and testing without actual Proton servers, create mock implementations.

**Steps:**

1. Create mock package projects:
```bash
mkdir -p mocks/Proton.Cryptography
mkdir -p mocks/Proton.Drive.Sdk
```

2. Create basic implementations (see Mock Implementation Guide below)

3. Pack and install locally:
```bash
cd mocks/Proton.Cryptography
dotnet pack -o ~/.nuget/local-packages

cd ../Proton.Drive.Sdk
dotnet pack -o ~/.nuget/local-packages
```

4. Configure NuGet to use local packages:
```xml
<!-- Add to NuGet.config -->
<add key="local" value="/home/youruser/.nuget/local-packages" />
```

### Option 3: Conditional Compilation (Advanced)

Modify the project to make these dependencies optional:

1. Edit project files to use conditional package references
2. Add preprocessor directives in code
3. Implement fallback behavior

## Mock Implementation Guide

### Mock: Proton.Cryptography

Create `mocks/Proton.Cryptography/Proton.Cryptography.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Version>0.22.0</Version>
    <PackageId>Proton.Cryptography</PackageId>
    <Authors>Mock Implementation</Authors>
    <Description>Mock implementation for development</Description>
  </PropertyGroup>
</Project>
```

Create stub classes matching the expected API:

```csharp
// Example stub - replace with actual API surface
namespace Proton.Cryptography
{
    public interface ICryptoService
    {
        byte[] Encrypt(byte[] data, byte[] key);
        byte[] Decrypt(byte[] data, byte[] key);
    }

    public class CryptoService : ICryptoService
    {
        public byte[] Encrypt(byte[] data, byte[] key)
        {
            // Mock implementation - DO NOT USE IN PRODUCTION
            return data;
        }

        public byte[] Decrypt(byte[] data, byte[] key)
        {
            // Mock implementation - DO NOT USE IN PRODUCTION
            return data;
        }
    }
}
```

### Mock: Proton.Drive.Sdk

Create `mocks/Proton.Drive.Sdk/Proton.Drive.Sdk.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Version>0.1.0-alpha.3</Version>
    <PackageId>Proton.Drive.Sdk</PackageId>
    <Authors>Mock Implementation</Authors>
    <Description>Mock implementation for development</Description>
  </PropertyGroup>
</Project>
```

Create stub classes:

```csharp
// Example stub - replace with actual API surface
namespace Proton.Drive.Sdk
{
    public interface IDriveClient
    {
        Task<string> GetUserInfoAsync();
        Task UploadFileAsync(string path);
        Task DownloadFileAsync(string fileId, string destination);
    }

    public class DriveClient : IDriveClient
    {
        public async Task<string> GetUserInfoAsync()
        {
            // Mock implementation
            await Task.Delay(100);
            return "Mock User";
        }

        public async Task UploadFileAsync(string path)
        {
            // Mock implementation
            await Task.Delay(100);
        }

        public async Task DownloadFileAsync(string fileId, string destination)
        {
            // Mock implementation
            await Task.Delay(100);
        }
    }
}
```

## Reverse Engineering the API Surface

If you don't have access to the packages but need to create mocks:

1. **Examine the calling code**:
   ```bash
   grep -r "using Proton.Cryptography" src/
   grep -r "using Proton.Drive.Sdk" src/
   ```

2. **Look at usage patterns**:
   ```bash
   # Find how these packages are used
   find src -name "*.cs" -exec grep -l "Proton.Cryptography\|Proton.Drive.Sdk" {} \;
   ```

3. **Extract interfaces from compiler errors**:
   - Try to build without the packages
   - Compiler errors will reveal method signatures
   - Create minimal interfaces to satisfy the compiler

4. **Check for documentation**:
   - Look for XML doc comments in the codebase
   - Check Proton's public repositories for similar APIs

## Building Without Proprietary Packages

### Quick Disable Method

Comment out projects that require proprietary packages:

Edit `ProtonDrive.slnx`:
```xml
<!-- Temporarily disable until packages are available
<Project Path="src/ProtonDrive.Client/ProtonDrive.Client.csproj" />
-->
```

Build only Linux-specific projects:
```bash
dotnet build src/ProtonDrive.Native.Linux/ProtonDrive.Native.Linux.csproj
dotnet build src/ProtonDrive.Sync.Linux/ProtonDrive.Sync.Linux.csproj
```

## Security Considerations

### For Mock Implementations

⚠️ **WARNING**: Mock implementations are for DEVELOPMENT ONLY

- Do NOT use mocks in production
- Do NOT handle real user data with mocks
- Do NOT distribute applications built with mocks
- Mock crypto operations provide NO security

### For Production Builds

- Only use official Proton packages from authorized sources
- Verify package signatures
- Use secure credential storage for NuGet feeds
- Do not commit credentials to source control

## Testing Strategy

### Without Real Packages

1. **Unit Tests**: Test platform-specific code that doesn't depend on Proton packages
2. **Integration Tests**: Use mocks for integration testing
3. **UI Tests**: Test UI layer separately from business logic

### With Real Packages

1. Full integration testing
2. End-to-end testing with Proton servers
3. Production-like scenarios

## Frequently Asked Questions

**Q: Can I distribute a build with mock packages?**
A: No. Mocks are for development only and provide no real functionality or security.

**Q: Will this application work without the real packages?**
A: No. The core functionality requires the real Proton.Cryptography and Proton.Drive.Sdk packages.

**Q: Can I replace these packages with open-source alternatives?**
A: This would require significant refactoring and would not be compatible with Proton Drive servers.

**Q: How do I get access to the real packages?**
A: Contact Proton AG or check if you have access through official channels.

## Resources

- [NuGet Package Configuration](https://docs.microsoft.com/nuget/reference/nuget-config-file)
- [Creating NuGet Packages](https://docs.microsoft.com/nuget/create-packages/creating-a-package)
- [Private NuGet Feeds](https://docs.microsoft.com/nuget/hosting-packages/overview)

## Support

For access to proprietary packages:
- Contact: Proton AG official channels
- Do NOT request proprietary packages in public issues

For development with mocks:
- Open an issue on this repository
- Include details about what API surfaces you need to mock
