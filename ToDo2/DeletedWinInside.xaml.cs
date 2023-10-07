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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ToDo2
{
    /// <summary>
    /// WinInside.xaml 的交互逻辑
    /// </summary>
    public partial class DeletedWinInside : Window
    {
        public DeletedWinInside()
        {
            InitializeComponent();
            InitElement((float)A1W.Width,(float)A1W.Height);
        }
        public DeletedWinInside(float Width,float Height)
        {
            InitializeComponent();
            A1W.Height = Height;
            A1W.Width = Width;
            InitElement((float)A1W.Width, (float)A1W.Height);
        }
        private void InitElement(float Width, float Height)
        {
            A1G.Width = Width - (2 * A1G.Margin.Left);
            A1G.Height = Height - (2 * A1G.Margin.Top);
            A1L.Width = A1SP.Width / 2;
            A2L.Width = A1SP.Width / 2;
            A1L.Height = A1SP.Height;
            A2L.Height = A1SP.Height;
            A1C.Margin = new Thickness(A1L.Width, 0, 0, 0);
            A1T.Width = A1G.Width;
            A0C.Height = A1G.Height;
            A1T.Height = A0C.Height;
        }
    }
}
