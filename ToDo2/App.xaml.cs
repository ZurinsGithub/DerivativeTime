using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ToDo2
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        //ToDo2命令
        public static CommandClass commandClass = new CommandClass();

        //禁止重复运行
        const int WM_COPYDATA = 0x004A;
        private static System.Threading.Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new System.Threading.Mutex(true, "ToDo2");
            if (mutex.WaitOne(0, false)|!ToDo2Settings.Default.CheckStartUp)
            {
                base.OnStartup(e);
            }
            else
            {
                this.Shutdown();
                SendMessage("ToDo2", "ToDo2: Open");//window的标题(title)
            }
        }


        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string ipClassName, string ipWindowName);

        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string ipData;
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage
        (
        IntPtr hWnd,
        int Msg,
        int wParam,
        ref CopyDataStruct lParam
        );

        public static void SendMessage(string windowName, string strMsg)
        {
            if (strMsg == null)
                return;
            IntPtr hwnd = FindWindow(null, windowName);
            if (hwnd != IntPtr.Zero)
            {
                CopyDataStruct cds;
                cds.dwData = IntPtr.Zero;
                cds.ipData = strMsg;
                cds.cbData = System.Text.Encoding.Default.GetBytes(strMsg).Length + 1;
                int fromWindowHandler = 0;
                Console.WriteLine(SendMessage(hwnd, WM_COPYDATA, fromWindowHandler, ref cds));
            }
        }
    }
}
