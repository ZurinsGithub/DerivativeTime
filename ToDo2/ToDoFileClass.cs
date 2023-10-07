using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToDo2
{
    public class ToDoFileClass
    {
        public struct Line
        {
            public bool isCommand;
            public int length;
            public byte[] LineBuffer;
        };
        public ArrayList linesArr = new ArrayList();

        public static ToDoFileClass ReadFile(string path)
        {
            ToDoFileClass td = new ToDoFileClass();
            if (File.Exists(path))
            {
                byte[] bf = File.ReadAllBytes(path);
                int hasRead = 0;
                while (hasRead < bf.Length)
                {
                    Line L;
                    L.isCommand = BitConverter.ToBoolean(bf, hasRead);
                    L.length = BitConverter.ToInt32(bf, hasRead + 1);
                    string str = Encoding.Default.GetString(bf, hasRead + 5, L.length);
                    L.LineBuffer = Encoding.Default.GetBytes(str);
                    td.linesArr.Add(L);
                    hasRead += (5 + L.length);
                }
                return td;
            }
            else
                return null;
        }

        public void WriteFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                for (short i = 0; i < linesArr.Count; i++)
                {
                    Line l = (Line)linesArr[i];
                    Console.WriteLine(l.length);
                    byte[] bf = BitConverter.GetBytes(l.isCommand);
                    fs.Write(bf, 0, bf.Length);
                    fs.Flush();
                    Console.WriteLine("position " + fs.Position);
                    byte[] bf1 = BitConverter.GetBytes(l.length);
                    fs.Write(bf1, 0, bf1.Length);
                    fs.Flush();
                    fs.Write(l.LineBuffer, 0, l.LineBuffer.Length);
                    fs.Flush();
                }
            }
        }

        public void AddLine(bool isCommand, string str)
        {
            Line L;
            L.isCommand = isCommand;
            L.LineBuffer = Encoding.Default.GetBytes(str);
            L.length = L.LineBuffer.Length;
            linesArr.Add(L);
        }

        /// <summary>
        /// 写入一个新的ToDo流文件（在文件中）
        /// </summary>
        /// <param name="root">根目录，例如：“./user/local/”</param>
        /// <param name="structPath">结构目录，分隔符为‘/’，例如：“1/2/1”</param>
        /// <param name="name">流的名字</param>
        /// <param name="time">流的时间（分钟）</param>
        public static void AddFile(string root,string structPath,string name,int time)
        {
            string fullPath = System.IO.Path.Combine(root, structPath);
            Console.WriteLine("create: " + fullPath);
            Directory.CreateDirectory(fullPath);
            File.WriteAllText(System.IO.Path.Combine(fullPath, "ProcessFile.wzh"), "");
            File.WriteAllText(System.IO.Path.Combine(fullPath, "information.txt"), name + ";" + time*60);
            File.AppendAllText(System.IO.Path.Combine(root, "ProcessList.txt"), structPath.Replace('/', 'v') + ";");
        }
    }

    public class MainClass
    {
        public string path;
        public string Name;
        public int LastTime;        //剩余的秒数
        public ArrayList LineArr;
        public struct Line
        {
            public int index;
            public bool isCommand;
            public string Content;
        };

        public MainClass()
        {
            LineArr = new ArrayList();
        }
        public MainClass(ToDoFileClass td, int LastTime, string Name,string path)
        {
            LineArr = new ArrayList();
            this.path = path;
            if (Name.Length < 40)
            {
                this.Name = Name;
            }
            if (LastTime < 7200)
            {
                this.LastTime = LastTime;
            }
            for (short i = 0; i < td.linesArr.Count; i++)
            {
                Line line;
                line.Content = Encoding.Default.GetString(((ToDoFileClass.Line)td.linesArr[i]).LineBuffer);
                line.index = i;
                line.isCommand = ((ToDoFileClass.Line)td.linesArr[i]).isCommand;
                LineArr.Add(line);
            }
        }

        public void WriteInformation(string path)
        {
            string s = Name + ";" + LastTime + ";";
            File.WriteAllText(path, s);
        }
    }
}
