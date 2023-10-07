using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections;
using System.Threading;
using System.IO;

namespace ToDo2
{
    /// <summary>
    /// MW2.xaml 的交互逻辑
    /// </summary>
    public partial class MW2 : Window
    {
        string[] runnings;
        int staticTime;
        public MW2(string[] runnings,int staticTime)
        {
            this.runnings = runnings;
            this.staticTime = staticTime;
            this.Loaded += MW2_Loaded;
            this.Closing += MW2_Closing1;
            InitializeComponent();
            InitIcon();
        }

        private void MW2_Closing1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //释放资源
            try
            {
                timer.Stop();
                timer.Dispose();
                ToDo2Settings.Default.WindowHeigh = this.ActualHeight;
                ToDo2Settings.Default.WindowWidth = this.ActualWidth;
                ToDo2Settings.Default.Save();

                SaveToClass(ArrIndex);
                SaveFile();
            }
            catch (Exception)
            {
            }
        }

        bool HasUsedTMode = false;      //已经用过可编辑模式
        private void SaveToClass(int index)
        {
            //保存A1TB的string到MainArr数组，可能会打乱MainArr数组
            if (HasUsedTMode)
            {
                string[] str = A1TB.Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                MainClass mainClass = new MainClass();
                for (short i = 0; i < str.Length; i++)
                {
                    MainClass.Line line=new MainClass.Line();
                    line.index = i;
                    line.Content = str[i];
                    if (str[i].StartsWith("#"))
                    {
                        line.isCommand = true;
                    }
                    mainClass.LineArr.Add(line);
                }
                (MainArr[index] as MainClass).LineArr = mainClass.LineArr;
            }
            HasUsedTMode = false;
        }

        private void SaveFile()
        {
            //保存文件
            try
            {
                for (short i = 0; i < MainArr.Count; i++)
                {
                    MainClass mainClass = MainArr[i] as MainClass;
                    ToDoFileClass td = new ToDoFileClass();
                    for (short j = 0; j < mainClass.LineArr.Count; j++)
                    {
                        td.AddLine(((MainClass.Line)(mainClass.LineArr[j])).isCommand, ((MainClass.Line)(mainClass.LineArr[j])).Content);
                    }
                    td.WriteFile(mainClass.path + "ProcessFile.wzh");

                    mainClass.WriteInformation(mainClass.path + "information.txt");
                }
            }
            catch (Exception)
            {
                new Window4("文件保存失败，请关闭所有打开的文件后重试。", 0).ShowDialog();
                SaveFile();
            }
        }

        ArrayList MainArr;          //流 数组 控件中
        int ArrIndex;              //正在显示流的控件内序号
        public void InitMainClass(string[] path)      //提供给外部调用，path格式："./users/local/1/"
        {
            MainArr = new ArrayList();
            for(short i = 0; i < path.Length; i++)
            {
                ToDoFileClass td = ToDoFileClass.ReadFile(path[i] + "ProcessFile.wzh");
                string[] s = File.ReadAllText(path[i] + "information.txt").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                MainClass mainClass = new MainClass(td, Int32.Parse(s[1]), s[0], path[i]);
                MainArr.Add(mainClass);
            }
            FreshTextBox(true, 0);
        }

        //刷新A1TB的内容
        private void FreshTextBox(bool CommandShowAble,int index) //是否显示命令
        {
            if (CommandShowAble)
                A5L.Content = "隐藏命令";
            else
                A5L.Content = "显示命令";
            ArrIndex = index;
            A1TB.Text = "";
            MainClass mainClass = MainArr[index] as MainClass;
            for(short i = 0; i < mainClass.LineArr.Count; i++)
            {
                if(CommandShowAble | ((MainClass.Line)mainClass.LineArr[i]).isCommand == false)
                    A1TB.Text += ((MainClass.Line)(mainClass.LineArr[i])).Content + "\n";
            }
            if (A6CStringsArr.Count<2)
            {
                A6CStringsArr.Add(mainClass.Name);
                A6CStringsArr.Add((mainClass.LastTime / 60).ToString() + "分钟");
            }
            else
            {
                A6CStringsArr.Insert(0, mainClass.Name);
                A6CStringsArr.RemoveAt(1);
                A6CStringsArr.Insert(1, (mainClass.LastTime / 60).ToString() + "分钟");
                A6CStringsArr.RemoveAt(2);
            }
            A1TB.ScrollToEnd();

            if (tdTimer != null)
            {
                tdTimer.SetProcessSeconds((MainArr[ArrIndex] as MainClass).LastTime);
                Console.WriteLine("---" + tdTimer.StaticTime);
            }
        }

        System.Windows.Forms.NotifyIcon notifyIcon;
        private void InitIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = "ToDo2";
            notifyIcon.Icon = new System.Drawing.Icon("./IMG/Icon.ico");
            notifyIcon.ContextMenu = GetNotifyContextMenu();    //当此项被设置时，右键将显示菜单，并且不调用click方法
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            notifyIcon.Visible = true;
            this.Closing += MW2_Closing;
        }

        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //if(e.Button==System.Windows.Forms.MouseButtons.Left)
                ShowWindow(true);
        }

        private System.Windows.Forms.ContextMenu GetNotifyContextMenu()
        {
            //在此处定义通知栏图标的右键菜单

            System.Windows.Forms.MenuItem menuItem = new System.Windows.Forms.MenuItem("注销");
            menuItem.Click += MenuItem1_Click;

            System.Windows.Forms.MenuItem menuItem1 = new System.Windows.Forms.MenuItem("退出");
            menuItem1.Click += MenuItem_Click;

            System.Windows.Forms.MenuItem[] menuItems = new System.Windows.Forms.MenuItem[] {menuItem,menuItem1};
            return new System.Windows.Forms.ContextMenu(menuItems);
        }

        private void MenuItem1_Click(object sender, EventArgs e)
        {
            LogOut();
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LogOut()
        {
            //补充：登出
        }

        //生命周期事件方法！
        private void MW2_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //删除通知栏图标
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }

        ToDo2WindowHasStarted tdWHS;
        ToDoTimer tdTimer = null;
        //生命周期事件方法！
        private void MW2_Loaded(object sender, RoutedEventArgs e)
        {
            //注册快捷键
            HotKey.Regist(this, HotkeyModifiers.MOD_ALT, Key.Z, () =>
            {
                //快捷键执行方法: 
                Console.WriteLine(this.WindowState.ToString());
                if (this.WindowState==WindowState.Minimized|!IsActive)
                {
                        ShowWindow(true); 
                }
                else
                {
                    Hide();
                    WindowState = WindowState.Minimized;
                }
            });

            //防止多开
            if(tdWHS == null)
                tdWHS = new ToDo2WindowHasStarted(this, new ProcessOpen(ShowWindow));

            //初始化样式
            InitStyle();

            //底部按钮动画
            InitA6CAnimation();

            //注册外部调用的方法
            MW2Event.mw2event += MW2Event_mw2event;

            //注册通知消息显示
            MW2noticeEvent.mw2noticeEvent += ShowNotice;

            //初始化图片轮换
            InitBackgroundImages();

            //初始化流类
            InitMainClass(runnings);

            //初始化计时器
            InitToDoTimer();
        }

        private void InitStyle()
        {
            //初始化自定义样式
            A1SC.Opacity = ToDo2Settings.Default.Opacity;
            Height = ToDo2Settings.Default.WindowHeigh;
            Width = ToDo2Settings.Default.WindowWidth;
            A1TB.FontSize = ToDo2Settings.Default.FontSize;
            if(File.Exists(ToDo2Settings.Default.bg1))
                B1IB.ImageSource = new BitmapImage(new Uri(ToDo2Settings.Default.bg1));
            if (File.Exists(ToDo2Settings.Default.noticeMusic))
                MW2noticeEvent.playerUri = new Uri(ToDo2Settings.Default.noticeMusic);
        }

        string[] BackGroundUris;
        private void InitBackgroundImages()
        {
            string s = "";
            DirectoryInfo directoryInfo = new DirectoryInfo("./mw2/background");
            foreach (FileInfo file in directoryInfo.GetFiles("*.jpg"))
                s += file.FullName + "\n";
            foreach (FileInfo file in directoryInfo.GetFiles("*.png"))
                s += file.FullName + "\n";
            BackGroundUris = s.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        //显示消息
        ArrayList noticeList = new ArrayList();
        bool noticeShowUsable = true;
        bool noticeHasHandle = true;
        private void ShowNotice(object sender, MW2noticeArgs e)
        {
            noticeList.Add(e);
            ExcuteShowNotice();
        }
        private void ExcuteShowNotice()
        {
            if (noticeShowUsable)
            {
                noticeShowUsable = false;
                noticeHasHandle = false;
                MW2noticeArgs e = noticeList[0] as MW2noticeArgs;
                if (e.Image != null)
                    C1IMG.Source = e.Image.Source;
                C1TB.Text = e.Text;
                noticeHasHandle = false;
                C1C.BeginAnimation(Canvas.TopProperty, new DoubleAnimation
                {
                    From = -100,
                    To = 15,
                    Duration = TimeSpan.FromSeconds(0.7),
                    EasingFunction = new PowerEase()
                });
                if (t5877 != null)
                    t5877.Close();
                if (!e.IsImportant)
                {
                    t5877 = new System.Timers.Timer(5000);
                    t5877.AutoReset = false;
                    t5877.Elapsed += Ti_Elapsed;
                    t5877.Start();
                } 
            }
        }

        System.Timers.Timer t5877;
        private void C2R_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!noticeHasHandle)
            {
                (noticeList[0] as MW2noticeArgs).Excute?.Invoke((noticeList[0] as MW2noticeArgs).Parameter);
                Ti_Elapsed(null, null);
            }
        }

        private void Ti_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!noticeHasHandle)
            {
                noticeHasHandle = true;
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    DoubleAnimation doubleAnimation = new DoubleAnimation
                    {
                        From = Canvas.GetTop(C1C),
                        To = -100,
                        Duration = TimeSpan.FromSeconds(0.7),
                        EasingFunction = new PowerEase()
                    };
                    doubleAnimation.Completed += DoubleAnimation_Completed2;
                    C1C.BeginAnimation(Canvas.TopProperty, doubleAnimation);
                });
            }
        }

        private void DoubleAnimation_Completed2(object sender, EventArgs e)
        {
            noticeList.RemoveAt(0);
            noticeShowUsable = true;
            if (noticeList.Count > 0)
                ExcuteShowNotice();
        }

        private void InitToDoTimer()
        {
            int t = 40;
            if (staticTime > 0)
                t = staticTime;
            tdTimer = new ToDoTimer(t, (MainArr[ArrIndex] as MainClass).LastTime / 60);
            tdTimer.timer.Elapsed += Timer_Elapsed2;
        }

        MediaElement media;
        private void Timer_Elapsed2(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (tdTimer.PTs == 0)
            {
                A6CStringsArr.Insert(1, "流 超时");
                A6CStringsArr.RemoveAt(2);
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    MW2noticeEvent.ShowNotice(true, new Image
                    {
                        Source = new BitmapImage(new Uri(new DirectoryInfo("./IMG/").FullName + "clock1.png"))
                    }, "流 时间到了。", null, "", null);
                });
            }
            if (tdTimer.STs == 0)
            {
                A6CStringsArr.Insert(1, "全局超时 注意休息");
                A6CStringsArr.RemoveAt(2);
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    //显示通知
                    MW2noticeEvent.ShowNotice(true, new Image
                    {
                        Source = new BitmapImage(new Uri(new DirectoryInfo("./IMG/").FullName + "clock1.png"))
                    }, "全局 时间到了，\n即将关闭ToDo。\n您也可以在关闭前修改全局时间", f6685, "", null);
                    //弹窗提醒？
                    if (ToDo2Settings.Default.StaticTimeDialog)
                        new Window4("全局时间到了。", 0).ShowDialog();
                    //播放音乐
                    media = new MediaElement();
                    media.Visibility = Visibility.Collapsed;
                    A1G.Children.Add(media);
                    media.LoadedBehavior = MediaState.Manual;
                    if (File.Exists(ToDo2Settings.Default.EndMusic))
                        media.Source = new Uri(ToDo2Settings.Default.EndMusic);
                    else
                        media.Source = new Uri(new DirectoryInfo("./music/").FullName + "Default.m4a");
                    media.MediaEnded += Player_MediaEnded;
                    media.Position = TimeSpan.Zero;
                    media.Loaded += Media_Loaded;
                });
            }
            if (tdTimer.I == 1)
            {
                //更新A6C动画
                if (tdTimer.PTs > 0)
                {
                    (MainArr[ArrIndex] as MainClass).LastTime = tdTimer.PTs;
                    A6CStringsArr.Insert(1, ((MainArr[ArrIndex] as MainClass).LastTime / 60).ToString() + "分钟");
                    A6CStringsArr.RemoveAt(2); 
                }
            }
        }

        private void Media_Loaded(object sender, RoutedEventArgs e)
        {
            media.Play();
        }

        private void f6685(string s)
        {
            Btn2(null, null);
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            if(tdTimer.STs <= 0)
                Close();
        }

        //命令接口
        private void MW2Event_mw2event(object sender, MW2Args e)
        {
            if (e.S == "Close")
                this.Close();
            else if (e.S.StartsWith("Rename"))
            {
                string[] s = e.S.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (s.Length == 2)
                {
                    (MainArr[ArrIndex] as MainClass).Name = s[1];
                    A6CStringsArr[0] = s[1];
                }
            }
        }

        ArrayList A6CStringsArr = new ArrayList();  //底部动画显示的String的数组
        /*
        已指定的显示顺序：
        1.流名称
        2.流的剩余时间
        3.是否有新通知

         */
        System.Timers.Timer timer;
        private void InitA6CAnimation()
        {
            timer = new System.Timers.Timer(6500);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Elapsed += Timer_Elapsed3;
            timer.Start();
        }

        int i6634 = 0, i8766 = 0;
        private void Timer_Elapsed3(object sender, System.Timers.ElapsedEventArgs e)
        {
            //轮换背景
            i6634++;
            if (i6634 > 2 & BackGroundUris.Length > 0)
            {
                i6634 = 0;
                i8766++;
                i8766 %= BackGroundUris.Length;

                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    A1IB.ImageSource = new BitmapImage(new Uri(BackGroundUris[i8766]));
                });
            }
        }

        short n9563 = 0;
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (A6CStringsArr.Count > 1)
            {
                if (n9563 == 0)
                {
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        DoubleAnimation doubleAnimation = new DoubleAnimation
                        {
                            From = 0,
                            To = -40,
                            Duration = TimeSpan.FromSeconds(1)
                        };
                        B1DP.BeginAnimation(Canvas.TopProperty, doubleAnimation);

                        DoubleAnimation doubleAnimation1 = new DoubleAnimation
                        {
                            From = 40,
                            To = 0,
                            Duration = TimeSpan.FromSeconds(1)
                        };
                        B1L.Content = A6CStringsArr[0] as string;
                        B1L.BeginAnimation(Canvas.TopProperty, doubleAnimation1);
                        n9563++;
                    });
                }
                else if (n9563 >= A6CStringsArr.Count)
                {
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        DoubleAnimation doubleAnimation = new DoubleAnimation
                        {
                            From = 0,
                            To = -40,
                            Duration = TimeSpan.FromSeconds(1)
                        };

                        DoubleAnimation doubleAnimation1 = new DoubleAnimation
                        {
                            From = 40,
                            To = 0,
                            Duration = TimeSpan.FromSeconds(1)
                        };
                        B2L.BeginAnimation(Canvas.TopProperty, doubleAnimation);
                        B1DP.BeginAnimation(Canvas.TopProperty, doubleAnimation1);
                    });

                    n9563 = 0;
                }
                else
                {
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        DoubleAnimation doubleAnimation = new DoubleAnimation
                        {
                            From = 0,
                            To = -40,
                            Duration = TimeSpan.FromSeconds(1)
                        };

                        DoubleAnimation doubleAnimation1 = new DoubleAnimation
                        {
                            From = 40,
                            To = 0,
                            Duration = TimeSpan.FromSeconds(1)
                        };

                        if (n9563 % 2 == 1)
                        {
                            B2L.Content = A6CStringsArr[n9563] as string;
                            B1L.BeginAnimation(Canvas.TopProperty, doubleAnimation);
                            B2L.BeginAnimation(Canvas.TopProperty, doubleAnimation1);
                        }
                        else
                        {
                            B1L.Content = A6CStringsArr[n9563] as string;
                            B2L.BeginAnimation(Canvas.TopProperty, doubleAnimation);
                            B1L.BeginAnimation(Canvas.TopProperty, doubleAnimation1);
                        }

                        n9563++;
                    });
                } 
            }
        }

        int MW2MissionStackCount = 0;
        private void ShowWindow(bool isNeedCheckCount)
        {
            if (MW2MissionStackCount==0|!isNeedCheckCount)
            {
                WindowState = WindowState.Normal;
                this.Show();
                if (!this.Topmost)          //若本来就置顶，则可以不管这个
                {
                    //将窗口排到最前
                    this.Topmost = true;
                    this.Topmost = false;
                }
                this.Activate();            //相当于任务栏按一下 
            }
        }

        private void PanelDrag(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
            if (A4SP.Visibility == Visibility.Visible)
                A4SPDown();
            if (Canvas.GetBottom(A2TB) == 0)
                A3GA2TBPC(true);
        }


        private void RE(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            Rectangle rectangle = (Rectangle)sender;
            doubleAnimation.From = rectangle.Opacity;
            doubleAnimation.To = 0.2;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.5);
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            rectangle.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void RL(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            Rectangle rectangle = (Rectangle)sender;
            doubleAnimation.From = rectangle.Opacity;
            doubleAnimation.To = 0;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            rectangle.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            A4SP.Visibility = Visibility.Collapsed;
        }

        private void A4SPDown()
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = Canvas.GetTop(A4SP);
            doubleAnimation.To = 0;
            doubleAnimation.Completed += DoubleAnimation_Completed;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.3);
            A4SP.BeginAnimation(Canvas.TopProperty, doubleAnimation);
        }

        private void Btn3(object sender, RoutedEventArgs e)
        {
            A4SP.Visibility = Visibility.Visible;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = Canvas.GetTop(A4SP);
            doubleAnimation.To = -(A2G.ActualHeight - 100);
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.3);
            A4SP.BeginAnimation(Canvas.TopProperty, doubleAnimation);
        }

        private void Btn1(object sender, RoutedEventArgs e)
        {
            A3GA2TBPC(false);
            A2TB.Focus();
        }

        private void A3GA2TBPC(bool v)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = A3G.Opacity;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.15);
            DoubleAnimation doubleAnimation1 = new DoubleAnimation();
            doubleAnimation1.Duration = TimeSpan.FromSeconds(0.15);
            if (v)
            {
                SetA6SPVisibility(false);

                doubleAnimation.To = 1;
                doubleAnimation1.From = Canvas.GetBottom(A2TB);
                doubleAnimation1.To = -A2TB.ActualHeight;
                doubleAnimation.BeginTime = TimeSpan.FromSeconds(0.15);
            }
            else
            {
                doubleAnimation.To = 0;
                doubleAnimation1.From = Canvas.GetBottom(A2TB);
                doubleAnimation1.To = 0;
                doubleAnimation1.BeginTime = TimeSpan.FromSeconds(0.15);
            }
            A3G.BeginAnimation(OpacityProperty, doubleAnimation);
            A2TB.BeginAnimation(Canvas.BottomProperty, doubleAnimation1);
        }

        private void BPE(object sender, System.Windows.Input.MouseEventArgs e)
        {
            B1BE.Radius = 10;
            A1EP.Visibility = Visibility.Visible;
        }

        private void BPL(object sender, System.Windows.Input.MouseEventArgs e)
        {
            B1BE.Radius = 0;
            A1EP.Visibility = Visibility.Collapsed;
        }

        private void DragE(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Canvas.SetTop(A1EP, Mouse.GetPosition(this as FrameworkElement).Y - (A1G.ActualHeight / 2));
        }

        private void DCE(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            double d = Canvas.GetTop(A1EP);
            double c = mw2.Height;
            if (d > (c / 3) - 50 & d < (c / 3) + 50)
            {
                A5C.Visibility = Visibility.Collapsed;
                A2G.Visibility = Visibility.Visible;
            }
            else if (d < 50 - (c / 3) & d > -(c / 3) - 50)
                Hide();
            Canvas.SetTop(A1EP, 0);
        }

        private void EE(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            DoubleAnimation doubleAnimation1 = new DoubleAnimation();
            DoubleAnimation doubleAnimation2 = new DoubleAnimation();
            doubleAnimation.From = Canvas.GetTop(I1);
            doubleAnimation1.From = Canvas.GetTop(I2);
            doubleAnimation2.From = I1.Opacity;
            doubleAnimation.To = 0;
            doubleAnimation1.To = 0;
            doubleAnimation2.To = 1;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.5);
            doubleAnimation1.Duration = TimeSpan.FromSeconds(0.5);
            doubleAnimation2.Duration = TimeSpan.FromSeconds(0.5);
            I1.BeginAnimation(Canvas.TopProperty, doubleAnimation);
            I1.BeginAnimation(OpacityProperty, doubleAnimation2);
            I2.BeginAnimation(Canvas.TopProperty, doubleAnimation1);
            I2.BeginAnimation(OpacityProperty, doubleAnimation2);
        }

        private void EL(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            DoubleAnimation doubleAnimation1 = new DoubleAnimation();
            DoubleAnimation doubleAnimation2 = new DoubleAnimation();
            doubleAnimation.From = Canvas.GetTop(I1);
            doubleAnimation1.From = Canvas.GetTop(I2);
            doubleAnimation2.From = I1.Opacity;
            doubleAnimation.To = 110;
            doubleAnimation1.To = -110;
            doubleAnimation2.To = 0;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.5);
            doubleAnimation1.Duration = TimeSpan.FromSeconds(0.5);
            doubleAnimation2.Duration = TimeSpan.FromSeconds(0.5);
            I1.BeginAnimation(Canvas.TopProperty, doubleAnimation);
            I1.BeginAnimation(OpacityProperty, doubleAnimation2);
            I2.BeginAnimation(Canvas.TopProperty, doubleAnimation1);
            I2.BeginAnimation(OpacityProperty, doubleAnimation2);
        }

        private void PanelLock(object sender, MouseButtonEventArgs e)
        {
            A1TB.IsReadOnly = false;
            A1L.Visibility = Visibility.Collapsed;
            A3L.Content = "锁定模式";
            FreshTextBox(true, ArrIndex);
            A1TB.Focus();
        }

        private void PanelFade(object sender, MouseButtonEventArgs e)
        {
            A5C.Visibility = Visibility.Visible;
            A2G.Visibility = Visibility.Hidden;
        }

        private void TMode(object sender, MouseButtonEventArgs e)
        {
            A5SP.Visibility = Visibility.Visible;
            if ((string)A3L.Content == "锁定模式")
            {
                A1TB.IsReadOnly = true;
                A1L.Visibility = Visibility.Visible;
                A3L.Content = "可编辑模式";
                SaveToClass(ArrIndex);
                A1B.Focus();
            }
            else
            {
                A1TB.IsReadOnly = false;
                A1L.Visibility = Visibility.Collapsed;
                A3L.Content = "锁定模式";
                FreshTextBox(true, ArrIndex);
            }
            A4SPDown();
        }

        private void WidthChanging(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double a123 = StartingWidth + (Mouse.GetPosition(this as FrameworkElement).X - MouseStartingPosition.X);
            if (a123 < mw2.MaxWidth & a123 > mw2.MinWidth)
            {
                mw2.Width = a123;
            }
        }

        Point MouseStartingPosition=new Point();
        double StartingWidth,StartingHeight;

        private void HeightChanging(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double a123 = StartingHeight + (Mouse.GetPosition(this as FrameworkElement).Y - MouseStartingPosition.Y);
            if (a123 < mw2.MaxHeight & a123 > mw2.MinHeight)
            {
                mw2.Height = a123;
            }
        }

        private void Wid_Hei_Changing(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double a123 = StartingWidth + (Mouse.GetPosition(this as FrameworkElement).X - MouseStartingPosition.X);
            if (a123 < mw2.MaxWidth & a123 > mw2.MinWidth)
            {
                mw2.Width = a123;
            }
            a123 = StartingHeight + (Mouse.GetPosition(this as FrameworkElement).Y - MouseStartingPosition.Y);
            if (a123 < mw2.MaxHeight & a123 > mw2.MinHeight)
            {
                mw2.Height = a123;
            }
        }

        MW3 mw3 = null;
        private void Btn2(object sender, RoutedEventArgs e)
        {
            TimerWindow timerWindow = new TimerWindow(tdTimer.StaticTime, (MainArr[ArrIndex] as MainClass).LastTime / 60,this);
            timerWindow.ShowDialog();
            if (timerWindow.HasChanged)
            {
                string[] s = timerWindow.GetTime();
                tdTimer.SetStaticTime(int.Parse(s[0]));
                tdTimer.SetProcessTime(int.Parse(s[1]));
                (MainArr[ArrIndex] as MainClass).LastTime = tdTimer.PTs;
                if (A5L.Content.ToString() == "隐藏命令")
                    FreshTextBox(true, ArrIndex);
                else
                    FreshTextBox(false, ArrIndex);
                if (media != null)
                {
                    media.Pause();
                    media.Close();
                } 
            }
        }

        private void ShowMW3(string flowItemsRootPath)          //待使用
        {
            if (mw3 == null)
            {
                mw3 = new MW3(this, MW3Loading, MW3Closing, flowItemsRootPath);
                mw3.Show();
            }
            else if (!mw3.IsShowingMW3)
            {
                mw3 = new MW3(this, MW3Loading, MW3Closing, flowItemsRootPath);
                mw3.Show();
            }
        }

        private void MW3Loading()
        {
            MW2MissionStackCount++;
            Hide();
        }

        private void MW3Closing()
        {
            MW2MissionStackCount--;
            if (MW2MissionStackCount < 0)
                MW2MissionStackCount = 0;
            ToDo2WindowHasStarted.ShowWindow(this);
        }

        private void A1LWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                A1TB.LineDown();
            else if (e.Delta > 0)
                A1TB.LineUp();
        }

        private void CommandHelpBtn(object sender, RoutedEventArgs e)
        {
            //待修改！！！
            A1TB.Text += App.commandClass.Excute("#", false);
            A1TB.ScrollToEnd();
        }

        private void A2TBTextChanged(object sender, TextChangedEventArgs e)
        {
            if (A2TB.Text.StartsWith("#"))
            {
                SetA6SPVisibility(true);
            }
            else
            {
                SetA6SPVisibility(false);
            }
        }

        private void SetA6SPVisibility(bool b)
        {
            if (b)
            {
                A6SP.Visibility = Visibility.Visible;
                Canvas.SetTop(A6SP, -A6SP.ActualHeight - A2TB.ActualHeight);
                A2TB.AcceptsTab = false;
            }
            else
            {
                Canvas.SetTop(A6SP, 5);
                A6SP.Visibility = Visibility.Collapsed;
                A2TB.AcceptsTab = true;
            }
        }

        private void CheckBtnFocus(object sender, RoutedEventArgs e)
        {
            if (A6SP.Visibility == Visibility.Visible)
            {
                A2TB.Focus();
            }
        }

        private void BtnTopMost(object sender, MouseButtonEventArgs e)
        {
            if (Topmost)
            {
                Topmost = false;
                A4L.Content = "置顶";
            }
            else
            {
                Topmost = true;
                A4L.Content = "取消置顶";
            }
        }

        Label label = null;
        System.Timers.Timer t6341 = null;
        int LastArrIndex = 0;
        private void ChangeOverMainClass(object sender, MouseWheelEventArgs e)
        {
            if (t6341 != null)
            {
                t6341.Stop();
                t6341.Close();
            }
            t6341 = new System.Timers.Timer(1500);
            t6341.AutoReset = false;
            t6341.Elapsed += Timer_Elapsed1;
            t6341.Start();
            LastArrIndex = ArrIndex;
            if (e.Delta < 0)
                ArrIndex++;
            else
                ArrIndex--;
            if (ArrIndex < 0)
                ArrIndex = MainArr.Count - 1;
            else if (ArrIndex >= MainArr.Count)
                ArrIndex = 0;


            if (label == null)
            {
                label = new Label
                {
                    Background = Brushes.White,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontSize = 24
                };
                A2G.Children.Add(label); Grid.SetRowSpan(label, 2);
                Panel.SetZIndex(label, 5);
                label.MouseWheel += ChangeOverMainClass;
            }
            label.Content = (MainArr[ArrIndex] as MainClass).Name + "\n" + ((MainArr[ArrIndex] as MainClass).LastTime/60).ToString() + "分钟";
        }
        private void Timer_Elapsed1(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                label.IsEnabled = false;
                if (A5L.Content.ToString() == "隐藏命令")
                    FreshTextBox(true, ArrIndex);
                else
                    FreshTextBox(false, ArrIndex);
                DoubleAnimation doubleAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5)
                };
                doubleAnimation.Completed += DoubleAnimation_Completed1;
                label.BeginAnimation(OpacityProperty, doubleAnimation);
            });
        }
        private void DoubleAnimation_Completed1(object sender, EventArgs e)
        {
            A2G.Children.Remove(label);
            label = null;
        }

        private void TextShowCommand(object sender, MouseButtonEventArgs e)
        {
            if (A5L.Content.ToString() == "显示命令")
            {
                FreshTextBox(true, ArrIndex);
            }
            else
            {
                FreshTextBox(false, ArrIndex);
                if (A3L.Content.ToString() == "锁定模式")
                {
                    TMode(null,null);
                }
            }
        }

        private void A1TBChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.IsInitialized)
                {
                    if (A3L.Content.ToString() == "锁定模式")
                        HasUsedTMode = true; 
                }
            }
            catch (Exception)
            {

            }
        }


        private void A2TBKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                int i = A2TB.SelectionStart;
                A2TB.Text = A2TB.Text.Insert(A2TB.SelectionStart, "\n");
                A2TB.SelectionStart = i + 1;
            }
            else if (e.Key == Key.Enter)
            {
                if (A2TB.Text.StartsWith("#"))
                {
                    string s = App.commandClass.Excute(A2TB.Text, false);
                    (MainArr[ArrIndex] as MainClass).LineArr.Add(new MainClass.Line
                    {
                        Content = A2TB.Text + " : " + s,
                        index = (MainArr[ArrIndex] as MainClass).LineArr.Count,
                        isCommand = true
                    });
                    if (A5L.Content.ToString() == "隐藏命令")
                        FreshTextBox(true, ArrIndex);
                    else
                    {
                        A1TB.Text += s + "\n";
                        A1TB.ScrollToEnd();
                    }
                }
                else if (A2TB.Text == "")
                {
                    A3GA2TBPC(true);
                    A2B.Focus();
                }
                else
                {
                    A1TB.Text += A2TB.Text + "\n";
                    (MainArr[ArrIndex] as MainClass).LineArr.Add(new MainClass.Line
                    {
                        Content = A2TB.Text,
                        index = (MainArr[ArrIndex] as MainClass).LineArr.Count,
                        isCommand = false
                    });
                    A1TB.ScrollToEnd();
                }
                A2TB.Text = "";
            }
        }

        private void ExcuteAllCommands(object sender, MouseButtonEventArgs e)
        {
            if (A3L.Content.ToString() == "锁定模式")
            {
                TMode(null, null);
            }
            else
                A4SPDown();
            A1TB.Text += App.commandClass.BatchExcute(MainArr[ArrIndex] as MainClass);
            A1TB.ScrollToEnd();
        }

        private void ShowConfigWindow(object sender, MouseButtonEventArgs e)
        {
            new MW2Config(this).ShowDialog();
            InitStyle();
        }

        private void noticeClose(object sender, MouseButtonEventArgs e)
        {
            Ti_Elapsed(null, null);
        }

        private void WCS(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            MouseStartingPosition = Mouse.GetPosition(this as FrameworkElement);
            StartingWidth = mw2.ActualWidth;
            StartingHeight = mw2.ActualHeight;
            Console.WriteLine(mw2.Height);
        }
    }

    //Bingding
    public class ClickBinding : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class A4SPHeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value - 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    //全局快捷键

    static class HotKey
    {
        #region 系统api
        [DllImport("user32.dll")]           //引用系统dll（API）
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, HotkeyModifiers fsModifiers, uint vk);

        [DllImport("user32.dll")]
        static extern bool UnRegisterHotKey(IntPtr hWnd, int id);
        #endregion
        public static void Regist(Window window, HotkeyModifiers fsModifiers, Key key, HotKeyCallBackHanlder callback)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var _hwndSource = HwndSource.FromHwnd(hwnd);
            _hwndSource.AddHook(WndProc);       //监听所有窗口消息，注意查看VS2019的注释

            int id = keyid++;

            var vk = KeyInterop.VirtualKeyFromKey(key);
            if (!RegisterHotKey(hwnd, id, fsModifiers, (uint)vk))
                new Window4("快捷键失效",0).ShowDialog();
            keymap[id] = callback;
        }


        static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (keymap.TryGetValue(id, out var callback))
                {
                    //当快捷键被使用时生效
                    callback();
                }
            }
            return IntPtr.Zero;
        }


        public static void UnRegist(IntPtr hWnd, HotKeyCallBackHanlder callback)
        {
            foreach (KeyValuePair<int, HotKeyCallBackHanlder> var in keymap)
            {
                if (var.Value == callback)
                    UnRegisterHotKey(hWnd, var.Key);
            }
        }


        const int WM_HOTKEY = 0x312;        //注意不是乘号
        static int keyid = 10;
        static Dictionary<int, HotKeyCallBackHanlder> keymap = new Dictionary<int, HotKeyCallBackHanlder>();




        public delegate void HotKeyCallBackHanlder();
    }
    enum HotkeyModifiers
    {
        MOD_ALT = 0x1,
        MOD_CONTROL = 0x2,
        MOD_SHIFT = 0x4,
        MOD_WIN = 0x8
    }
}

public class MW2Args : EventArgs
{
    string s;

    public MW2Args(string s)
    {
        this.S = s;
    }

    public string S { get => s; set => s = value; }
}

public static class MW2Event
{
    public static event EventHandler<MW2Args> mw2event;    //myArg是自定义参数类型，参照上下文理解。因为： 凡是 EventHandler<参数类型> 的事件（类型？）产生的变量？（对象？），参数都 只能 是两个：object(sender), 自定义类型(myArg arg)  。

    public static string Excute(string str)
    {
        var arg = new MW2Args("") { S = str };        //参数可直接为空，因为没用，但是通过{}内的...设置属性
        mw2event?.Invoke(null, arg);                     //凡是 EventHandler<参数类型> 的事件（类型？）产生的变量？（对象？），参数都是两个：object(sender), 自定义类型
        return null;
    }
}

public delegate void MW2noticeExcute(string p);             //p 为 参数
public class MW2noticeArgs : EventArgs
{
    bool isImportant;
    Image image;                //只需要定义Source属性
    string text;
    MW2noticeExcute excute;     //点击通知消息按钮执行的方法
    string parameter;           //执行方法的参数

    public MW2noticeArgs(bool isImportant, Image image, string text, MW2noticeExcute excute, string parameter)
    {
        this.IsImportant = isImportant;
        this.Image = image;
        this.Text = text;
        this.Excute = excute;
        this.Parameter = parameter;
    }

    public bool IsImportant { get => isImportant; set => isImportant = value; }
    public Image Image { get => image; set => image = value; }
    public string Text { get => text; set => text = value; }
    public MW2noticeExcute Excute { get => excute; set => excute = value; }
    public string Parameter { get => parameter; set => parameter = value; }
}

public static class MW2noticeEvent
{
    public static EventHandler<MW2noticeArgs> mw2noticeEvent;
    public static MediaPlayer player;
    public static Uri playerUri;

    //发送通知
    public static void ShowNotice(bool isImportant, Image img,string str,MW2noticeExcute excute, string parameter, object sender)
    {
        var arg = new MW2noticeArgs(isImportant, img, str, excute, parameter);
        mw2noticeEvent?.Invoke(sender, arg);
        if (playerUri != null)
        {
            player = new MediaPlayer();
            player.Open(playerUri);
            player.Play(); 
        }
    }
}

