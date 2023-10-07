using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToDo2
{
    public delegate string CommandMethod(string parameter);
    public class CommandClass
    {
        public ArrayList CommandArr = new ArrayList();
        public struct CommandLine
        {
            public bool ExcutingTimesAble;      //支持批处理
            public string key;                  //关键字
            public string tips;                 //提示
            public string parameter;            //默认参数！！！若命令行未指定参数，则传入此参数
            public string result;               //执行结果
            public int index;                   //命令数组中序号
            public CommandMethod method;        //命令执行的方法
        }

        public CommandClass()
        {
            //内置命令
            //AddLine(true, "New", "在当前流的目录下创建新的流（参数：名称）（测试）", "", new CommandMethod(Test));
            //AddLine(false, "To", "切换到一个流（参数：目录，例如：“1.1”）（测试）", "", new CommandMethod(Test));
            AddLine(false, "Exit", "保存并退出", "Close", new CommandMethod(MW2Event.Excute));      //触发事件中一系列的方法，若参数正确，则调用
            AddLine(false, "Rename", "重命名当前流（参数：名称）", "Rename", new CommandMethod(CommandMethods.Rename_Method));
            //Run命令的参数包含":"，所以需要把参数包装在<>括号内进行调用，且只能包含一个参数
            AddLine(true, "Run", "运行文件（参数：“<目录 | 目录|...>”）", "", new CommandMethod(CommandMethods.Run_Method));
        }

        public void AddLine(bool ExcutingTimesAble, string key, string tips , string parameter, CommandMethod method)
        {
            CommandLine line = new CommandLine
            {
                ExcutingTimesAble = ExcutingTimesAble,
                key = key,
                method = method,
                tips = tips,
                parameter = parameter,
                index = CommandArr.Count - 1
            };
            CommandArr.Add(line);
        }

        public string Excute(string command, bool ExcuteMode)       //ExcuteMode: 是否是成批处理中的命令
        {
            if (command == "#")
            {
                string s = "\n命令列表：\n";
                for (short i = 0; i < CommandArr.Count; i++)
                    s += ((CommandLine)CommandArr[i]).key + " : " + ((CommandLine)CommandArr[i]).tips + "\n";
                return s;
            }
            //命令格式：key <空格>参数1,<英文逗号>参数2...<以此类推> :<冒号> 结果
            if (!command.Contains(":"))             //不包含结果：未执行
            {
                string[] s = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (short i = 0; i < CommandArr.Count; i++)
                {
                    if ("#" + ((CommandLine)CommandArr[i]).key == s[0])
                    {
                        if (!ExcuteMode | ((CommandLine)CommandArr[i]).ExcutingTimesAble)
                        {
                            if (s.Length > 1)
                            {
                                //连接字符串组成参数
                                for (short j = 2; j < s.Length; j++)
                                    s[1] += s[j] + " ";
                                return ((CommandLine)CommandArr[i]).method(s[1]);
                            }
                            else
                                return ((CommandLine)CommandArr[i]).method(((CommandLine)CommandArr[i]).parameter);
                        }
                        else
                        {
                            return "该命令不支持批处理。";
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("!!!++" + command);
                string[] s = command.Split(new char[] { '<', '>' });
                if (s.Length > 1)
                {
                    for (short i = 0; i < CommandArr.Count; i++)
                    {
                        if (command.Remove(command.IndexOf(" ")) == ("#" + ((CommandLine)CommandArr[i]).key))
                        {
                            return ((CommandLine)CommandArr[i]).method(s[1]);
                        }
                    }
                }
                else
                    return "参数出现错误";
            }
            return "未找到该命令，请检查语句。";
        }
    
        public string BatchExcute(MainClass mainClass)
        {
            string result = "";
            short n = 1;
            for(short i = 0; i < mainClass.LineArr.Count; i++)
            {
                if (((MainClass.Line)(mainClass.LineArr[i])).isCommand)
                {
                    string str = "";
                    string[] s = ((MainClass.Line)(mainClass.LineArr[i])).Content.Split(new char[] { ':' });
                    for(short j = 0; j < s.Length - 1; j++)
                    {
                        str += s[j] + ":";
                    }
                    str = str.Remove(str.Length - 1);
                    result += "批处理命令[" + n++ + "]: " + Excute(str, true) + "\n";
                }
            }
            return result;
        }
    }

    //用于MW2的命令的方法集合
    public static class CommandMethods
    {
        public static string Rename_Method(string p)
        {
            MW2Event.Excute("Rename " + p);
            return "命令已执行";
        }

        public static string Run_Method(string p)
        {
            string[] s = p.Split(new char[] { '|' },StringSplitOptions.RemoveEmptyEntries);
            string returnStr = "";
            if (s.Length==0 & !File.Exists(p))
            {
                if (ToDo2Settings.Default.SelectFileDefaultPath == "")
                {
                    ToDo2Settings.Default.SelectFileDefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    ToDo2Settings.Default.Save();
                }
                var OpenfileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = "文件|*.*",
                    InitialDirectory = ToDo2Settings.Default.SelectFileDefaultPath,
                    Multiselect = true
                };
                var result = OpenfileDialog.ShowDialog();
                if (result == false)
                    returnStr = "取消";
                else
                {
                    returnStr = "<";
                    short n7448 = 0;
                    for(short i = 0; i < OpenfileDialog.FileNames.Length; i++)
                    {
                        if (File.Exists(OpenfileDialog.FileNames[i]))
                        {
                            System.Diagnostics.Process.Start(OpenfileDialog.FileNames[i]);
                            returnStr += OpenfileDialog.FileNames[i] + "|";
                        }
                        else
                        {
                            n7448++;
                        }
                    }
                    returnStr = returnStr.Remove(returnStr.Length - 1);
                    returnStr += ">: " + n7448 + "个失败。";
                }
            }
            else
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (File.Exists(s[i]))
                    {
                        System.Diagnostics.Process.Start(s[i]);
                        returnStr += "打开 " + Path.GetFileName(s[i]) + "\n";
                    }
                    else
                    {
                        returnStr += "未能找到路径为 " + s[i] + " 的文件。\n";
                    } 
                }
            }
            return returnStr;
        }
    }
}
