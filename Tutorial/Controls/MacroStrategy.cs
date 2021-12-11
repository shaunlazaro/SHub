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
        public Task StartAction();
        public Task StopAction();
    }

    public class OHNO : IMacroStrategy
    {
        public async Task StartAction()
        {
            Console.WriteLine("This is broken!");
        }

        // No aborting this!
        public async Task StopAction()
        {
            Console.WriteLine("This is broken!");
        }
    }

    public class SingleClick : IMacroStrategy
    {
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

    // We're constrained by ActionSpecs being only basic data types so we can easily serialize them, so we'll use this weird mix of strategy/factory to determine what a macro does.
    // We should only process ActionSpecs here.
    public static class MacroStrategyFactory
    {
        public static IMacroStrategy GetStrategy(ActionSpecs specs)
        {
            Debug.Assert(specs.isClick ^ specs.isKeypress, "Specs must be either a click or a keypress type");

            if (specs.isClick)
            {
                return new SingleClick((ClickType)specs.clickType);
            }
            else if (specs.isKeypress)
            {
                if (string.IsNullOrEmpty(specs.keyCode))
                    return null;
                VirtualKeyCode keyCode;
                // Give a virtual key code of a specific key.
                if (specs.keyCode.Length == 1)
                {
                    bool success = Enum.TryParse($"VK_{char.ToUpper(specs.keyCode.ToCharArray()[0])}", out keyCode);
                    if (success)
                    {
                        return new SingleKeypress(keyCode);
                    }
                }
                // Give a virtual key code of F1-0
                else if (specs.keyCode.ToCharArray()[0] == 'F' && (specs.keyCode.Length == 2 || specs.keyCode.Length == 3))
                {
                    bool success = Enum.TryParse($"VK_{char.ToUpper(specs.keyCode.ToCharArray()[0])}", out keyCode);
                    if (success)
                    {
                        return new SingleKeypress(keyCode);
                    }
                }

                Console.WriteLine("Failed to get keyCode from specs");
            }

            Console.WriteLine("Failed to GetStrategy");
            return null;
        }
    }
}
