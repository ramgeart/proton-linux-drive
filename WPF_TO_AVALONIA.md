# WPF to Avalonia Migration Guide

This guide helps with converting WPF XAML views to Avalonia for the Linux port.

## Overview

The Proton Drive Windows application has approximately 70 XAML files that need to be converted from WPF to Avalonia. While Avalonia is designed to be similar to WPF, there are important differences to be aware of.

## Key Differences

### 1. XML Namespace

**WPF:**
```xml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
```

**Avalonia:**
```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
```

### 2. Window Properties

**WPF:**
```xml
<Window WindowStartupLocation="CenterScreen"
        WindowStyle="None"
        ResizeMode="NoResize"
        ShowInTaskbar="True">
```

**Avalonia:**
```xml
<Window WindowStartupLocation="CenterScreen"
        SystemDecorations="None"
        CanResize="False"
        ShowInTaskbar="True">
```

### 3. Resource Dictionaries

**WPF:**
```xml
<Window.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="/Resources/Styles/ColorPalette.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Window.Resources>
```

**Avalonia:**
```xml
<Window.Styles>
    <StyleInclude Source="/Resources/Styles/ColorPalette.axaml" />
</Window.Styles>
```

Note: `.xaml` becomes `.axaml` in Avalonia

### 4. Triggers and Behaviors

**WPF Trigger:**
```xml
<Style.Triggers>
    <Trigger Property="IsMouseOver" Value="True">
        <Setter Property="Background" Value="LightBlue" />
    </Trigger>
</Style.Triggers>
```

**Avalonia Pseudo-classes:**
```xml
<Style Selector="Button:pointerover">
    <Setter Property="Background" Value="LightBlue" />
</Style>
```

### 5. Events

Most events work the same, but some have different names:

| WPF | Avalonia |
|-----|----------|
| MouseEnter | PointerEnter |
| MouseLeave | PointerLeave |
| MouseDown | PointerPressed |
| MouseUp | PointerReleased |

## Conversion Process

### Step-by-Step Guide

1. **Copy the XAML file**
   ```bash
   cp src/ProtonDrive.App.Windows/Views/Example.xaml \
      src/ProtonDrive.App.Linux/Views/Example.axaml
   ```

2. **Update namespace**
   - Replace WPF namespace with Avalonia namespace
   - Change file extension references from `.xaml` to `.axaml`

3. **Update control names**
   - Most controls have the same name
   - Check [Avalonia Control Catalog](https://docs.avaloniaui.net/docs/controls) for differences

4. **Convert triggers to styles**
   - Replace WPF triggers with Avalonia selectors
   - Use pseudo-classes for state changes

5. **Update behaviors**
   - Some behaviors need Avalonia.Xaml.Interactions package
   - Convert WPF behaviors to Avalonia equivalents

6. **Copy code-behind**
   ```bash
   cp src/ProtonDrive.App.Windows/Views/Example.xaml.cs \
      src/ProtonDrive.App.Linux/Views/Example.axaml.cs
   ```

7. **Update code-behind**
   - Change namespace
   - Update base class if needed
   - Update any WPF-specific code

8. **Test**
   - Build the project
   - Run and verify the view appears correctly
   - Test all interactions

## Example Conversion

### Before (WPF)

**SignInView.xaml:**
```xml
<UserControl x:Class="ProtonDrive.App.Windows.Views.SignIn.SignInView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d">
    <UserControl.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="/Resources/Styles/TextBoxStyle.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </UserControl.Resources>
    
    <Grid Background="{StaticResource BackgroundBrush}">
        <StackPanel Margin="20" VerticalAlignment="Center">
            <TextBlock Text="Sign In" 
                      FontSize="24" 
                      FontWeight="Bold"
                      Foreground="{StaticResource TextBrush}" />
            
            <TextBox x:Name="EmailTextBox"
                    Margin="0,20,0,0"
                    Style="{StaticResource ProtonTextBox}" />
            
            <PasswordBox x:Name="PasswordBox"
                        Margin="0,10,0,0" />
            
            <Button Content="Sign In"
                   Margin="0,20,0,0"
                   Click="SignInButton_Click">
                <Button.Style>
                    <Style TargetType="Button">
                        <Setter Property="Background" Value="Blue" />
                        <Style.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter Property="Background" Value="LightBlue" />
                            </Trigger>
                        </Style.Triggers>
                    </Style>
                </Button.Style>
            </Button>
        </StackPanel>
    </Grid>
</UserControl>
```

### After (Avalonia)

**SignInView.axaml:**
```xml
<UserControl x:Class="ProtonDrive.App.Linux.Views.SignIn.SignInView"
             xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d">
    <UserControl.Styles>
        <StyleInclude Source="/Resources/Styles/TextBoxStyle.axaml" />
    </UserControl.Styles>
    
    <Grid Background="{StaticResource BackgroundBrush}">
        <StackPanel Margin="20" VerticalAlignment="Center">
            <TextBlock Text="Sign In" 
                      FontSize="24" 
                      FontWeight="Bold"
                      Foreground="{StaticResource TextBrush}" />
            
            <TextBox x:Name="EmailTextBox"
                    Margin="0,20,0,0"
                    Classes="ProtonTextBox"
                    Watermark="Email" />
            
            <TextBox x:Name="PasswordBox"
                    Margin="0,10,0,0"
                    PasswordChar="●"
                    Watermark="Password" />
            
            <Button Content="Sign In"
                   Margin="0,20,0,0"
                   Click="SignInButton_Click"
                   Classes="PrimaryButton" />
        </StackPanel>
    </Grid>
</UserControl>

<!-- Styles moved to separate file or defined here -->
<UserControl.Styles>
    <Style Selector="Button.PrimaryButton">
        <Setter Property="Background" Value="Blue" />
    </Style>
    <Style Selector="Button.PrimaryButton:pointerover">
        <Setter Property="Background" Value="LightBlue" />
    </Style>
</UserControl.Styles>
```

**Changes made:**
1. ✅ Namespace changed to Avalonia
2. ✅ ResourceDictionary → StyleInclude
3. ✅ .xaml → .axaml
4. ✅ PasswordBox → TextBox with PasswordChar
5. ✅ Style attribute → Classes
6. ✅ Triggers → Selectors with pseudo-classes
7. ✅ Added Watermark for placeholders

## Common Control Mappings

| WPF Control | Avalonia Control | Notes |
|-------------|------------------|-------|
| TextBox | TextBox | Same |
| PasswordBox | TextBox | Use `PasswordChar="●"` |
| Button | Button | Same |
| CheckBox | CheckBox | Same |
| RadioButton | RadioButton | Same |
| ComboBox | ComboBox | Same |
| ListBox | ListBox | Same |
| DataGrid | DataGrid | Need Avalonia.Controls.DataGrid package |
| Menu | Menu | Same |
| ContextMenu | ContextMenu | Same |
| ToolTip | ToolTip | Same |
| TreeView | TreeView | Same |
| TabControl | TabControl | Same |
| WebBrowser | N/A | Use CefNet or similar |

## Styles and Resources

### Converting Styles

Create a new file for each style resource:

**TextBoxStyle.axaml:**
```xml
<Styles xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <Style Selector="TextBox.ProtonTextBox">
        <Setter Property="FontSize" Value="14" />
        <Setter Property="Padding" Value="8" />
        <Setter Property="BorderThickness" Value="1" />
        <Setter Property="CornerRadius" Value="4" />
    </Style>
    
    <Style Selector="TextBox.ProtonTextBox:focus">
        <Setter Property="BorderBrush" Value="Blue" />
    </Style>
</Styles>
```

### Converting Resource Dictionaries

**ColorPalette.axaml:**
```xml
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <SolidColorBrush x:Key="BackgroundBrush" Color="#FFFFFF" />
    <SolidColorBrush x:Key="TextBrush" Color="#000000" />
    <SolidColorBrush x:Key="AccentBrush" Color="#6D4AFF" />
    
    <x:Double x:Key="StandardFontSize">14</x:Double>
    <x:Double x:Key="LargeFontSize">24</x:Double>
</ResourceDictionary>
```

## Advanced Features

### Data Binding

Most WPF data binding works the same in Avalonia:

```xml
<TextBlock Text="{Binding UserName}" />
<Button Command="{Binding SignInCommand}" />
```

### Converters

Value converters work similarly:

```csharp
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? true : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

### Animations

Avalonia uses a different animation system:

```xml
<Style Selector="Button:pointerover">
    <Style.Animations>
        <Animation Duration="0:0:0.2">
            <KeyFrame Cue="0%">
                <Setter Property="Opacity" Value="1.0" />
            </KeyFrame>
            <KeyFrame Cue="100%">
                <Setter Property="Opacity" Value="0.8" />
            </KeyFrame>
        </Animation>
    </Style.Animations>
</Style>
```

## Testing Your Conversions

### Using Avalonia Designer

1. Install Avalonia XAML Intelligence:
   ```bash
   # VS Code
   code --install-extension AvaloniaTeam.vscode-avalonia
   ```

2. Open `.axaml` files to see live preview

### Running the Application

```bash
dotnet run --project src/ProtonDrive.App.Linux/ProtonDrive.App.Linux.csproj
```

### Common Issues

**Issue: Control not found**
- Check if you need to add a NuGet package
- Some WPF controls don't exist in Avalonia

**Issue: Binding not working**
- Verify DataContext is set correctly
- Check property names match exactly
- Use Output window for binding errors

**Issue: Style not applying**
- Ensure StyleInclude is in the right place
- Check selector syntax
- Verify resource keys match

## Conversion Checklist

For each view:

- [ ] Copy XAML file and rename to .axaml
- [ ] Update XML namespace
- [ ] Update all resource references
- [ ] Convert triggers to selectors
- [ ] Update control-specific properties
- [ ] Copy and update code-behind
- [ ] Test the view
- [ ] Verify all interactions work
- [ ] Check on multiple Linux distributions
- [ ] Document any issues or workarounds

## Resources

- [Avalonia Documentation](https://docs.avaloniaui.net/)
- [WPF to Avalonia Migration Guide](https://docs.avaloniaui.net/docs/guides/wpf-migration)
- [Avalonia Samples](https://github.com/AvaloniaUI/Avalonia.Samples)
- [Control Catalog](https://github.com/AvaloniaUI/Avalonia.Controls.Catalog)

## Getting Help

- [Avalonia Discord](https://discord.gg/avaloniaui)
- [Avalonia GitHub Discussions](https://github.com/AvaloniaUI/Avalonia/discussions)
- Open an issue in this repository with the `ui-conversion` label
