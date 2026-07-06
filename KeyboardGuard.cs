using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ToddlerCoder;

internal sealed class KeyboardGuard : IDisposable
{
    private const int WhKeyboardLl = 13;
    private const int WmKeyDown = 0x0100;
    private const int WmKeyUp = 0x0101;
    private const int WmSysKeyDown = 0x0104;
    private const int WmSysKeyUp = 0x0105;

    private readonly LowLevelKeyboardProc _hookProc;
    private IntPtr _hookId;
    private bool _disposed;
    private bool _leftWinHeld;
    private bool _rightWinHeld;

    public KeyboardGuard()
    {
        _hookProc = HookCallback;
        _hookId = SetHook(_hookProc);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_hookId != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        _disposed = true;
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using Process currentProcess = Process.GetCurrentProcess();
        using ProcessModule currentModule = currentProcess.MainModule
            ?? throw new InvalidOperationException("Could not find the current process module.");

        return SetWindowsHookEx(WhKeyboardLl, proc, GetModuleHandle(currentModule.ModuleName), 0);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && IsKeyboardEvent(wParam))
        {
            KeyboardEvent keyboardEvent = Marshal.PtrToStructure<KeyboardEvent>(lParam);
            Keys key = (Keys)keyboardEvent.VirtualKeyCode;
            bool keyDown = wParam == WmKeyDown || wParam == WmSysKeyDown;

            if (ShouldSuppress(key, keyDown))
            {
                return 1;
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private static bool IsKeyboardEvent(IntPtr wParam)
    {
        return wParam == WmKeyDown
            || wParam == WmKeyUp
            || wParam == WmSysKeyDown
            || wParam == WmSysKeyUp;
    }

    private bool ShouldSuppress(Keys key, bool keyDown)
    {
        bool altDown = IsKeyDown(Keys.Menu) || IsKeyDown(Keys.LMenu) || IsKeyDown(Keys.RMenu);
        bool ctrlDown = IsKeyDown(Keys.ControlKey) || IsKeyDown(Keys.LControlKey) || IsKeyDown(Keys.RControlKey);
        bool shiftDown = IsKeyDown(Keys.ShiftKey) || IsKeyDown(Keys.LShiftKey) || IsKeyDown(Keys.RShiftKey);

        if (key == Keys.LWin)
        {
            _leftWinHeld = keyDown;
            return true;
        }

        if (key == Keys.RWin)
        {
            _rightWinHeld = keyDown;
            return true;
        }

        bool winHeld = _leftWinHeld || _rightWinHeld || IsKeyDown(Keys.LWin) || IsKeyDown(Keys.RWin);
        if (winHeld)
        {
            return true;
        }

        if (!keyDown)
        {
            return false;
        }

        if (altDown && key is Keys.Tab or Keys.F4 or Keys.Escape or Keys.Space)
        {
            return true;
        }

        if (ctrlDown && !shiftDown && key == Keys.Escape)
        {
            return true;
        }

        return false;
    }

    private static bool IsKeyDown(Keys key)
    {
        return (GetAsyncKeyState((int)key) & 0x8000) != 0;
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct KeyboardEvent
    {
        public uint VirtualKeyCode;
        public uint ScanCode;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
}
