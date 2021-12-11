using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static Tutorial.Helpers;
using WindowsInput;
using WindowsInput.Native;
using ElectronNET.API;

namespace Tutorial.Controls
{
    // Public class for sending controls and binding hotkeys.
    public static class HotkeyManager
    {
        static Dictionary<string, Action> hotkeyCallbacks = new Dictionary<string, Action>(); // Follow accelerator syntax from Electron
        static bool initialized = false;
        public static void Init()
        {
            if (initialized) return;
            /*
            hotkeyCallbacks.Add("CommandOrControl+Shift+F5", () => { Console.WriteLine("Clicky"); InputSender.SendClickType(ClickType.Left); });
            hotkeyCallbacks.Add("CommandOrControl+Shift+F6", () => { Console.WriteLine("KeyPress"); InputSender.SendKeyPress(VirtualKeyCode.VK_P); });
            Task.Run(HandleHotkeyCallbacks);
            */
            initialized = true;
        }
        public static async Task AddNewHotkey(string key, Action action, bool refresh=true)
        {
            if (!initialized) return;



            if (!hotkeyCallbacks.ContainsKey(key))
            {
                Console.WriteLine($"Adding new hotkey: {key}");
                hotkeyCallbacks.Add(key, action);
            }
            else
            {
                Console.WriteLine($"Overwriting old hotkey: {key}");
                hotkeyCallbacks[key] = action;
            }
            if (refresh)
                await RefreshHotkeys();
        }

        #region Internal Functions
        static async Task RefreshHotkeys()
        {
            await StopAllHotkeys();
            await HandleHotkeyCallbacks();
        }

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

    }
}
