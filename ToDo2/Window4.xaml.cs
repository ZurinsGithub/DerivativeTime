using System;
using System.Collections.Generic;
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


public delegate void DelegateButton();      //在这里定义！！！注意可访问性public
namespace ToDo2
{
    /// <summary>
    /// Window4.xaml 的交互逻辑
    /// </summary>
    public partial class Window4 : Window
    {
        public Window4(string Content,Double WindowWidth)
        {
            InitializeComponent();
            A2L.Content = Content;
            //若不指定宽度则WindowWidth设置为0
            if (WindowWidth!=0)
            {
                DialogWindow.Width = WindowWidth; 
            }
        }

        /// <summary>
        /// 等待程序运行时显示
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="WindowWidth"></param>
        /// <param name="isWaitingRunning">是否可以关闭此窗口</param>
        public Window4(string Content, Double WindowWidth,bool isWaitingRunning)
        {
            InitializeComponent();
            A2L.Content = Content;
            //若不指定宽度则WindowWidth设置为0
            if (WindowWidth != 0)
            {
                DialogWindow.Width = WindowWidth;
            }
            if (isWaitingRunning)
                A1DP.IsEnabled = false;
        }

        //为按钮构造不同的点击方法
        DelegateButton delegateButton;
        public Window4(string Content, Double WindowWidth, DelegateButton delegate1,string ButtonContent)
        {
            delegateButton = delegate1;
            InitializeComponent();
            if (WindowWidth != 0)
            {
                DialogWindow.Width = WindowWidth;
            }
            Button A1B = new Button();
            A2L.Content = Content;
            A1B.Content = ButtonContent;
            A1B.Margin = new Thickness(15);
            A1B.Height = 30;
            A1B.Click += A1B_Click;
            A1SP.Children.Add(A1B);
        }

        private void A1B_Click(object sender, RoutedEventArgs e)
        {
            delegateButton();
            this.Close();
        }
        //↑

        private void A1DPMouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = A1DP.Opacity;
            doubleAnimation.To = 1;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.3);
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            A1DP.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void A1DPMouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = A1DP.Opacity;
            doubleAnimation.To = 0;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.3);
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            A1DP.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void A1DPMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
