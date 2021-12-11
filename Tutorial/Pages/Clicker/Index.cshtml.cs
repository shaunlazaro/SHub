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
            set { macros = value; Task.Run(RefreshMacroShortcuts); }
        }
        public List<Macro> activeMacros = new List<Macro>();


        public void OnGet()
        {
            Macros = Helpers.DefaultMacroDetails();
        }

        async void RefreshMacroShortcuts()
        {
            activeMacros.Clear();
            foreach (MacroDetails macro in macros)
            {
                Macro macroAction = new Macro();
                macroAction.Init(macro.specs);
                macro.macro = macroAction;
                activeMacros.Add(macroAction);
                await HotkeyManager.AddNewHotkey(macro.GetElectronShortcutString(), async () => await macroAction.ToggleActing());
            }
        }
    }
}
