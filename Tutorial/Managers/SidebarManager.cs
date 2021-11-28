using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;

namespace Tutorial.Managers
{
    // Sidebar is controlled by _LayoutBase
    public static class SidebarManager
    {
        const bool SIDEBAR_EXPANDED_BY_DEFAULT = true;
        public static bool Expanded { get; private set; } = SIDEBAR_EXPANDED_BY_DEFAULT;
        private static bool initialized = false;
        public static void Init()
        {
            if (initialized) return;
            Electron.IpcMain.On("SidebarToggle", OnSidebarToggle);
            initialized = true;
        }
        static void OnSidebarToggle(object data)
        {
            Expanded = !Expanded;
        }
    }
}
