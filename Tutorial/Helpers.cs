using Tutorial.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Tutorial
{
    // using static is the future
    public static class Helpers
    {
        public static List<MacroDetails> DefaultMacroDetails()
        {
            List<MacroDetails> defaults = new List<MacroDetails>();
            defaults.Add(new MacroDetails
            {
                name = "SingleClickDefaultMacro",
                index = 0,
                modifiers = new List<string> { "CommandOrControl" },
                shortcutKey = "F5",
                specs = new ActionSpecs
                {
                    isClick = true,
                    clickType = 0
                }
            });
            defaults.Add(new MacroDetails
            {
                name = "SingleKeyDefaultMacro",
                index = 1,
                modifiers = new List<string> { "CommandOrControl" },
                shortcutKey = "F6",
                specs = new ActionSpecs
                {
                    isKeypress = true,
                    keyCode = "F"
                }
            });

            return defaults;
        }

        public static void AssertOnWindows()
        {
            if(!OnWindows())
                throw new System.NotImplementedException();
        }
        public static bool OnWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }
}
