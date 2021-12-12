using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tutorial.Pages.Clicker;
using WindowsInput.Native;

// It's easiest to keep the strategies and factories in one file.
namespace Tutorial.Controls
{
    public interface IMacroStrategy
    {
        public bool Running { get; set; }
        public Task StartAction();
        public Task StopAction();
    }

    public class SingleClick : IMacroStrategy
    {
        public bool Running { get; set; } = false;
        public SingleClick(ClickType clickType)
        {
            _clickType = clickType;
        }
        ClickType _clickType;

        public async Task StartAction()
        {
            InputSender.SendClickType(_clickType);
        }

        // No aborting this!
        public async Task StopAction()
        {
            return;
        }
    }

    public class SingleKeypress : IMacroStrategy
    {
        public bool Running { get; set; } = false;
        public SingleKeypress(VirtualKeyCode keyCode)
        {
            _keyCode = keyCode;
        }

        VirtualKeyCode _keyCode;

        public async Task StartAction()
        {
            InputSender.SendKeyPress(_keyCode);
        }

        // No aborting this!
        public async Task StopAction()
        {
            return;
        }
    }

    public class MultiClick : IMacroStrategy
    {
        public bool Running { get; set; } = false;
        public MultiClick(ClickType clickType, int delay, int count = -1)
        {
            _clickType = clickType;
            _delay = delay;
            _count = count;
        }

        int _delay = 0;
        int _count = -1;
        const int MULTI_CLICK_VARIANCE = 5; // randomly up to 5ms delay between clicks.
        Random rng = new Random();
        ClickType _clickType;
        Task runningTask = null;

        public async Task StartAction()
        {
            if (runningTask != null)
                return;

            Running = true;
            runningTask = Task.Run(async () =>
            {
                int runningCount = _count;
                while (runningCount != 0 && Running)
                {
                    InputSender.SendClickTypeInstant(_clickType);
                    await Task.Delay(_delay + rng.Next(MULTI_CLICK_VARIANCE));
                    runningCount--;
                }
            });
        }

        public async Task StopAction()
        {
            Running = false;
            await runningTask;
            runningTask = null;
        }
    }

    // Little bit of repetition between this and multiclick.  But, not worth abstracting yet, and I doubt I'll repeat myself a third time.
    public class MultiKeyPress : IMacroStrategy
    {
        public bool Running { get; set; } = false;
        public MultiKeyPress(VirtualKeyCode keyCode, int delay, int count = -1)
        {
            _keyCode = keyCode;
            _delay = delay;
            _count = count;
        }
        const int MULTI_PRESS_VARIANCE = 5; // randomly up to 5ms delay between clicks.
        int _delay = 0;
        int _count = -1; // -1 = infinite
        Random rng = new Random();
        VirtualKeyCode _keyCode;
        Task runningTask = null;

        public async Task StartAction()
        {
            if (runningTask != null) 
                return;

            Running = true;
            runningTask = Task.Run(async () =>
            {
                int runningCount = _count;
                while (runningCount != 0 && Running)
                {
                    InputSender.SendKeyPressInstant(_keyCode);
                    await Task.Delay(_delay + rng.Next(MULTI_PRESS_VARIANCE));
                    runningCount--;
                }
            });
        }

        public async Task StopAction()
        {
            Running = false;
            await runningTask;
            runningTask = null;
        }
    }

    // We're constrained by ActionSpecs being only basic data types so we can easily serialize them, so we'll use this weird mix of strategy/factory to determine what a macro does.
    // We should only process ActionSpecs here.
    public static class MacroStrategyFactory
    {
        public static IMacroStrategy GetStrategy(ActionSpecs specs)
        {
            Debug.Assert(specs.isClick ^ specs.isKeypress, "Specs must be either a click or a keypress type");

            if (specs.isClick)
            {
                if(!specs.repeated)
                {
                    return new SingleClick((ClickType)specs.clickType);
                }
                else
                {
                    return new MultiClick((ClickType)specs.clickType, specs.repDelay, specs.repCount);
                }
            }
            else if (specs.isKeypress)
            {
                if (string.IsNullOrEmpty(specs.keyCode))
                    return null;

                VirtualKeyCode keyCode = VirtualKeyCode.JUNJA; // Lol
                bool success = false;

                // Parse specs to get a keycode:
                // Give a virtual key code of a specific key.
                if (specs.keyCode.Length == 1)
                {
                    success = Enum.TryParse($"VK_{char.ToUpper(specs.keyCode.ToCharArray()[0])}", out keyCode);

                }
                // Give a virtual key code of F0-9.
                else if (specs.keyCode.ToCharArray()[0] == 'F' && (specs.keyCode.Length == 2 || specs.keyCode.Length == 3))
                {
                    success = Enum.TryParse($"VK_{char.ToUpper(specs.keyCode.ToCharArray()[0])}", out keyCode);                       
                }

                // If we've gotten a keycode, proceed to choose which type of keypress strategy to use.
                if(success)
                {
                    if (!specs.repeated)
                        return new SingleKeypress(keyCode);
                    else
                        return new MultiKeyPress(keyCode, specs.repDelay, specs.repCount);
                }
                Console.WriteLine("Failed to get keyCode from specs");
            }

            Console.WriteLine("Failed to GetStrategy");
            return null;
        }
    }
}
