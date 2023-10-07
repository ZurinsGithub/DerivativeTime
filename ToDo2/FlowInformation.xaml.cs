using System;
using System.Collections;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ToDo2
{
    /// <summary>
    /// FlowInformation.xaml 的交互逻辑
    /// </summary>
    public partial class FlowInformation : Window
    {
        bool changed = false;
        int time,index;
        string processName;
        public FlowInformation(int time,string name,ArrayList FlowItems,int defaultIndex,bool CreateMode)
        {
            InitializeComponent();
            A1S.Value = time;
            A1TB.Text = name;
            MW3.FlowItem flowItem;
            A1CB.Items.Add(new ComboBoxItem
            {
                Content = "无"
            });
            for(int i = 0; i < FlowItems.Count; i++)
            {
                flowItem = FlowItems[i] as MW3.FlowItem;
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = flowItem.Name + "(" + flowItem.Time + " 分钟)";
                A1CB.Items.Add(comboBoxItem);
            }
            A1CB.SelectedIndex = defaultIndex;
            if (!CreateMode)
                A1SP.Visibility = Visibility.Collapsed;
        }

        public int Time { get => time; set => time = value; }
        public string ProcessName { get => processName; set => processName = value; }
        public int Index { get => index; set => index = value; }
        public bool Changed { get => changed; set => changed = value; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Changed = true;
            Time = (int)A1S.Value;
            ProcessName = A1TB.Text;
            Index = A1CB.SelectedIndex;
            Close();
        }
    }
}
