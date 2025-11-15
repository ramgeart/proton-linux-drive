#!/bin/bash
set -e

# Build script for Proton Drive Linux

echo "========================================="
echo "Proton Drive Linux Build Script"
echo "========================================="

# Configuration
BUILD_CONFIG=${1:-Release}
RUNTIME=${2:-linux-x64}
OUTPUT_DIR="./publish/$RUNTIME"

echo "Build Configuration: $BUILD_CONFIG"
echo "Runtime: $RUNTIME"
echo "Output Directory: $OUTPUT_DIR"
echo ""

# Check for .NET SDK
echo "Checking for .NET SDK..."
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET SDK not found. Please install .NET 9.0 SDK or later."
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo "Found .NET SDK version: $DOTNET_VERSION"

# Extract major version and check minimum required version
MAJOR_VERSION=$(echo "$DOTNET_VERSION" | cut -d'.' -f1)
if [ "$MAJOR_VERSION" -lt 9 ]; then
    echo "ERROR: .NET SDK 9.0 or later is required. Found version: $DOTNET_VERSION"
    exit 1
fi
echo ""

# Clean previous builds
echo "Cleaning previous builds..."
rm -rf "$OUTPUT_DIR"
dotnet clean src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj --configuration "$BUILD_CONFIG" || echo "Warning: Clean failed, continuing anyway..."
echo ""

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj || {
    echo "WARNING: Some dependencies could not be restored."
    echo "This is expected if proprietary Proton packages are not available."
    echo "Continuing anyway..."
}
echo ""

# Build
echo "Building application..."
dotnet build src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj \
    --configuration "$BUILD_CONFIG" \
    --runtime "$RUNTIME" || {
    echo "ERROR: Build failed"
    exit 1
}
echo ""

# Publish
echo "Publishing self-contained application..."
dotnet publish src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj \
    --configuration "$BUILD_CONFIG" \
    --runtime "$RUNTIME" \
    --self-contained true \
    --output "$OUTPUT_DIR" \
    -p:PublishSingleFile=true \
    -p:PublishTrimmed=false \
    -p:PublishReadyToRun=false || {
    echo "ERROR: Publish failed"
    exit 1
}
echo ""

# Create zip archive
echo "Creating release archive..."
cd "$OUTPUT_DIR"
ZIP_NAME="ProtonDrive-$RUNTIME.zip"
zip -r "../$ZIP_NAME" . > /dev/null
cd - > /dev/null
echo "Created: publish/$ZIP_NAME"
echo ""

echo "========================================="
echo "Build completed successfully!"
echo "========================================="
echo "Executable: $OUTPUT_DIR/ProtonDrive"
echo "Archive: publish/$ZIP_NAME"
echo ""
echo "To run the application:"
echo "  cd $OUTPUT_DIR"
echo "  ./ProtonDrive"
