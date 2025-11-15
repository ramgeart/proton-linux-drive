#!/bin/bash

# Script to analyze XAML files that need conversion from WPF to Avalonia

echo "========================================="
echo "WPF to Avalonia Conversion Analysis"
echo "========================================="
echo ""

WINDOWS_VIEWS_DIR="src/ProtonDrive.App.Windows/Views"
LINUX_VIEWS_DIR="src/ProtonDrive.App.Linux/Views"

# Count total XAML files
TOTAL_XAML=$(find "$WINDOWS_VIEWS_DIR" -name "*.xaml" 2>/dev/null | wc -l)
echo "Total WPF XAML files: $TOTAL_XAML"

# Count converted files (if any)
CONVERTED=0
if [ -d "$LINUX_VIEWS_DIR" ]; then
    CONVERTED=$(find "$LINUX_VIEWS_DIR" -name "*.axaml" 2>/dev/null | wc -l)
fi
echo "Converted Avalonia files: $CONVERTED"

REMAINING=$((TOTAL_XAML - CONVERTED))
echo "Remaining to convert: $REMAINING"
echo ""

# Calculate percentage
if [ $TOTAL_XAML -gt 0 ]; then
    PERCENTAGE=$((CONVERTED * 100 / TOTAL_XAML))
    echo "Progress: $PERCENTAGE%"
else
    echo "Progress: 0%"
fi

echo ""
echo "========================================="
echo "File List by Category"
echo "========================================="
echo ""

# List files by subdirectory
find "$WINDOWS_VIEWS_DIR" -type d | while read -r dir; do
    relative_dir=${dir#$WINDOWS_VIEWS_DIR/}
    if [ "$relative_dir" != "$WINDOWS_VIEWS_DIR" ]; then
        xaml_count=$(find "$dir" -maxdepth 1 -name "*.xaml" 2>/dev/null | wc -l)
        if [ $xaml_count -gt 0 ]; then
            echo "📁 $relative_dir: $xaml_count files"
        fi
    fi
done

echo ""
echo "========================================="
echo "Detailed File List"
echo "========================================="
echo ""

# List all XAML files
find "$WINDOWS_VIEWS_DIR" -name "*.xaml" | sort | while read -r file; do
    relative_path=${file#$WINDOWS_VIEWS_DIR/}
    
    # Check if converted version exists
    converted_file="$LINUX_VIEWS_DIR/${relative_path%.xaml}.axaml"
    
    if [ -f "$converted_file" ]; then
        echo "✅ $relative_path"
    else
        echo "❌ $relative_path"
    fi
done

echo ""
echo "========================================="
echo "Conversion Priority Suggestions"
echo "========================================="
echo ""

echo "High Priority (Core UI):"
echo "  - SignIn views"
echo "  - Main window"
echo "  - Settings views"
echo ""

echo "Medium Priority (Features):"
echo "  - Activity views"
echo "  - MyComputer views"
echo "  - Onboarding views"
echo ""

echo "Low Priority (Optional):"
echo "  - Help/About views"
echo "  - Dialogs"
echo ""

echo "========================================="
echo "Next Steps"
echo "========================================="
echo ""
echo "1. Create Views directory structure:"
echo "   mkdir -p $LINUX_VIEWS_DIR/{SignIn,Main,Settings,Onboarding}"
echo ""
echo "2. Start with a simple view (e.g., SignIn):"
echo "   See WPF_TO_AVALONIA.md for conversion guide"
echo ""
echo "3. Test each converted view:"
echo "   dotnet run --project src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj"
echo ""
