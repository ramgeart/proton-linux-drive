# Linux Port Status

This document tracks the status of the Linux port of Proton Drive.

Last Updated: 2025-11-15

## Overall Progress: ~30%

The port is in **Early Development** stage. Core infrastructure is in place, but significant work remains.

---

## Infrastructure ✅ COMPLETE

- [x] Project structure created
- [x] Solution file updated
- [x] Build system configured
- [x] CI/CD pipeline setup
- [x] Flatpak manifest created
- [x] Documentation written
- [x] Developer tooling

**Status:** Infrastructure is complete and functional.

---

## Platform-Specific Implementations

### ProtonDrive.Native.Linux - 20% Complete

#### Authentication ⏳ IN PROGRESS
- [x] Basic structure created
- [x] Contract classes defined
- [ ] WebAuthn/FIDO2 implementation
- [ ] PAM integration
- [ ] Biometric support

**Priority:** Medium (needed for 2FA)

#### System Integration ⏳ PLANNED
- [ ] D-Bus service implementation
- [ ] Desktop notifications (partial - notify-send only)
- [ ] System tray integration
- [ ] Auto-start configuration
- [ ] File manager extensions

**Priority:** High

### ProtonDrive.Sync.Linux - 40% Complete

#### File System ✅ BASIC IMPLEMENTATION
- [x] FileSystemExtendedWatcher (inotify-based)
- [x] FileSystemClient (POSIX operations)
- [x] File attribute handling
- [ ] Extended attributes (xattr) support
- [ ] Sparse file handling
- [ ] Hard link detection
- [ ] Symbolic link handling

**Priority:** High

#### Security 🔧 PARTIAL
- [x] DataProtectionProvider (AES-based)
- [ ] GNOME Keyring integration
- [ ] KDE Wallet integration
- [ ] Secret Service API support
- [ ] Key derivation improvements

**Priority:** Critical (security!)

#### Shell Integration 🔧 MINIMAL
- [x] Basic notifications (notify-send)
- [ ] D-Bus notification server
- [ ] Progress notifications
- [ ] Context menu integration
- [ ] Quick actions

**Priority:** Medium

### ProtonDrive.App.Linux - 10% Complete

#### UI Framework ✅ SETUP COMPLETE
- [x] Avalonia UI integration
- [x] Basic window structure
- [x] Application lifecycle
- [ ] Theme system
- [ ] Resource dictionaries
- [ ] Custom controls

**Priority:** High

#### Views/XAML ❌ NOT STARTED
Progress: 0 of ~70 XAML files converted

**Windows XAML Files:**
- SignIn views (multiple)
- Main window
- Activity view
- MyComputer view
- SharedWithMe view
- Settings views
- Dialog views
- Onboarding views

**Status:** Major undertaking required

**Priority:** Critical

#### View Models 🔧 PARTIAL
- [ ] SignIn ViewModel
- [ ] Main ViewModel
- [ ] Settings ViewModel
- [ ] Activity ViewModel
- [ ] Folder ViewModel
- [ ] File ViewModel

**Priority:** High

---

## Shared/Cross-Platform Code

### ProtonDrive.App - 🔧 PARTIAL

- [x] Made multi-target (Windows + cross-platform)
- [ ] Platform abstraction layer
- [ ] Dependency injection setup
- [ ] Service registration

**Status:** Needs platform-specific service registration

### ProtonDrive.Client - ⚠️ BLOCKED

**Blocker:** Requires proprietary packages
- Proton.Cryptography
- Proton.Drive.Sdk

**Workaround:** Mock implementations for development

### Other Shared Libraries - ✅ SHOULD WORK

These are platform-agnostic and should work on Linux:
- ProtonDrive.Shared
- ProtonDrive.Sync.Shared
- ProtonDrive.Sync.Engine
- ProtonDrive.Sync.Agent
- ProtonDrive.Sync.Adapter
- ProtonDrive.DataAccess
- ProtonDrive.Update

**Status:** Untested but likely compatible

---

## Features by Priority

### Critical (Blocker for MVP)

1. **Proprietary Package Access** ⏳
   - Status: Waiting for access or mock implementation
   - Impact: Cannot build working application without these

2. **UI Conversion** ❌
   - Status: Not started
   - Impact: No user interface
   - Effort: ~2-4 weeks

3. **Authentication Flow** ❌
   - Status: Depends on #1
   - Impact: Cannot sign in
   - Effort: 1 week

4. **File Sync Core** 🔧
   - Status: Basic implementation exists
   - Impact: Core functionality
   - Effort: 1-2 weeks to complete

### High Priority (Important for Release)

1. **Secure Credential Storage** ⏳
   - Status: Basic AES encryption
   - Needs: Keyring integration
   - Impact: Security concern
   - Effort: 1 week

2. **Desktop Integration** ⏳
   - Status: Basic notifications only
   - Needs: Full D-Bus integration
   - Impact: User experience
   - Effort: 1 week

3. **System Tray** ❌
   - Status: Not implemented
   - Impact: Background operation
   - Effort: 3-5 days

### Medium Priority (Nice to Have)

1. **WebAuthn/FIDO2** ❌
   - Status: Stub only
   - Impact: 2FA with hardware keys
   - Effort: 2 weeks

2. **File Manager Extensions** ❌
   - Status: Not implemented
   - Impact: Convenience
   - Effort: 1 week per file manager

3. **Theme Support** ❌
   - Status: Default theme only
   - Impact: Customization
   - Effort: 3-5 days

### Low Priority (Future)

1. **AppImage Support** ❌
2. **Snap Package** ❌
3. **ARM64 Build** ❌
4. **Wayland Native** ❌

---

## Timeline Estimates

### Conservative Estimate (With Full Resources)

**Phase 1: MVP (3-4 months)**
- Complete UI conversion
- Implement authentication
- Complete file sync
- Keyring integration

**Phase 2: Beta (2-3 months)**
- Desktop integration
- System tray
- Polish UI/UX
- Bug fixes

**Phase 3: Release (1-2 months)**
- Performance optimization
- Documentation
- Testing
- Package distribution

**Total: 6-9 months** of full-time development

### Current Reality

With current resources (automated port):
- Infrastructure: ✅ Complete
- Core implementation: 30% complete
- Needs: Significant manual work for UI conversion

---

## Blockers

### Critical Blockers

1. **Proprietary Package Access**
   - Cannot build fully functional app
   - Need: Access to Proton.Cryptography and Proton.Drive.Sdk
   - OR: Detailed API documentation to create compatible implementations

2. **UI Conversion Effort**
   - 70 XAML files to convert
   - Each requires manual review and adaptation
   - Avalonia has differences from WPF

### Minor Blockers

1. **Testing Infrastructure**
   - Need real package access to test end-to-end
   - Mock testing only goes so far

2. **Platform Testing**
   - Need to test on multiple Linux distributions
   - Different desktop environments (GNOME, KDE, etc.)

---

## What Works Now

✅ **Working:**
- Project structure compiles (with mocks)
- Linux-specific implementations build
- CI/CD pipeline runs
- Documentation is comprehensive

❌ **Not Working:**
- Cannot run the application (no UI)
- Cannot authenticate (no real packages)
- Cannot sync files (missing auth)

---

## What's Needed to Reach MVP

### Development Tasks

1. **UI Conversion** (~120-160 hours)
   - Convert 70 XAML files
   - Adapt view models
   - Test each view

2. **Service Integration** (~40 hours)
   - Wire up dependency injection
   - Platform-specific services
   - Configuration

3. **File Sync Completion** (~40 hours)
   - Extended attributes
   - Conflict resolution
   - Progress tracking

4. **Security Hardening** (~20 hours)
   - Keyring integration
   - Key management
   - Credential storage

5. **Testing** (~60 hours)
   - Unit tests
   - Integration tests
   - Manual testing

**Total Estimated Effort:** 280-320 hours (7-8 weeks full-time)

### External Dependencies

1. Access to proprietary Proton packages
2. Proton Drive test account
3. Linux test environments

---

## How to Contribute

### High-Impact Areas

1. **UI Conversion**
   - Pick a view from Windows
   - Convert XAML to Avalonia
   - Test and submit PR

2. **Platform Integration**
   - Implement D-Bus services
   - Add file manager extensions
   - Improve notifications

3. **Security**
   - Implement keyring integration
   - Review crypto implementations
   - Security testing

### Getting Started

See [DEVELOPMENT.md](DEVELOPMENT.md) for setup instructions.

---

## Contact

For questions about the Linux port:
- Open an issue on GitHub
- Tag with `linux-port` label

For proprietary package access:
- Contact Proton AG directly
- This is not available through this repository
