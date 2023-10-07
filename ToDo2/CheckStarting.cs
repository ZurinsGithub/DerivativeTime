using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ToDo2
{
    delegate void ProcessOpen(bool v);
    class ToDo2WindowHasStarted
    {
        ProcessOpen po;

        public ToDo2WindowHasStarted(Window window,ProcessOpen po)
        {
            this.po = po;
            (PresentationSource.FromVisual(window) as HwndSource).AddHook(new HwndSourceHook(this.WndProc));
        }

        public static void ShowWindow(Window window)
        {
            window.WindowState = WindowState.Normal;
            window.Show();
            if (!window.Topmost)          //若本来就置顶，则可以不管这个
            {
                //将窗口排到最前
                window.Topmost = true;
                window.Topmost = false;
            }
            window.Activate();
        }

        const int WM_COPYDATA = 0x004A;
        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string ipData;
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_COPYDATA)
            {
                CopyDataStruct cds = (CopyDataStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));
                if (cds.ipData == "ToDo2: Open")
                {
                    try
                    {
                        po(true);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("错误：ToDo2已经在运行。");
                    }
                }
                else
                {
                    Console.WriteLine("进程通信的传入命令错误");
                }
            }
            return hwnd;
        }
    }
}
