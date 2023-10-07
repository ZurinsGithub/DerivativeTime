using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToDo2.Properties;

namespace ToDo2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool Window1isLoad;
        public MainWindow()
        {
            //InitializeToDo();
            InitializeComponent();
            LayOut();
            this.Loaded += MainWindow_Loaded;
            this.Activated += MainWindow_Activated;
        }

        private void LayOut()
        {
            SetBackground();
            Config1LO();
        }

        private void Config1LO()
        {
            if (ToDo2Settings.Default.AutoSavePassKey)
            {
                if (ToDo2Settings.Default.PassKey != "")
                {
                    A1T.Text = ToDo2Settings.Default.PassKey;
                }
            }

        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;      //有时候程序反应没那么快，需要手动设置
            RunShowTips();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //设置项
            if (ToDo2Settings.Default.AutoUpdateBackground)
            {
                Thread thread = new Thread(IMGtran);
                thread.IsBackground = true;
                thread.Start();
            }

            //防止多开
            new ToDo2WindowHasStarted(this, new ProcessOpen(ShowWindow));
        }


        private void ShowWindow(bool v)
        {
            WindowState = WindowState.Normal;
            this.Show();
            if (!this.Topmost)          //若本来就置顶，则可以不管这个
            {
                //将窗口排到最前
                this.Topmost = true;
                this.Topmost = false;
            }
        }

        private void SetBackground()
        {
            try
            {
                try
                {
                    //警告：exe程序在运行时，文件需要被占用，所以要复制一个文件供exe程序使用，这样更新文件才会成功覆盖原文件（否则会因为被exe程序占用而覆盖失败！！！）
                    System.IO.File.Delete(@".\backgroundIMG\backgroundImage1.jpg");
                    System.IO.File.Copy(@".\backgroundIMG\backgroundImage.jpg", @".\backgroundIMG\backgroundImage1.jpg");
                    //先删除原有的文件才能复制
                }
                catch (Exception)
                {
                }
                string[] TxtContent = System.IO.File.ReadAllText(@".\backgroundIMG\backgroundImage.txt").Split(new char[] { '+', '+' },StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine(TxtContent.Length);
                if (TxtContent.Length == 2)
                {
                    A1L.Content = "由 " + TxtContent[0] + " 分享的图片\n    " + TxtContent[1];
                }
                A1IMG.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(@".\backgroundIMG\backgroundImage1.jpg")));       //此处Uri需要绝对路径
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void IMGtran()
        {
            AddTips(new TipsShowing(3, "访问ToDo社区的背景...",1));        //显示通知

            NetIMG netIMG = new NetIMG(ToDo2Settings.Default.ip, "Default", ToDo2Settings.Default.port);
            netIMG.Picture_transmission();

            if (netIMG.NetState == "Error")
            {
                AddTips(new TipsShowing(3, "访问ToDo社区时发生了错误...", 2));
            }
            else if (netIMG.NetState == "Normal")
            {
                AddTips(new TipsShowing(2, "已更新背景。",3));
            }
            netIMG.SocketClose();
        }

        //初始化文件等
        private void InitializeToDo()
        {
            Window1 window1 = new Window1();
            window1.ShowDialog();
        }


        public class NetIMG
        {
            Socket socket;
            string ip, requestMessage;
            int port;
            private string netState = "Normal";
            public NetIMG(string ip, string requestMessage, int port)
            {
                this.ip = ip;
                this.requestMessage = requestMessage;
                this.port = port;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress address = IPAddress.Parse(ip);
                IPEndPoint endPoint = new IPEndPoint(address, port);
                try
                {
                    socket.Connect(endPoint);
                    Console.Write("已连接");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    NetState = "Error";
                }
            }
            public void Picture_transmission()
            {
                try
                {
                    string IMGpath = @".\backgroundIMG"; //存储图片的路径
                    byte[] buffer = new byte[1024 * 32];
                    socket.Send(Encoding.Default.GetBytes("Step1:IMG"));//发送请求
                    int count = socket.Receive(buffer);
                    int length = 0;
                    string[] command = Encoding.Default.GetString(buffer, 0, count).Split(new char[] { ',' });
                    //command 0.文件大小(length) 1.文件名
                    length = Convert.ToInt32(command[0]);
                    Console.WriteLine(command[0] + length);
                    socket.Send(Encoding.Default.GetBytes("IMGReady"));
                    long hasReceived = 0L;
                    using (FileStream fileStream = new FileStream(System.IO.Path.Combine(IMGpath, command[1]), FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer1 = new byte[1024 * 128];
                        int received = 1;
                        while (hasReceived < length)
                        {
                            received = socket.Receive(buffer1);
                            fileStream.Write(buffer1, 0, received);
                            fileStream.Flush();
                            hasReceived += (long)received;
                        }
                    }
                    Console.WriteLine("传输完毕");
                    buffer = Encoding.Default.GetBytes("TXTReceived");
                    socket.Send(buffer);
                    byte[] buffer2 = new byte[1024 * 32];
                    count = socket.Receive(buffer2);
                    command = Encoding.Default.GetString(buffer2, 0, count).Split(new char[] { ',' });
                    Console.WriteLine(Encoding.Default.GetString(buffer2, 0, count));
                    length = Convert.ToInt32(command[0]);
                    socket.Send(Encoding.Default.GetBytes("TXTReady"));
                    hasReceived = 0L;
                    using (FileStream fileStream = new FileStream(System.IO.Path.Combine(IMGpath, command[1]), FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer3 = new byte[1024 * 128];
                        int received = 1;
                        while (hasReceived < length)
                        {
                            received = socket.Receive(buffer3);
                            fileStream.Write(buffer3, 0, received);
                            fileStream.Flush();
                            hasReceived += (long)received;
                        }
                    }
                    Console.WriteLine(hasReceived + "+" + command[0] + "+" + command[1]);
                }
                catch (Exception e)
                {
                    NetState = "Error";
                    Console.WriteLine(e.Message);
                }
            }
            public void SocketClose()
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception)
                {
                }
            }

            string LoginNum;        //登陆序号
            bool hasLogin = false;  //登陆状态

            public string LoginNum1 { get => LoginNum; set => LoginNum = value; }
            public bool HasLogin { get => hasLogin; set => hasLogin = value; }
            public string NetState { get => netState; set => netState = value; }

            public void LoadInServer(string UserKey)
            {
                try
                {

                    socket.Send(Encoding.Default.GetBytes("Login"));
                    byte[] buffer = new byte[1024 * 4];
                    socket.Receive(buffer);
                    string str1 = Encoding.Default.GetString(buffer);
                    if (str1.Contains("ServerLoginReady+"))
                    {
                        string[] str = str1.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                        if (str.Length == 2)
                        {
                            LoginNum1 = str[1];
                        }
                        else
                            NetState = "Error";
                    }
                    else
                        NetState = "Error";
                    if (NetState != "Error")
                    {
                        socket.Send(Encoding.Default.GetBytes(UserKey));
                        byte[] buffer1 = new byte[1024 * 4];
                        socket.Receive(buffer1);
                        if (Encoding.Default.GetString(buffer1).Contains("LoginSuccess"))
                        {
                            HasLogin = true;
                        }
                        else
                        {
                            NetState = "UserKeyError";
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("LoginError: " + e.Message);
                }
            }
        }

        private void RE(object sender, MouseEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            Rectangle rectangle = (Rectangle)sender;
            doubleAnimation.From = rectangle.Opacity;
            doubleAnimation.To = 0.2;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.5);
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            rectangle.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void RL(object sender, MouseEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            Rectangle rectangle = (Rectangle)sender;
            doubleAnimation.From = rectangle.Opacity;
            doubleAnimation.To = 0;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            rectangle.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void BackgroundIMGClick(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            DoubleAnimation doubleAnimation1 = new DoubleAnimation();
            doubleAnimation1.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.From = A1C.Width;
            if (A1C.Width == 0)
            {
                doubleAnimation.To = 370;
                doubleAnimation1.To = 1;
                IMGtipsHide();
                A2C.Visibility = Visibility.Collapsed;
            }
            else
            {
                A2C.Visibility = Visibility.Visible;
                IMGtipsShow();
                doubleAnimation.To = 0;
                doubleAnimation1.To = 0;
            }
            doubleAnimation.Duration = TimeSpan.FromSeconds(1);
            doubleAnimation1.Duration = TimeSpan.FromSeconds(1);
            doubleAnimation.EasingFunction = new PowerEase();
            A1C.BeginAnimation(WidthProperty, doubleAnimation);
            A1C.BeginAnimation(OpacityProperty, doubleAnimation1);
        }

        private void LoadIn(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.From = 1;
            doubleAnimation.To = 0;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            doubleAnimation.Completed += DoubleAnimation_Completed;
            A1SP.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            A1SP.Visibility = Visibility.Collapsed;
            A2SP.Visibility = Visibility.Visible;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.From = 0;
            doubleAnimation.To = 1;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            A2SP.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void BackTo(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.From = 1;
            doubleAnimation.To = 0;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            A2SP.BeginAnimation(OpacityProperty, doubleAnimation);
            A2SP.Visibility = Visibility.Collapsed;
            A1SP.Visibility = Visibility.Visible;
            doubleAnimation.From = 0;
            doubleAnimation.To = 1;
            A1SP.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void A3SPback(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = 600;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            doubleAnimation.EasingFunction = new PowerEase();
            A1BE.Radius = 0;
            A2BE.Radius = 0;
            A3SP.BeginAnimation(Canvas.TopProperty, doubleAnimation);
        }

        private void A3SPup(object sender, RoutedEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 600;
            doubleAnimation.To = 0;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.2);
            doubleAnimation.EasingFunction = new PowerEase();
            doubleAnimation.Completed += DoubleAnimation_Completed1;
            A3SP.BeginAnimation(Canvas.TopProperty, doubleAnimation);
        }

        private void DoubleAnimation_Completed1(object sender, EventArgs e)
        {
            A1BE.Radius = 14;
            A2BE.Radius = 14;
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void IMGtipsShow(object sender, MouseEventArgs e)
        {
            if (A1C.Width == 0)
            {
                IMGtipsShow();
            }
        }
        private void IMGtipsShow()
        {
            //A1L.Content
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.From = A2C.Opacity;
            doubleAnimation.To = 1;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.5);
            A2C.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void IMGtipsHide(object sender, MouseEventArgs e)
        {
            IMGtipsHide();
        }

        private void IMGtipsHide()
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = A2C.Opacity;
            doubleAnimation.To = 0;
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.5);
            A2C.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        
        public class TipsShowing
        {
            Double ShowTime;
            string Text;
            short TipTag;

            public TipsShowing(double showTime, string text, short tipTag)
            {
                ShowTime1 = showTime;
                Text1 = text;
                TipTag1 = tipTag;
            }

            public double ShowTime1 { get => ShowTime; set => ShowTime = value; }
            public string Text1 { get => Text; set => Text = value; }
            public short TipTag1 { get => TipTag; set => TipTag = value; }
        }
        ArrayList TipsArr = new ArrayList();
        short TagOfTip = 0;
        Double ShowTime;
        private void A3CShow(Double ST,string TextTip,short TipsTag)
        {
            A2L.Content = TextTip;
            ShowTime = ST;
            TagOfTip = TipsTag;

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = -300;
            doubleAnimation.To = 20;
            doubleAnimation.Duration = TimeSpan.FromSeconds(1);
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.Completed += DoubleAnimation_Completed2;
            A3C.BeginAnimation(Canvas.RightProperty, doubleAnimation);


            //决定通知框加载的动画或图片
            if (TagOfTip == 1 & !WaitingAnimationIsExist)       //1是加载圆点等待动画
            {
                WaitAnimationShow();
            }
            else if (TagOfTip == 2)
            {
                ShowErrorTipsIMG(false);
            }
            else if (TagOfTip == 3)
            {
                ShowErrorTipsIMG(true);
            }
        }

        private void DoubleAnimation_Completed2(object sender, EventArgs e)
        {

            //退出通知框动画
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.To = -300;
            doubleAnimation.From = 20;
            doubleAnimation.Duration = TimeSpan.FromSeconds(1);
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.BeginTime = TimeSpan.FromSeconds(ShowTime);
            doubleAnimation.Completed += DoubleAnimation_Completed3;
            A3C.BeginAnimation(Canvas.RightProperty, doubleAnimation);
        }

        private void ShowErrorTipsIMG(bool IC)
        {
            Image image = new Image();
            if (!IC)
            {
                image.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(@".\IMG\ErrorLogo.png")));
            }
            else
            {
                image.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(@".\IMG\CorrectLogo.png")));
            }
            image.Margin = new Thickness(12);
            image.Width = 36;
            A3C.Children.Add(image);
        }

        private void DoubleAnimation_Completed3(object sender, EventArgs e)
        {
            IsShowingTips = false; try
            {

                if (WaitingAnimationIsExist)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        A3C.Children.RemoveAt(1);
                    }
                    WaitingAnimationIsExist = false;
                }
                else
                {
                    A3C.Children.RemoveAt(1);
                }
            }
            catch (Exception)
            {
            }
            RunShowTips();
        }

        bool WaitingAnimationIsExist = false;        //等待动画是否已加载
        private void WaitAnimationShow()
        {
            for (short i = 0; i < 4; i++)
            {
                ElPoint elPoint = new ElPoint(i);
                A3C.Children.Add(elPoint.ellipse);
                elPoint.BeginAni();
            }
            WaitingAnimationIsExist = true;
        }
        
        
        public class ElPoint
        {
            public Ellipse ellipse;
            TransformGroup transformGroup;
            RotateTransform rotateTransform;
            DoubleAnimation doubleAnimation;
            WaitingEase waitingEase;
            public ElPoint(short i)
            {
                ellipse = new Ellipse();
                transformGroup = new TransformGroup();
                rotateTransform = new RotateTransform(0, 18, 18);
                doubleAnimation = new DoubleAnimation();
                waitingEase = new WaitingEase();
                ellipse.Width = 5;
                ellipse.Height = 5;
                ellipse.Margin = new Thickness(13);
                ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 140, 255));
                ellipse.RenderTransformOrigin = Point.Parse("-0.5,-0.5");
                transformGroup.Children.Add(rotateTransform);
                ellipse.RenderTransform = transformGroup;
                doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                doubleAnimation.By = 360;
                doubleAnimation.Duration = TimeSpan.FromSeconds(1.5);
                waitingEase.EasingMode = EasingMode.EaseIn;
                doubleAnimation.EasingFunction = waitingEase;
                doubleAnimation.BeginTime = TimeSpan.FromSeconds(0.15 * i);
            }
            public void BeginAni()
            {
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, doubleAnimation);
            }
        }

        public class WaitingEase : EasingFunctionBase
        {
            protected override double EaseInCore(double x)
            {
                if (x <= 0.2)
                {
                    return 0.7 * x;
                }
                else if (x > 0.2 && x <= 0.5)
                {
                    return (x * x + x * 0.5);
                }
                else if (x > 0.5 && x <= 0.8)
                {
                    return (2.5 * x) - (x * x) - 0.5;
                }
                else
                {
                    return 0.7 * x + 0.3;
                }

            }

            protected override Freezable CreateInstanceCore()
            {
                return new WaitingEase();
            }
        }

        //TestButton
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new MW3().ShowDialog();
        }

        private void AddTips(TipsShowing tipsShowing)
        {
            //TipsArr.Add(tipsShowing);
            //RunShowTips();


            this.Dispatcher.Invoke(new Action(() =>
            {
                TipsArr.Add(tipsShowing);
                RunShowTips();
            }));
        }

        bool IsShowingTips = false;     //是否在显示通知框？若是，则不再启动动画，并在动画结束后设置为否（在上面的程序中）；若否，则启动动画。
        private void RunShowTips()
        {
            if (!IsShowingTips&&this.WindowState!=WindowState.Minimized)
            {
                if (TipsArr.Count > 0)
                {
                    IsShowingTips = true;
                    TipsShowing tipsShowing = TipsArr[0] as TipsShowing;
                    Console.WriteLine(tipsShowing.ShowTime1);
                    Console.WriteLine(tipsShowing.Text1);
                    Console.WriteLine(tipsShowing.TipTag1);
                    A3CShow(tipsShowing.ShowTime1, tipsShowing.Text1, tipsShowing.TipTag1);
                    TipsArr.RemoveAt(0);
                } 
            }
        }

        private void ConfigMouseDown(object sender, MouseButtonEventArgs e)
        {
            A4C.Visibility = Visibility.Visible;
            new Window3().ShowDialog();
            A4C.Visibility = Visibility.Collapsed;
        }

        private void About(object sender, MouseButtonEventArgs e)
        {
            A4C.Visibility = Visibility.Visible;
            new Window4("",300).ShowDialog();
            A4C.Visibility = Visibility.Collapsed;
        }

        private void JoinDia(object sender, MouseButtonEventArgs e)
        {
            A4C.Visibility = Visibility.Visible;
            try
            {
                new Window4(System.IO.File.ReadAllText(@".\Config\About.txt"),700).ShowDialog();
            }
            catch (Exception)
            {
            }
            A4C.Visibility = Visibility.Collapsed;
        }

        private void ExpendDia(object sender, MouseButtonEventArgs e)
        {
            A4C.Visibility = Visibility.Visible;
            new Window4("敬请期待！！！",300).ShowDialog();
            A4C.Visibility = Visibility.Collapsed;
        }

        private void KeyLoadIn(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("DengLu!!!");
            //正在登陆时禁止进入
            A2IMG.IsEnabled = false;
            A2IMG.Visibility = Visibility.Collapsed;
            A3L.Visibility = Visibility.Collapsed;
            //是否保存秘钥
            if (ToDo2Settings.Default.AutoSavePassKey)
            {
                ToDo2Settings.Default.PassKey = A1T.Text;
                ToDo2Settings.Default.Save();
            }
            //登陆
            NetIMG netIMG = new NetIMG(ToDo2Settings.Default.ip, "Default", ToDo2Settings.Default.port);
            netIMG.LoadInServer(A1T.Text);
            if (netIMG.HasLogin)
            {
                //跳转...
                Console.WriteLine("LoginSucess!!!");
            }
            else
            {
                if (netIMG.NetState == "UserKeyError")
                {
                    A4C.Visibility = Visibility.Visible;
                    new Window4("该秘钥不存在或已过期", 300).ShowDialog();
                    A4C.Visibility = Visibility.Collapsed;
                }
                else if (netIMG.NetState == "Error")
                {
                    A4C.Visibility = Visibility.Visible;
                    new Window4("登陆时出错了...", 300).ShowDialog();
                    A4C.Visibility = Visibility.Collapsed;
                }
                A2IMG.IsEnabled = true;
                A2IMG.Visibility = Visibility.Visible;
                A3L.Visibility = Visibility.Visible;
            }
        }

        private void EnterMain(object sender, MouseButtonEventArgs e)
        {
            MW3 mW3 = new MW3();
            mW3.Show();
            Close();
        }
    }
}