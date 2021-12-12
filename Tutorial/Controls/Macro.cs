using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorial.Controls
{
    #region Macro Classes

    // The rest of the codebase should deal with Macro objects to start/stop.
    // We'll set an ActionSpecs externally, and Macro will use that to determine how it should behave.
    public class Macro
    {
        public async void Init(ActionSpecs specs)
        {
            if (Ready && strategy.Running)
                await StopActing();
            strategy = MacroStrategyFactory.GetStrategy(specs);
        }
        public bool Ready => strategy != null;

        IMacroStrategy strategy = null;

        public async Task BeginActing()
        {
            if (!Ready || strategy.Running)
                return;
            await strategy.StartAction();
            return;
        }

        public async Task StopActing()
        {
            if (!Ready || !strategy.Running)
                return;
            await strategy.StopAction();
            return;
        }

        public async Task ToggleActing()
        {
            if (!Ready) return;
            if (strategy.Running)
                await StopActing();
            else
                await BeginActing();
        }
    }

    // This is the way we store macros as json in UserData.  The view will take this class, but the rest of the code should only see the Macro after it's been Init.
    [Serializable]
    public class MacroDetails
    {
        public string name;
        public int index;
        // hotkey settings
        public List<string> modifiers = new List<string>();
        public string shortcutKey = "owo";
        [NonSerialized]
        public Macro macro = null;
        public ActionSpecs specs;

        public string GetElectronShortcutString()
        {
            string electronString = "";
            for(int i = 0; i < modifiers.Count; i++)
            {
                electronString = $"{electronString}{modifiers[i]}+";
            }
            electronString += shortcutKey;
            return electronString;
        }
    }

    public class MacroDetailsComparer : IEqualityComparer<MacroDetails>
    {
        public bool Equals(MacroDetails a, MacroDetails b) => a.name == b.name;
        public int GetHashCode(MacroDetails a) => a.GetHashCode();
    }

    // Settings format.  Default is a click.
    [Serializable]
    public class ActionSpecs
    {
        public bool isClick = false;
        public bool isKeypress = false;

        // Repeated settings.  Disregard if not repeated.
        public bool repeated = false;
        public int repCount = -1; // val < 0 means infinite
        public int repDelay = 1000; // 1 second delay default

        // keypress settings, ok to be null if isKeypress false.
        public List<string> keyPressModifiers;
        public string keyCode; // We'll convert this to a VirtualKeyCode later.

        // mouse settings
        public int clickType; // 0 left, 1 right, 2 middle

        // let's keep this false for now.
        public bool isTargetedClickSequence;
    }
    #endregion
}
