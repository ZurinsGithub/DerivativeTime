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
using System.IO;

namespace ToDo2
{
    /// <summary>
    /// Window3.xaml 的交互逻辑
    /// </summary>
    public partial class Window3 : Window
    {
        public Window3()
        {
            InitializeComponent();
            ReadConfig();
        }

        private void ReadConfig()
        {
                if (ToDo2Settings.Default.AutoUpdateBackground)
                {
                    A1CB.IsChecked = true;
                }
                if (ToDo2Settings.Default.AutoSavePassKey)
                {
                    A2CB.IsChecked = true;
                }
            A1TB.Text = ToDo2Settings.Default.ip;
            A2TB.Text = ToDo2Settings.Default.port.ToString();
        }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (A1CB.IsChecked == true)
            {
                ToDo2Settings.Default.AutoUpdateBackground = true;
            }
            else
            {
                ToDo2Settings.Default.AutoUpdateBackground = false;
            }
            if (A2CB.IsChecked == true)
            {
                ToDo2Settings.Default.AutoSavePassKey = true;
            }
            else
            {
                ToDo2Settings.Default.AutoSavePassKey = false;
                ToDo2Settings.Default.PassKey = "";
            }
            ToDo2Settings.Default.ip = A1TB.Text;
            if(Int32.Parse(A2TB.Text)>1& Int32.Parse(A2TB.Text)<10000)
                ToDo2Settings.Default.port = Int32.Parse(A2TB.Text);
            //从这里开始改动设置文件

            ToDo2Settings.Default.Save();
            this.Close();
        }
    }
}
