using System.Runtime.InteropServices;

namespace Tutorial
{
    // using static is the future
    public static class Helpers
    {
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
