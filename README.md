# Alt+Tab Mouse Follower

Automatically moves your mouse cursor to the center of windows selected with Alt+Tab on Windows.

## The Problem

When working with multiple monitors, Alt+Tab is an efficient way to switch between applications. However, after switching windows, you often need to manually move your mouse across large amounts of screen space to interact with the newly selected window. This breaks the flow and reduces the efficiency that Alt+Tab provides.

## The Solution

This application automatically moves your mouse cursor to the center of whichever window you select with Alt+Tab. When you release the Alt key after tabbing to your desired window, the mouse instantly appears in that window's center - eliminating the need to manually move your cursor across monitors.

## Installation

### Option 1: Download Pre-built Executable (Recommended)

1. Download `AltTabMouseFollower-v1.0.0.zip` from the [Releases](../../releases) page
2. Extract the zip file to a permanent location (e.g., `C:\Program Files\AltTabMouseFollower\`)
3. Run `AltTabMouseFollower.exe` - an icon will appear in your system tray

### Option 2: Build from Source

**Requirements:**
- Visual Studio 2022 (or later)
- .NET 9.0 SDK (or later)

**Steps:**
1. Clone this repository
2. Open `AltTabMouseFollower.sln` in Visual Studio
3. Build in Release mode (`Ctrl+Shift+B`)
4. Publish as self-contained (right-click project → Publish → win-x64 self-contained)
5. Find the executable in the publish output folder

## Auto-Start on Login (Optional)

To make the application start automatically when you log in to Windows:

### Using PowerShell Script (Recommended)

1. Download `install-startup.ps1` from this repository
2. Open PowerShell
3. Navigate to the folder containing the script
4. Run:
```powershell
   .\install-startup.ps1 "C:\Full\Path\To\AltTabMouseFollower.exe"
```

### Manual Method

1. Press `Win+R`, type `shell:startup`, press Enter
2. Create a shortcut to `AltTabMouseFollower.exe` in that folder

## Usage

1. Launch the application (icon appears in system tray)
2. Use Alt+Tab as normal to switch between windows
3. When you release Alt, your mouse cursor automatically moves to the selected window's center
4. To exit: Right-click the system tray icon → Exit

## Tested Environment

- **OS:** Windows 11
- **IDE:** Visual Studio 2022 Community
- **Framework:** .NET 9.0
- **Language:** C#

Other versions of Windows (10+), Visual Studio, and .NET (6.0+) are expected to work but have not been explicitly tested.

## How It Works

1. The application registers a global keyboard hook with Windows
2. It monitors for Alt+Tab key combinations
3. When Alt is released after pressing Tab, it queries the active window's position
4. The mouse cursor is programmatically moved to the window's center using Windows API calls

For detailed technical documentation, see [TECHNICAL.md](TECHNICAL.md).

## License

MIT License - see [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## Author

Created by Leonardo Rodrigues ([@lrtechnet](https://github.com/lrtechnet))

---

**Note:** This application uses low-level keyboard hooks and requires appropriate permissions to function. It does not collect, store, or transmit any user data.