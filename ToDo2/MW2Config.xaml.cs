using System;
using System.Collections.Generic;
using System.IO;
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
    /// MW2Config.xaml 的交互逻辑
    /// </summary>
    public partial class MW2Config : Window
    {
        public MW2Config(Window owner)
        {
            InitializeComponent();
            this.Owner = owner;
            this.FontSize = ToDo2Settings.Default.FontSize;
            A1SCB.Opacity = ToDo2Settings.Default.Opacity;
            this.Loaded += MW2Config_Loaded;
        }

        private void MW2Config_Loaded(object sender, RoutedEventArgs e)
        {
            A1S.Value = ToDo2Settings.Default.FontSize;
            if (File.Exists(ToDo2Settings.Default.bg1))
            {
                A1IB.ImageSource = new BitmapImage(new Uri(ToDo2Settings.Default.bg1));
                A1L.Content = "路径：" + ToDo2Settings.Default.bg1;
            }
            if (File.Exists(ToDo2Settings.Default.noticeMusic))
                A2L.Content = "通知提示音：" + ToDo2Settings.Default.noticeMusic;
            if (File.Exists(ToDo2Settings.Default.EndMusic))
                A3L.Content = "背景音乐：" + ToDo2Settings.Default.EndMusic;
        }

        private void SetBg(object sender, MouseButtonEventArgs e)
        {
            string s = ShowFileDialog("图片|*.jpg;*.png");
            if (File.Exists(s))
            {
                A1L.Content = "路径" + s;
                ToDo2Settings.Default.bg1 = s;
            }
        }

        private string ShowFileDialog(string filter)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "文件|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            if (filter != "")
                openFileDialog.Filter = filter;
            var result = openFileDialog.ShowDialog();
            if (result == true)
                return openFileDialog.FileName;
            else
                return null;
        }

        private void SetNoticeMusic(object sender, MouseButtonEventArgs e)
        {
            string s = ShowFileDialog("音乐|*.wav;*.mp3");
            if (File.Exists(s))
            {
                A2L.Content = "通知提示音：" + s;
                ToDo2Settings.Default.noticeMusic = s;
            }
        }

        private void SetBackgroundMusic(object sender, MouseButtonEventArgs e)
        {
            string s = ShowFileDialog("音乐|*.wav;*.mp3;*.flac;*.m4a");
            if (File.Exists(s))
            {
                A2L.Content = "背景音乐：" + s;
                ToDo2Settings.Default.EndMusic = s;
            }
        }

        private void SaveConfiG(object sender, RoutedEventArgs e)
        {
            ToDo2Settings.Default.FontSize = A1S.Value;
            ToDo2Settings.Default.Opacity = A2S.Value;
            ToDo2Settings.Default.Save();
            Close();
        }
    }
}
