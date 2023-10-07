using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
using System.Windows.Shapes;

namespace ToDo2
{
    /// <summary>
    /// TimerWindow.xaml 的交互逻辑
    /// </summary>
    
    public static class ASclass             //全局属性
    {
        public readonly static int r = 100;         //半径
    }

    public class ASIVC : IValueConverter        //由 0% -100% 返回一个点，用于绘制曲线
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double a = (double)value * 360 + 90;
            Point p = new Point();
            p.X = Math.Sin(a / 180 * Math.PI) * ASclass.r;
            p.Y = Math.Cos(a / 180 * Math.PI) * ASclass.r;
            return p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ILAIVC : IValueConverter           //决定曲线是否采用远端，当 进度 > 50% 时，采用远端
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((double)value < 0.50001)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class TimerWindow : Window
    {

        public bool HasChanged = false;             //TimerWindow的值是已经经过修改的

        public TimerWindow()
        {
            InitializeComponent();
            this.Loaded += TimerWindow_Loaded;
        }

        public TimerWindow(int staticTime, int processTime,Window owner)
        {
            InitializeComponent();
            this.Owner = owner;
            this.Loaded += TimerWindow_Loaded;
            if (!(staticTime > 0))
                staticTime = 0;
            if (!(processTime > 0))
                processTime = 0;
            A1TB.Text = staticTime.ToString();
            A2TB.Text = processTime.ToString();
            if (staticTime < processTime)
                LastCompare = false;
        }

        private void TimerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double a = Int32.Parse(A1TB.Text.ToString()), b = Int32.Parse(A2TB.Text.ToString());
            if (a >= b)
                A1ASc.Width = b / a;
            else
            {
                A1ASc.Width = a / b;
                LastCompare = false;
                SetDirection(false);
            }
        }

        public string[] GetTime()
        {
            string[] s = new string[2];
            s[0] = A1TB.Text;
            s[1] = A2TB.Text;
            return s;
        }

        bool AnimationUsable = true;
        double AnimationTo = -1;
        public void ExcuteAnimation(double to)
        {
            AnimationTo = to;
            if (AnimationUsable & AnimationTo >= 0)
            {
                AnimationUsable = false;
                AnimationTo = -1;
                DoubleAnimation doubleAnimation = new DoubleAnimation
                {
                    From = A1ASc.Width,
                    To = to,
                    Duration = TimeSpan.FromSeconds(0.8),
                    EasingFunction = new PowerEase()
                };
                doubleAnimation.Completed += DoubleAnimation_Completed;
                A1ASc.BeginAnimation(WidthProperty, doubleAnimation);
            }
        }

        bool changeDirection = false;
        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            if (changeDirection)
            {
                changeDirection = false;
                SetDirection(LastCompare);
            }
            AnimationUsable = true;
            ExcuteAnimation(AnimationTo);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            A1ASat.ScaleY = -1;
            DoubleAnimation doubleAnimation = new DoubleAnimation
            {
                From = 0,
                To = 0.9999,
                Duration = TimeSpan.FromSeconds(5)
            };
            //使用A1ASc的Width属性作为“媒介”，这样A1AS的两个属性（Point、IsLargeArc）都可以通过这个“媒介”得到
            A1ASc.BeginAnimation(WidthProperty, doubleAnimation);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ExcuteAnimation(new Random().NextDouble());
        }

        private void PathBtnClick(object sender, MouseButtonEventArgs e)
        {
            if (A1TB.Text=="0" | A1TB.Text=="")
            {
                path.Stroke = new SolidColorBrush(Color.FromRgb(215, 209, 0));
                A1TB.Focus();
                if (B1L.IsEnabled)
                    LabelBtnClick(B1L, null);
            }
            else if (A2TB.Text=="0" | A2TB.Text== "")
            {
                path.Stroke = new SolidColorBrush(Color.FromRgb(215, 209, 0));
                A2TB.Focus();
                if (B2L.IsEnabled)
                    LabelBtnClick(B2L, null);
            }
            else
            {
                path.Stroke = Brushes.SpringGreen;
                DoubleAnimation doubleAnimation = new DoubleAnimation
                {
                    From = A1ASc.Width,
                    To = 0.9999,
                    Duration = TimeSpan.FromSeconds(1)
                };
                doubleAnimation.Completed += DoubleAnimation_Completed2;
                A1ASc.BeginAnimation(WidthProperty, doubleAnimation);
            }

        
    }
        private void DoubleAnimation_Completed2(object sender, EventArgs e)
        {
            System.Timers.Timer timer = new System.Timers.Timer(800)
            {
                AutoReset = false
            };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
            {
                this.IsEnabled = false;
                Close();
            });
        }

        private void PathBtnEnter(object sender, MouseEventArgs e)
        {
            A1P.Opacity = 0.8;
        }

        private void PathBtnLeave(object sender, MouseEventArgs e)
        {
            A1P.Opacity = 1;
        }

        private void LabelBtnClick(object sender, MouseButtonEventArgs e)
        {
            if((sender as Label).Content.ToString() == "Static")
            {
                SetLabelStyle(B2L, B1L);
                A1TB.Focus();
            }
            else
            {
                SetLabelStyle(B1L, B2L);
                A2TB.Focus();
            }
        }

        bool LabelSettingUsable = true;
        private void SetLabelStyle(Label b2L, Label b1L)
        {
            if (LabelSettingUsable)
            {
                LabelSettingUsable = false;
                b2L.FontSize = b1L.FontSize;
                b1L.FontSize = 12;
                b2L.Foreground = b1L.Foreground;
                b1L.Foreground = Brushes.Black;
                b1L.IsEnabled = false;
                b2L.IsEnabled = true;
                DoubleAnimation doubleAnimation = new DoubleAnimation
                {
                    Duration = TimeSpan.FromSeconds(0.5),
                    From = Canvas.GetLeft(B1DP)
                };
                doubleAnimation.Completed += DoubleAnimation_Completed1;
                if (b2L.Equals(B2L))
                {
                    doubleAnimation.To = 0;
                }
                else
                {
                    doubleAnimation.To = -178;
                }
                B1DP.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
            }
        }
        private void DoubleAnimation_Completed1(object sender, EventArgs e)
        {
            LabelSettingUsable = true;
        }

        bool LastCompare = true;            //上一个 全局时间 大于 流时间？
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsInitialized)
            {
                if (!Int32.TryParse((sender as TextBox).Text, out _))
                {
                    (sender as TextBox).Text = "";
                }
                else if(A1TB.Text!="" & A2TB.Text!="")
                {
                    double a = Int32.Parse(A1TB.Text), b = Int32.Parse(A2TB.Text);
                    if ((a > 0 & a < 1000) & (b > 0 & b < 1000))
                    {
                        HasChanged = true;
                        Console.WriteLine("HasChanged");

                        if (a >= b)
                        {
                            if (LastCompare)
                            {
                                ExcuteAnimation(b / a - 0.00001);
                            }
                            else
                            {
                                changeDirection = true;
                                LastCompare = true;
                                ExcuteAnimation(0);
                                ExcuteAnimation(b / a - 0.00001);
                            }
                        }
                        else
                        {
                            if (!LastCompare)
                            {
                                ExcuteAnimation(a / b - 0.00001);
                            }
                            else
                            {
                                changeDirection = true;
                                LastCompare = false;
                                ExcuteAnimation(0);
                                ExcuteAnimation(a / b - 0.00001);
                            }
                        }
                    }
                } 
            }
        }

        private void SetDirection(bool v)
        {
            if (v)
            {
                A1ASat.ScaleY = 1;
                path.Stroke = new SolidColorBrush(Color.FromRgb(0, 120, 215));
            }
            else
            {
                A1ASat.ScaleY = -1;
                path.Stroke = new SolidColorBrush(Color.FromRgb(215, 0, 0));
            }
        }

        private void PathDrag(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void TBEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                PathBtnClick(null, null);
        }
    }
}
