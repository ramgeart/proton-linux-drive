# CI/CD Pipeline Documentation

This document describes the automated build and release pipeline for Proton Drive Linux.

## Overview

The project uses GitHub Actions for continuous integration and deployment. The pipeline builds both standalone binaries and Flatpak packages.

## Workflows

### Main Build Workflow: `build-linux.yml`

Location: `.github/workflows/build-linux.yml`

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` branch
- Manual workflow dispatch
- Git tags starting with `v*` (for releases)

**Jobs:**

#### 1. `build-linux`
Builds the standalone Linux application.

**Steps:**
1. Checkout code
2. Setup .NET 9.0 SDK
3. Install build dependencies
4. Restore NuGet packages (continues on error for missing proprietary packages)
5. Build and publish for linux-x64
6. Create ZIP archive
7. Upload artifact

**Outputs:**
- `ProtonDrive-linux-x64.zip` - Self-contained executable in ZIP format

**Runtime:** Ubuntu 24.04

#### 2. `build-flatpak`
Builds the Flatpak package.

**Steps:**
1. Checkout code
2. Install Flatpak tools and dependencies
3. Add Flathub repository
4. Install required runtimes:
   - org.freedesktop.Platform//24.08
   - org.freedesktop.Sdk//24.08
   - org.freedesktop.Sdk.Extension.dotnet9//24.08
5. Build Flatpak using flatpak-builder
6. Export Flatpak bundle
7. Upload artifact

**Outputs:**
- `ProtonDrive.flatpak` - Flatpak bundle

**Runtime:** Ubuntu 24.04

#### 3. `release`
Creates a GitHub release with build artifacts.

**Triggers:**
- Only runs on git tags (e.g., `v1.12.0`)

**Dependencies:**
- Requires successful `build-linux` job
- Requires successful `build-flatpak` job

**Steps:**
1. Download build artifacts
2. Create GitHub release
3. Attach artifacts to release

**Outputs:**
- GitHub Release with ZIP and Flatpak attached

## Build Configuration

### Environment Variables

```yaml
DOTNET_VERSION: '9.0.x'
```

### Continue-on-Error

The workflow uses `continue-on-error: true` for some steps to handle missing proprietary packages gracefully:

```yaml
- name: Restore dependencies
  run: dotnet restore src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj
  continue-on-error: true
```

This allows the workflow to proceed even if `Proton.Cryptography` or `Proton.Drive.Sdk` packages are not available.

## Artifacts

### Artifact Retention

- **Build artifacts**: Retained for 90 days (GitHub default)
- **Release artifacts**: Retained permanently with the release

### Artifact Structure

**ProtonDrive-linux-x64.zip:**
```
ProtonDrive                 # Main executable
ProtonDrive.pdb             # Debug symbols
*.dll                       # Dependencies
*.json                      # Configuration
Logo.png                    # Icon
```

**ProtonDrive.flatpak:**
- Self-contained Flatpak bundle
- Can be installed with: `flatpak install ProtonDrive.flatpak`

## Release Process

### Automated Releases

1. **Tag the release:**
   ```bash
   git tag v1.12.0
   git push origin v1.12.0
   ```

2. **Wait for CI:**
   - GitHub Actions automatically detects the tag
   - Runs build jobs
   - Creates release if builds succeed

3. **Release created:**
   - Title: Version from tag (e.g., "v1.12.0")
   - Artifacts attached
   - Published automatically (not draft)

### Manual Release Steps

If automatic release fails:

1. **Download artifacts from Actions tab**
2. **Create release manually:**
   ```bash
   gh release create v1.12.0 \
     ProtonDrive-linux-x64.zip \
     ProtonDrive.flatpak \
     --title "Version 1.12.0" \
     --notes "Release notes here"
   ```

## Local Testing

### Test Locally Before Pushing

**Test Linux build:**
```bash
./build-linux.sh Release linux-x64
```

**Test Flatpak build:**
```bash
flatpak-builder --user --install --force-clean \
  build-dir flatpak/ch.proton.drive.yml
```

**Test the built application:**
```bash
# Standalone
cd publish/linux-x64
./ProtonDrive

# Flatpak
flatpak run ch.proton.drive
```

## Troubleshooting

### Common Issues

#### 1. Missing Proprietary Packages

**Symptom:**
```
error NU1101: Unable to find package Proton.Cryptography
```

**Solution:**
- This is expected in public forks
- Workflow uses `continue-on-error: true`
- For working builds, configure private NuGet feed

#### 2. Flatpak Build Fails

**Symptom:**
```
ERROR: Failed to build Flatpak
```

**Solutions:**
- Check Flatpak manifest syntax: `flatpak-builder --help`
- Verify runtime versions are available: `flatpak remote-ls flathub`
- Check build logs in Actions tab

#### 3. .NET SDK Version Mismatch

**Symptom:**
```
The current .NET SDK does not support targeting .NET 9.0
```

**Solution:**
- Update workflow to use correct SDK version
- Verify `DOTNET_VERSION` environment variable
- Check `DefaultTargetFramework` in Directory.Build.props

#### 4. Artifact Upload Fails

**Symptom:**
```
Error: Unable to find any artifacts for upload
```

**Solution:**
- Check if build actually succeeded
- Verify output directory path
- Check for typos in artifact names

### Debug Actions Locally

Use [act](https://github.com/nektos/act) to run GitHub Actions locally:

```bash
# Install act
brew install act  # macOS
# or
sudo snap install act  # Linux

# Run workflow
act push
```

## Security Considerations

### Secrets Management

**Required Secrets:**
- `GITHUB_TOKEN` - Automatically provided by GitHub

**Optional Secrets:**
- NuGet feed credentials (if using private feed)

**Adding Secrets:**
1. Go to repository Settings
2. Navigate to Secrets and variables → Actions
3. Add new repository secret

### Artifact Security

- Artifacts are stored in GitHub's secure storage
- Only repository collaborators can access artifacts
- Public releases are accessible to everyone
- Consider signing releases for verification

## Performance Optimization

### Caching

Currently not implemented but recommended:

```yaml
- name: Cache NuGet packages
  uses: actions/cache@v3
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
    restore-keys: |
      ${{ runner.os }}-nuget-
```

### Build Time

Typical build times:
- Linux build: 2-5 minutes
- Flatpak build: 5-10 minutes
- Total pipeline: 7-15 minutes

## Monitoring

### Build Status

**Badge for README:**
```markdown
![Build Status](https://github.com/ramgeart/proton-linux-drive/workflows/Build%20and%20Release%20Linux/badge.svg)
```

**Check recent builds:**
- Go to Actions tab
- View workflow runs
- Check logs for failures

### Notifications

**Configure notifications:**
1. Go to repository Settings
2. Navigate to Notifications
3. Enable email notifications for workflow failures

## Future Improvements

### Planned Enhancements

1. **Add caching** for faster builds
2. **Matrix builds** for multiple Linux distributions
3. **Code signing** for releases
4. **Automated testing** in CI
5. **Docker builds** for containerized deployment
6. **AppImage generation** as alternative to Flatpak
7. **Snap package** for Ubuntu users

### Code Quality Checks

Consider adding:
```yaml
- name: Run linter
  run: dotnet format --verify-no-changes

- name: Run tests
  run: dotnet test

- name: Security scan
  uses: github/codeql-action/analyze@v2
```

## Resources

- [GitHub Actions Documentation](https://docs.github.com/actions)
- [Flatpak Builder Documentation](https://docs.flatpak.org/en/latest/flatpak-builder.html)
- [.NET GitHub Actions](https://github.com/actions/setup-dotnet)

## Support

For CI/CD issues:
1. Check workflow logs in Actions tab
2. Review this documentation
3. Open an issue with workflow run URL
4. Include relevant log snippets
