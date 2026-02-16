# Alt+Tab Mouse Follower - Technical Documentation

## User Description

### Problem
When working with multiple monitors, Alt+Tab is an efficient way to switch between applications. However, after switching windows, you often need to manually move your mouse across large amounts of screen space to interact with the newly selected window. This breaks the flow and reduces the efficiency that Alt+Tab provides.

### Solution
This application automatically moves your mouse cursor to the center of whichever window you select with Alt+Tab. When you release the Alt key after tabbing to your desired window, the mouse instantly appears in that window's center - eliminating the need to manually move your cursor across monitors.

---

## Technical Overview

### How It Works

1. **Process Launch**: `AltTabMouseFollower.exe` starts as a Windows process
2. **System Tray Registration**: The application registers itself in the system tray (no visible window)
3. **Windows API Integration**: The app imports functions from Windows DLLs (`user32.dll`, `kernel32.dll`) via P/Invoke to interact with the operating system
4. **Hook Registration**: `SetWindowsHookEx` registers the `HookCallback` method as a global keyboard hook with Windows, passing a function pointer
5. **Message Loop**: `Application.Run(new AltTabMouseFollower())` starts and maintains the Windows message loop, keeping the initial instance of `AltTabMouseFollower` alive in memory
6. **Event Processing**: Windows calls the registered `HookCallback` for every keyboard event system-wide
7. **State Tracking**: The callback uses class member variables (`altPressed`, `tabPressed`) to track the Alt+Tab key combination across multiple separate callback invocations
8. **Mouse Movement**: When Alt is released after Tab was pressed, the app queries the active window's position and moves the cursor to its center

### Key Components

- **Class member variables** (`altPressed`, `tabPressed`): Maintain state between separate Windows callback invocations
- **Message loop**: Pumps Windows messages (including keyboard events) to the application
- **Global keyboard hook**: Intercepts all keyboard events before they reach applications
- **Cleanup**: `Exit()` unhooks the callback, though Windows automatically cleans up hooks if the process terminates unexpectedly

### Technical Notes

- The application detects both left Alt (VK code 0x12/18) and right Alt (VK code 164) to ensure universal keyboard compatibility
- A 50ms delay after Alt release ensures the window is fully activated before cursor movement
- The hook persists until explicit unhook or process termination
- Windows gracefully handles hook cleanup if the application crashes or is force-terminated

## Architecture

### Windows API Functions Used

| Function | DLL | Purpose |
|----------|-----|---------|
| `SetWindowsHookEx` | user32.dll | Register global keyboard hook |
| `UnhookWindowsHookEx` | user32.dll | Unregister keyboard hook |
| `CallNextHookEx` | user32.dll | Pass event to next hook in chain |
| `SetCursorPos` | user32.dll | Move mouse cursor to coordinates |
| `GetForegroundWindow` | user32.dll | Get handle to active window |
| `GetWindowRect` | user32.dll | Get window position and dimensions |
| `LoadLibrary` | kernel32.dll | Load DLL for hook registration |

### Event Flow
```
User presses Alt → Windows calls HookCallback → altPressed = true
User presses Tab (while Alt held) → Windows calls HookCallback → tabPressed = true
User releases Alt → Windows calls HookCallback → altPressed = false
                 → if (tabPressed) → MoveCursorToActiveWindow()
                                  → GetForegroundWindow()
                                  → GetWindowRect()
                                  → Calculate center coordinates
                                  → SetCursorPos()
```

### State Management

The application maintains state across separate callback invocations using instance fields:
```csharp
private bool altPressed = false;  // Tracks if Alt is currently held
private bool tabPressed = false;  // Tracks if Tab was pressed during current Alt hold
```

These persist in memory as part of the `AltTabMouseFollower` instance kept alive by `Application.Run()`.

## Security Considerations

- **Keyboard Hook Permissions**: Global keyboard hooks can see all keyboard input. This application only reads key codes and does not log or transmit any data.
- **No Data Collection**: The application does not store, log, or transmit any information.
- **Local Execution Only**: All processing happens locally on the user's machine.
- **Open Source**: Source code is publicly available for audit.

## Performance

- **Minimal CPU Usage**: Hook callbacks are lightweight and return immediately
- **Memory Footprint**: ~10-15 MB (typical for .NET Windows Forms app)
- **No Network Activity**: Application has no network components

## Compatibility

### Supported
- Windows 10 (expected, not tested)
- Windows 11 (tested)
- .NET 6.0+ (tested with .NET 9.0)
- Multiple monitor configurations
- Both left and right Alt keys

### Not Supported
- macOS, Linux (Windows-specific APIs)
- Windows 7 or earlier (untested, may work with .NET Framework version)

## Known Limitations

- Requires active message loop (application must remain running)
- 50ms delay may be perceptible on very fast hardware
- Does not handle minimized or hidden windows differently

## Future Enhancements

Potential improvements for future versions:
- Configurable delay timing
- Option to move to specific window regions (top-left, center, etc.)
- Support for other window-switching methods (Win+Tab, taskbar clicks)
- Toggle hotkey to enable/disable functionality
- Settings UI for customization