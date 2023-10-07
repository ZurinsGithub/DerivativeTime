using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ToDo2
{
    /// <summary>
    /// FlowStruct.xaml 的交互逻辑
    /// </summary>
    public partial class FlowStruct : Window
    {
        public FlowStruct(string path)
        {
            InitializeComponent();
            FlowStructEvent.fEvent += FlowStructEvent_fEvent;
            Sort(path);
        }

        private void FlowStructEvent_fEvent(object sender, FlowStructEventArgs e)
        {
            VCreate(e.a, e.b, e.c);
        }

        #region 排序
        static ArrayList arrayList;
        /// <summary>
        /// 对 path 文件中的字符串进行排序，并返回arrayList数组中；其中FlowStructEvent.Exc()用于初始化控件，若在其他文件中移植代码可删除。
        /// </summary>
        /// <param name="path">文件路径，文件为ProcessList.txt</param>
        public static void Sort(string path)
        {
            arrayList = new ArrayList();
            string[] str = System.IO.File.ReadAllText(path).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (str.Length != 0)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (arrayList.Count == 0)
                    {
                        arrayList.Add(str[i]);
                    }
                    else
                    {
                        //start from here. 
                        int n1654 = arrayList.Count;
                        string[] AddStr = str[i].Split(new char[] { 'v' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int b = 0; b < n1654; b++)     //个
                        {
                            bool CanBreak = false;
                            string[] TargetStr = (arrayList[b] as string).Split(new char[] { 'v' }, StringSplitOptions.RemoveEmptyEntries);
                            if (TargetStr.Length >= AddStr.Length)
                            {
                                for (int o = 0; o < AddStr.Length; o++)      //位
                                {
                                    if (Int32.Parse(AddStr[o]) < Int32.Parse(TargetStr[o]))
                                    {
                                        arrayList.Insert(b, str[i]);//!!
                                        CanBreak = true;
                                        break;
                                    }
                                    else if (Int32.Parse(AddStr[o]) > Int32.Parse(TargetStr[o]))
                                    {
                                        if (b == n1654 - 1)
                                        {
                                            CanBreak = true;
                                            arrayList.Add(str[i]);
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        if (o == AddStr.Length - 1)
                                        {
                                            arrayList.Insert(b, str[i]);
                                            CanBreak = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int o = 0; o < TargetStr.Length; o++)
                                {
                                    if (Int32.Parse(AddStr[o]) < Int32.Parse(TargetStr[o]))
                                    {
                                        arrayList.Insert(b, str[i]);
                                        CanBreak = true;
                                        break;
                                    }
                                    else if (Int32.Parse(AddStr[o]) == Int32.Parse(TargetStr[o]))
                                    {
                                        if (o == TargetStr.Length - 1)
                                        {
                                            if (b == n1654 - 1)
                                            {
                                                arrayList.Add(str[i]);
                                                CanBreak = true;
                                            }
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (b == n1654 - 1)
                                        {
                                            arrayList.Add(str[i]);
                                            CanBreak = true;
                                        }
                                        break;

                                    }
                                }
                            }
                            if (CanBreak)
                            {
                                CanBreak = false;
                                break;
                            }
                        }
                    }
                }


                string lastStr, nextStr;
                int c = 0;

                FlowStructEvent.Exc(arrayList[0] as string, arrayList[0] as string, "A1SP");

                for (int i = 0; i < arrayList.Count - 1; i++)
                {
                    lastStr = arrayList[i] as string;
                    nextStr = arrayList[i + 1] as string;
                    if (lastStr.Length < nextStr.Length)
                    {
                        FlowStructEvent.Exc(nextStr, nextStr, "ChildStackPanel_" + lastStr);
                        c++;
                    }
                    else if (lastStr.Length == nextStr.Length)
                    {

                        if (nextStr.Length - 2 > 1)
                            FlowStructEvent.Exc(nextStr, nextStr, "ChildStackPanel_" + nextStr.Substring(0, nextStr.Length - 2));
                        else
                            FlowStructEvent.Exc(nextStr, nextStr, "A1SP");
                    }
                    else
                    {
                        c = nextStr.Length - 1;

                        if (nextStr.Length - 2 > 1)
                            FlowStructEvent.Exc(nextStr, nextStr, "ChildStackPanel_" + nextStr.Substring(0, nextStr.Length - 2));
                        else
                            FlowStructEvent.Exc(nextStr, nextStr, "A1SP");
                    }
                }
            }
        }

        #endregion
        #region 控件

        private SolidColorBrush SelectBrush(int s)
        {
            s = s % 10;
            if (s < 1)
                return new SolidColorBrush(Color.FromRgb(218, 182, 255));
            else if (s < 3)
                return new SolidColorBrush(Color.FromRgb(182, 255, 235));
            else if (s < 5)
                return new SolidColorBrush(Color.FromRgb(192, 255, 182));
            else if (s < 7)
                return new SolidColorBrush(Color.FromRgb(254, 255, 182));
            else if (s < 9)
                return new SolidColorBrush(Color.FromRgb(255, 199, 182));
            else
                return new SolidColorBrush(Color.FromRgb(218, 182, 255));
        }

        private void VCreate(string content, string name, string LastEle)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Background = SelectBrush(name.Length);
            stackPanel.Margin = new Thickness(0.3, 0.3, 0.3, 0.3); //这里用来设置边框
            Label label = new Label();
            label.Height = 30;
            label.Content = content;
            label.Name = "Lable_" + name;
            StackPanel childStackPanel = new StackPanel();
            stackPanel.Cursor = Cursors.Hand;       //设置光标
            stackPanel.Name = "v" + name;
            stackPanel.MouseDown += StackPanel_MouseDown;
            stackPanel.MouseEnter += StackPanel_MouseEnter;
            stackPanel.MouseLeave += StackPanel_MouseLeave;
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(childStackPanel);
            this.RegisterName("StackPanel_" + name, stackPanel);
            this.RegisterName("ChildStackPanel_" + name, childStackPanel);
            StackPanel stackPanel1 = this.FindName(LastEle) as StackPanel;
            StackPanel stackPanel2 = new StackPanel();
            stackPanel2.Children.Add(stackPanel);
            stackPanel2.Background = Brushes.Black;
            stackPanel2.Margin = new Thickness(21, 9, 3, 3);
            stackPanel2.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel1.Children.Add(stackPanel2);
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as StackPanel).Background = SelectBrush((sender as StackPanel).Name.Length - 1);
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as StackPanel).Background = new SolidColorBrush(Color.FromRgb(59, 235, 226));
            //鼠标进出的冒泡无法阻止！！！
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel stackPanel = sender as StackPanel;
            e.Handled = true;           //阻止冒泡！
            //Label label = sender as Label;
        }
#endregion
        public static ArrayList getArr(string path)
        {
            Sort(path);
            return arrayList;
        }

        public static void UpdateProcessList(string path)
        {
            Sort(path);
            string s = "";
            for(int i = 0; i < arrayList.Count; i++)
            {
                s += arrayList[i] as string + ";"; 
            }
            Console.WriteLine(s + "  " + path);
            System.IO.File.WriteAllText(path, s);
        }

        #region int[]与排序
        /// <summary>
        /// 取得最后一个比int[]（参数）小的int[]（在源数组中）的位置
        /// </summary>
        /// <param name="source">源数组，由int[]组成</param>
        /// <param name="insert">int[]参数</param>
        /// <returns></returns>
        public static int GetMax(ArrayList source, int[] insert)
        {
            if (source.Count == 0)
                return 0;

            int index = source.Count - 1;
            for (int i = 0; i < insert.Length; i++)
            {
                for (int j = index; j >= 0; j--)
                {
                    if ((source[j] as int[]).Length < i + 1)     //i+1?
                    {
                        index = j;
                        break;
                    }
                    else
                    {
                        if ((source[j] as int[])[i] <= insert[i])
                        {
                            index = j;
                            break;
                        }
                    }
                }
            }
            return index;
        }
         
        /// <summary>
        /// 将指定字符分割的字符串转化为int数组并返回，例如：“120.120.1.1”转换为new int[]{120,120,1,1}
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="c">分割字符</param>
        /// <returns></returns>
        public static int[] GetIntArray(string s, char c)
        {
            string[] str = s.Split(new char[] { c }, StringSplitOptions.RemoveEmptyEntries);
            int[] a = new int[str.Length];
            for (int j = 0; j < str.Length; j++)
            {
                a[j] = int.Parse(str[j]);
            }
            return a;
        }


        /// <summary>
        /// 取得 int[] 数组某一位置后可加入的 int[] 的最小 int[]
        /// </summary>
        /// <param name="source">包含若干个int[]的动态数组，若该值为负，则返回仅比较第一位的可插入位置</param>
        /// <param name="position">动态数组中的位置</param>
        /// <param name="outPosition">输出：可插入最小int[]的位置</param>
        /// <returns>int[]</returns>
        public static int[] GetLarge(ArrayList source, int position, out int outPosition)
        {
            if (position == source.Count - 1)
            {
                int[] b = source[position] as int[];
                int[] a = new int[b.Length + 1];
                for (int i = 0; i < b.Length; i++)
                {
                    a[i] = b[i];
                }
                a[b.Length] = 1;
                outPosition = source.Count;
                return a;
            }
            int[] t = source[position] as int[];
            int[] t1 = source[position + 1] as int[];
            if (t1.Length <= t.Length)
            {
                int[] a = new int[t.Length + 1];
                for (int i = 0; i < t.Length; i++)
                {
                    a[i] = t[i];
                }
                a[t.Length] = 1;
                outPosition = position + 1;
                return a;
            }
            else
            {
                int b = 1;
                int n = position + 1;
                bool canBreak = false;
                while (b == t1[t.Length])
                {
                    while (b == t1[t.Length])
                    {
                        n++;
                        t1 = source[n] as int[];
                        if (t.Length >= t1.Length)
                        {
                            canBreak = true;
                            break;
                        }
                    }
                    b++;
                    if (canBreak)
                        break;
                }

                int[] a = new int[t.Length + 1];
                for (int i = 0; i < t.Length; i++)
                {
                    a[i] = t[i];
                }
                a[t.Length] = b;
                outPosition = n;
                return a;
            }
        }

        /// <summary>
        /// 将int[]转化为string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="str">分隔符</param>
        /// <returns></returns>
        public static string GetString(int[] source, string str)
        {
            string s = "";
            for (int i = 0; i < source.Length; i++)
            {
                s += source[i] + str;
            }
            return s;
        }

        #endregion
    }

    public class FlowStructEventArgs : EventArgs
    {
        public string a, b, c;

        public FlowStructEventArgs(string a, string b, string c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }

    public static class FlowStructEvent
    {
        public static event EventHandler<FlowStructEventArgs> fEvent;
        public static void Exc(string a,string b,string c)
        {
            var arg = new FlowStructEventArgs(a, b, c);
            fEvent?.Invoke(null, arg);
        }
    }
}
