using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToDo2.Properties;

namespace ToDo2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            this.Loaded += MainWindow_ContentRendered;
        }
        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            BeginRunning();
        }

        public Storyboard storyboard;
        private void BeginRunning()
        {
            //"as"是类型转换，Resources代表XAML中定义的资源
            storyboard = Resources["A3story"] as Storyboard;
            storyboard.Completed += Storyboard_Completed;
            storyboard.Begin();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            A1LGB.StartPoint = Point.Parse("0,1");
            A1LGB.EndPoint = Point.Parse("1,0");
            storyboard = Resources["A2story"] as Storyboard;
            storyboard.Completed -= Storyboard_Completed;
            storyboard.Completed += Storyboard_Completed1;
            storyboard.Begin();
        }

        private void Storyboard_Completed1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MLBD(object sender, MouseButtonEventArgs e)
        {
            Storyboard_Completed1(null, null);
        }
    }
}
