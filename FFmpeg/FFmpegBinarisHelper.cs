using System;
using System.IO;
using System.Runtime.InteropServices;

namespace videoPlayerApp.FFmpeg
{
    class FFmpegBinarisHelper
    {
        private const String LD_LIBRARY_PATH = "LD_LIBRARY_PAYH";

        internal static void RegisterFFmpegBinries()
        {
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    var current = Environment.CurrentDirectory;
                    var probe = Path.Combine("FFmpeg", "Plugins");
                    while(current != null)
                    {
                        var ffmpegDirectory = Path.Combine(current, probe);
                        if ( Directory.Exists(ffmpegDirectory))
                        {
                            RegisterLibrariesSearchPath(ffmpegDirectory);
                            return;                            
                        }
                        current = Directory.GetParent(current)?.FullName;
                    }
                    break;                                    
            }
        }

        private static void RegisterLibrariesSearchPath(String path)
        {
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    SetDllDirectory(path);
                    break;
            }
        }

        [DllImport("kernal32", SetLastError = true)]
        private static extern bool SetDllDirectory(String lpPathName);
    }
}
