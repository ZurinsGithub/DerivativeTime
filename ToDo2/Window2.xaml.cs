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

namespace ToDo2
{
    /// <summary>
    /// Window2.xaml 的交互逻辑
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2(string Title)
        {
            InitializeComponent();
            A1L.Content = Title;
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
    }
}
