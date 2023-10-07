using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ToDo2
{
    class ToDoTimer
    {
        int staticTime, processTime, sTs, pTs, i=60;
        public Timer timer = new Timer(1000);

        public int StaticTime { get => staticTime; set => staticTime = value; }
        public int ProcessTime { get => processTime; set => processTime = value; }
        public int STs { get => sTs; set => sTs = value; }
        public int PTs { get => pTs; set => pTs = value; }
        public int I { get => i; set => i = value; }

        public void SetProcessSeconds(int seconds)
        {
            I = 0;
            processTime = seconds / 60;
            pTs = seconds;
        }

        public void SetProcessTime(int min)
        {
            I = 0;
            processTime = min;
            pTs = min * 60;
        }

        public void SetStaticTime(int min)
        {
            I = 0;
            staticTime = min;
            sTs = min * 60;
        }

        public ToDoTimer(int staticTime, int processTime)
        {
            this.StaticTime = staticTime;
            this.ProcessTime = processTime;
            STs = staticTime * 60;
            PTs = processTime * 60;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            STs--;
            PTs--;
            I--;
            if(I < 1)
            {
                staticTime = sTs / 60;
                processTime = pTs / 60;
                I = 60;
            }
        }
    }
}
