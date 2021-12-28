using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tutorial.Managers;
using ElectronNET.API;
using Newtonsoft.Json;
using Tutorial.Controls;

namespace Tutorial.Pages.Clicker
{
    public class IndexModel : PageModel
    {
        List<MacroDetails> macros = new List<MacroDetails>();
        public List<MacroDetails> Macros
        {
            get { return macros; }
            set { macros = value;}
        }
        public bool Listening { get => UserInfoManager.Instance.MacrosListening;
            set { 
                UserInfoManager.Instance.MacrosListening = value;
                Task.Run(OnChangedListeningFlag);
            } }

        public void OnGet()
        {
            InitMacros();
            InitEvents();
        }

        void InitMacros()
        {
            macros.Clear();
            macros = Helpers.DefaultMacroDetails();
            macros.AddRange(UserInfoManager.Instance.CustomMacros);
            SaveMacros();
            Task.Run(() => RefreshMacroShortcuts());
        }

        void SaveMacros()
        {
            UserInfoManager.Instance.CustomMacros = Macros;
        }

        void InitEvents()
        {
            Electron.IpcMain.RemoveAllListeners("ClickerToggle");
            Electron.IpcMain.On("ClickerToggle", OnClickerToggle);
        }

        void OnClickerToggle(object message)
        {
            bool toggled = (bool)message;
            Listening = toggled;
        }

        async Task OnChangedListeningFlag()
        {
            if (UserInfoManager.Instance.MacrosListening)
            {
                await HotkeyManager.Instance.Refresh();
            }
            else
            {
                await HotkeyManager.Instance.Disable();
            }
        }

        async void RefreshMacroShortcuts()
        {
            Console.WriteLine($"Running RefreshMacroShortcuts(), macrosCount: {macros.Count}, CustomMacros: {UserInfoManager.Instance.CustomMacros.Count}...");
            foreach (MacroDetails macroDetails in macros)
            {
                macroDetails.macro = new Macro();
                macroDetails.macro.Init(macroDetails.specs);
                await HotkeyManager.Instance.AddNewHotkey(macroDetails.GetElectronShortcutString(), 
                    async () => await macroDetails.macro.ToggleActing(), false);
            }
            await HotkeyManager.Instance.Refresh();
        }
    }
}
