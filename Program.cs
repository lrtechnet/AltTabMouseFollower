using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace AltTabMouseFollower
{
    public class AltTabMouseFollower : ApplicationContext
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback,
            IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr hookId = IntPtr.Zero;
        private LowLevelKeyboardProc proc;
        private bool altPressed = false;
        private bool tabPressed = false;
        private NotifyIcon trayIcon;

        public AltTabMouseFollower()
        {
            trayIcon = new NotifyIcon()
            {
                Icon = SystemIcons.Application,
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = true,
                Text = "Alt+Tab Mouse Follower"
            };

            trayIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);

            proc = HookCallback;
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    LoadLibrary("user32"), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (vkCode == 0x12 || vkCode == 164) // Alt key (left or right) 
                {
                    if (wParam == (IntPtr)WM_SYSKEYDOWN || wParam == (IntPtr)WM_KEYDOWN)
                    {
                        altPressed = true;
                    }
                    else if (wParam == (IntPtr)WM_SYSKEYUP || wParam == (IntPtr)WM_KEYUP)
                    {
                        altPressed = false;

                        if (tabPressed)
                        {
                            tabPressed = false;
                            System.Threading.Tasks.Task.Delay(50).ContinueWith(_ =>
                                MoveCursorToActiveWindow());
                        }
                    }
                }

                if (vkCode == 0x09 && altPressed) // Tab key while Alt held
                {
                    if (wParam == (IntPtr)WM_SYSKEYDOWN || wParam == (IntPtr)WM_KEYDOWN)
                    {
                        tabPressed = true;
                    }
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private void MoveCursorToActiveWindow()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd != IntPtr.Zero && GetWindowRect(hwnd, out RECT rect))
            {
                int centerX = (rect.Left + rect.Right) / 2;
                int centerY = (rect.Top + rect.Bottom) / 2;
                SetCursorPos(centerX, centerY);
            }
        }

        void Exit(object? sender, EventArgs e)
        {
            UnhookWindowsHookEx(hookId);
            trayIcon.Visible = false;
            Application.Exit();
        }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AltTabMouseFollower());
        }
    }
}