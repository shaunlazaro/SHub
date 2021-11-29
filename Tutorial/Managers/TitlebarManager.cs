using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;

namespace Tutorial.Managers
{
    public static class TitlebarManager
    {
        public static BrowserWindow window {
            get
            {
                if (Electron.WindowManager.BrowserWindows.Count != 0)
                    return Electron.WindowManager.BrowserWindows.First();
                Console.WriteLine("Failed to get Electron.WindowManager.BrowserWindows...");
                return null;
            }
        }
        
        private static bool initialized = false;

        // We set this in Startup because getting the reference from BrowserManager wasn't working (it was null?)
        public static Action Minimize;
        public static Action Maximize;
        public static Action Restore;
        public static Action Close;

        public async static void Init()
        {
            if (initialized) return;
            Electron.IpcMain.On("MinimizeWindow", OnMinimize);
            Electron.IpcMain.On("MaximizeWindow", OnMaximize);
            Electron.IpcMain.On("RestoreWindow", OnRestore);
            Electron.IpcMain.On("CloseWindow", OnClose);
            initialized = true;
        }
        static void OnMinimize(object data)
        {
            Minimize.Invoke();
        }
        static void OnMaximize(object data)
        {
            Maximize.Invoke();
        }
        static void OnRestore(object data)
        {
            Restore.Invoke();
        }
        static void OnClose(object data)
        {
            Close.Invoke();
        }
        public static async void ToggleMaxRestoreButtons()
        {
            if(await window.IsMaximizedAsync())
            {
                Electron.IpcMain.Send(window, "maximized", "uwu");
            }
            else
            {
                Electron.IpcMain.Send(window, "minimized", "uwu");
            }
        }
    }
}
