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

        public void OnGet()
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

        async void RefreshMacroShortcuts()
        {
            Console.WriteLine($"Running RefreshMacroShortcuts(), macrosCount: {macros.Count}, CustomMacros: {UserInfoManager.Instance.CustomMacros.Count}...");
            foreach (MacroDetails macro in macros)
            {
                macro.macro = new Macro();
                macro.macro.Init(macro.specs);
                await HotkeyManager.Instance.AddNewHotkey(macro.GetElectronShortcutString(), async () => await macro.macro.ToggleActing(), false);
            }
            await HotkeyManager.Instance.Refresh();
        }
    }
}
