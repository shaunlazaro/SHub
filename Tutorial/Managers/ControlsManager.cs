using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static Tutorial.Helpers;
using WindowsInput;
using WindowsInput.Native;
using ElectronNET.API;

namespace Tutorial.Managers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MousePoint
    {
        public int X;
        public int Y;

        public MousePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // Public class for sending controls and binding hotkeys.
    public static class ControlsManager
    {
        static Dictionary<string, Action> hotkeyCallbacks = new Dictionary<string, Action>(); // Follow accelerator syntax from Electron
        static bool initialized = false;
        static ControlSender sender = new ControlSender();
        public static void Init()
        {
            if (initialized) return;
            hotkeyCallbacks.Add("CommandOrControl+Shift+F5", () => { Console.WriteLine("Clicky"); ControlSender.SendClick(); });
            Task.Run(HandleHotkeyCallbacks);
            initialized = true;
        }

        #region Hotkeys
        // Check if hotkeys are registered as anything.  To change the action itself is hard.
        static async Task HandleHotkeyCallbacks()
        {
            foreach (KeyValuePair<string, Action> pair in hotkeyCallbacks)
            {
                if (!await Electron.GlobalShortcut.IsRegisteredAsync(pair.Key))
                    Electron.GlobalShortcut.Register(pair.Key, pair.Value);
            }
        }

        static async Task StopAllHotkeys()
        {
            await Task.Run(() => Electron.GlobalShortcut.UnregisterAll());
        }
        #endregion

        #region Public Functions


        #endregion
    }

    // Functions for actually sending controls.
    public class ControlSender
    {
        static Random random = new Random(); // Let's avoid suspicious looking appearance on logs by using rng.
        static InputSimulator inputSim = new InputSimulator(); // https://github.com/michaelnoonan/inputsimulator pinvoke for SendEvent giga sucks.

        const int MIN_RANDOM_PRESS_TIME = 30;
        const int MAX_RANDOM_PRESS_TIME = 60;

        // We make our two actions less obviously botted on logs by adding random delay.
        // Can use for both 
        static void WindowsDelayedUpDown(Action down, Action up)
        {
            AssertOnWindows();
            Task.Run(async () =>
            {
                down?.Invoke();
                await Task.Delay(random.Next(MIN_RANDOM_PRESS_TIME, MAX_RANDOM_PRESS_TIME)); // Sneaky random, I personally click at 40-120ms delay on average, but we should keep this low to feel good.
                up?.Invoke();
            });
        }

        public static void SendClick()
        {
            WindowsDelayedUpDown(() => WindowsMouseControls.MouseEvent(WindowsMouseControls.MouseEventFlags.LeftDown), 
                    () => WindowsMouseControls.MouseEvent(WindowsMouseControls.MouseEventFlags.LeftUp));
        }
        public static void SendClickInstant()
        {
            WindowsMouseControls.MouseEvent(WindowsMouseControls.MouseEventFlags.LeftDown);
            WindowsMouseControls.MouseEvent(WindowsMouseControls.MouseEventFlags.LeftUp);
        }

        public static void SendKeyPressInstant(VirtualKeyCode key)
        {
            SendKeyPressInstant(new VirtualKeyCode[] { key });
        }
        public static void SendKeyPressInstant(VirtualKeyCode[] keys)
        {
            inputSim.Keyboard.KeyPress(keys);
        }

        // We don't have a delayed keyupdown for multi keys, since we're not sending them instantly in sequence...
        public static void SendKeyPress(VirtualKeyCode key)
        {
            WindowsDelayedUpDown(() => inputSim.Keyboard.KeyDown(key),
                () => inputSim.Keyboard.KeyUp(key));
        }

        // https://stackoverflow.com/questions/2416748/how-do-you-simulate-mouse-click-in-c
        private class WindowsMouseControls
        {
            [Flags]
            public enum MouseEventFlags
            {
                LeftDown = 0x00000002,
                LeftUp = 0x00000004,
                MiddleDown = 0x00000020,
                MiddleUp = 0x00000040,
                Move = 0x00000001,
                Absolute = 0x00008000,
                RightDown = 0x00000008,
                RightUp = 0x00000010
            }

            [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool SetCursorPos(int x, int y);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetCursorPos(out MousePoint lpMousePoint);

            [DllImport("user32.dll")]
            private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

            public static void SetCursorPosition(int x, int y)
            {
                AssertOnWindows();
                SetCursorPos(x, y);
            }

            public static void SetCursorPosition(MousePoint point)
            {
                AssertOnWindows();
                SetCursorPos(point.X, point.Y);
            }

            public static MousePoint GetCursorPosition()
            {
                AssertOnWindows();
                MousePoint currentMousePoint;
                var gotPoint = GetCursorPos(out currentMousePoint);
                if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
                return currentMousePoint;
            }

            public static void MouseEvent(MouseEventFlags value, MousePoint position)
            {
                AssertOnWindows();
                mouse_event
                    ((int)value,
                     position.X,
                     position.Y,
                     0,
                     0)
                    ;
            }
            public static void MouseEvent(MouseEventFlags value)
            {
                MousePoint position = GetCursorPosition();
                MouseEvent(value, position);
            }
		}
	}
}
