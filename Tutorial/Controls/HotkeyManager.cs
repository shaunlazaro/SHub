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
    public class HotkeyManager
    {
        private HotkeyManager() { }
        static Lazy<HotkeyManager> instance = new Lazy<HotkeyManager>(() => new HotkeyManager());
        public static HotkeyManager Instance { get => instance.Value; }

        Dictionary<string, Action> hotkeyCallbacks = new Dictionary<string, Action>(); // Follow accelerator syntax from Electron
        public async Task AddNewHotkey(string key, Action action, bool refresh=true)
        {
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
        public async Task Refresh()
        {
            await RefreshHotkeys();
        }

        #region Internal Functions

        async Task RefreshHotkeys()
        {
            await StopAllHotkeys();
            await HandleHotkeyCallbacks();
        }

        // Check if hotkeys are registered as anything.  To change the action itself is hard.
        async Task HandleHotkeyCallbacks()
        {
            foreach (KeyValuePair<string, Action> pair in hotkeyCallbacks)
            {
                if (!await Electron.GlobalShortcut.IsRegisteredAsync(pair.Key))
                    Electron.GlobalShortcut.Register(pair.Key, pair.Value);
            }
        }

        async Task StopAllHotkeys()
        {
            await Task.Run(() => Electron.GlobalShortcut.UnregisterAll());
        }
        #endregion
    }
}
