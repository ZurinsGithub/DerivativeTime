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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.IO;
using System.Windows.Controls.Primitives;
using System.Threading;

namespace ToDo2
{
    /// <summary>
    /// MW3.xaml 的交互逻辑
    /// </summary>
    public partial class MW3 : Window
    {
        public delegate void MW3ReturnEvent();
        MW3ReturnEvent loading = null;      //当MW3加载完成后执行
        MW3ReturnEvent closing = null;      //当MW3关闭时执行
        public bool IsShowingMW3 { get;private set; } = false;         //来自VS建议
        public static string RootPath;

        /// <summary>
        /// 测试用构造函数
        /// </summary>
        public MW3()
        {
            InitializeComponent();
            Window1 window1 = new Window1();
            window1.ShowDialog();
            this.Loaded += MW3_Loaded;
            this.Closing += MW3_Closing;
            RootPath = "./users/local/";
        }

        public MW3(Window OwnerWindow,MW3ReturnEvent loading, MW3ReturnEvent closing,string flowItemsRootPath)
        {
            InitializeComponent();
            RootPath = flowItemsRootPath;
            this.Owner = OwnerWindow;
            this.loading = loading;
            this.closing = closing;
            this.Loaded += MW3_Loaded;
            this.Closing += MW3_Closing;
        }

        private void MW3_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closing?.Invoke();
            IsShowingMW3 = false;
        }

        private void MW3_Loaded(object sender, RoutedEventArgs e)
        {
            //防止多开
            new ToDo2WindowHasStarted(this, new ProcessOpen(ShowWindow));

            //简化委托调用
            //if (loading != null)
            //    loading();
            loading?.Invoke();
            IsShowingMW3 = true;

            //初始化外部调用事件
            MW3Event.mw3Event += MW3Event_mw3Event;

            //初始化面板
            InitItemParameter();

            FreshStyle();
        }

        private void ShowWindow(bool v)
        {
            this.Show();
        }

        public static Point MousePosition = new Point();        //公共资源：实时鼠标位置
        static AppItem operatingAppItem;
        private void MW3Event_mw3Event(object sender, MW3Args e)
        {
            if (e.str == "A3C.RemoveAt")
            {
                A3C.Children.RemoveAt(int.Parse(e.parameter));
            }
            else if (e.str == "itemArr.RemoveAt and Refresh")
            {
                int index = int.Parse(e.parameter);
                itemArr.RemoveAt(index);
                for (int i = index; i < itemArr.Count; i++)
                {
                    (itemArr[i] as AppItem).Fresh(i);
                }
            }
            else if (e.str == "Refresh A3C.Height")
            {
                A3C.Height = (itemArr.Count / itemColumnCount + 1) * itemWidth;
            }
            else if (e.str == "A4C.RemoveAt")
            {
                A4C.Children.RemoveAt(int.Parse(e.parameter));
            }
            else if (e.str == "FreshA4CInformation")
            {
                FreshA4CInformation();
            }
            else if (e.str == "AddToPanel3, ItemType=AppItem")
            {
                AppItem appItem = sender as AppItem;
                AddToPanel3(MassiveItem.ItemType.AppItem, appItem.path, null);
            }
            else if (e.str == "AddToPanel3, ItemType=FlowItem")
            {
                FlowItem flowItem = sender as FlowItem;
                AddToPanel3(MassiveItem.ItemType.FlowItem, flowItem.Path, flowItem);
            }
            else if (e.str == "AddToPanel1")
            {
                AddToPanel1((sender as MassiveItem).path, int.Parse(e.parameter));
            }
            else if (e.str == "Popup")
            {
                operatingAppItem = sender as AppItem;
                ShowA1P(double.Parse(e.parameter));
            }
            else if (e.str == "A5C.RemoveAt")
            {
                A5C.Children.RemoveAt(int.Parse(e.parameter));
            }
            else if (e.str == "AddToPanel2")
            {
                ReturnToPanel2(sender);
            }
            else if (e.str == "Close")
                Close();
        }

        private void DoubleAnimation2_Completed(object sender, EventArgs e)
        {
            A1Img.Visibility = Visibility.Collapsed;
        }

        private void FreshStyle()           //窗口大小发生改变时发生
        {
            B1L.Width = B1C.ActualWidth / 3 * 2;
            B2L.Width = B1C.ActualWidth / 6;
            B3L.Width = B1C.ActualWidth / 6;
            itemColumnCount = (short)(ActualWidth / 100);
            itemWidth = ActualWidth / itemColumnCount;
            for (short i = 0; i < itemArr.Count; i++)
                (itemArr[i] as AppItem).Fresh(i);
            FreshA4CInformation();
            FreshA5CInformation();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void miniBtn(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            Close();
        }

        Canvas SelectingMenuCanvas = null;       //注意是面板不是按钮
        object SelectingMenuBtn = null;

        private void MenuBtnClick(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Content.ToString();
            if (SelectingMenuCanvas == null)
            {
                try
                {
                    B1C.Visibility = Visibility.Collapsed;
                }
                catch (Exception) { }
            }
            else
                SelectingMenuCanvas.Visibility = Visibility.Collapsed;

            if (SelectingMenuBtn == null)
            {
                SelectingMenuBtn = A1B;
            }

            ReplaceMenuBtnAltitude(SelectingMenuBtn,sender);

            if (content == "流")
            {
                SelectingMenuCanvas = B1C;
            }
            else if (content == "设置")
            {
                SelectingMenuCanvas = B2C;
            }
            else if (content == "扩展")
            {
                SelectingMenuCanvas = B3C;
            }
            else if (content == "账户与社区")
            {
                SelectingMenuCanvas = B4C;
            }

            SelectingMenuCanvas.Visibility = Visibility.Visible;
            SelectingMenuBtn = sender;
        }

        private void ReplaceMenuBtnAltitude(object b1,object b2)
        {
            Button b1s = (b1 as Button), b2s = (b2 as Button);
            b1s.IsEnabled = true;
            b2s.IsEnabled = false;
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            brush.Opacity = 0.2;
            b1s.Background = brush;
            b2s.Background = Brushes.Transparent;
        }

        private void ShowA1P(double width)
        {
            A1P.IsOpen = true;
            A1P.Width = width;
        }

        
        /// <summary>
        /// 初始化面板
        /// </summary>
        public void InitItemParameter()
        {
            //面板1
            //ItemWidth可选范围 90-110
            itemColumnCount = (short)(ActualWidth / 100);
            itemWidth = ActualWidth / itemColumnCount;
            InitPanel1();
            //面板3
            FreshA4CInformation();
            //面板2
            InitPanel2(RootPath);
        }
        #region 面板1
        private void Line1Animation(bool right)
        {
            Point p, p1;
            if (right)
            {
                p = new Point(600, 0);
                p1 = new Point(650, 100);
            }
            else
            {
                p = new Point(150, 0);
                p1 = new Point(200, 100);
            }
            A1LS.BeginAnimation(LineSegment.PointProperty, new PointAnimation
            {
                From = A1LS.Point,
                To = p,
                Duration = TimeSpan.FromSeconds(0.2)
            });
            A2LS.BeginAnimation(LineSegment.PointProperty, new PointAnimation
            {
                From = A2LS.Point,
                To = p1,
                Duration = TimeSpan.FromSeconds(0.2)
            });
        }
        private void Line2Animation(bool right)
        {
            Point p, p1;
            if (right)
            {
                p = new Point(750, 0);
                p1 = new Point(800, 100);
            }
            else
            {
                p = new Point(300, 0);
                p1 = new Point(350, 100);
            }
            A3LS.BeginAnimation(LineSegment.PointProperty, new PointAnimation
            {
                From = A3LS.Point,
                To = p,
                Duration = TimeSpan.FromSeconds(0.2)
            });
            A4LS.BeginAnimation(LineSegment.PointProperty, new PointAnimation
            {
                From = A4LS.Point,
                To = p1,
                Duration = TimeSpan.FromSeconds(0.2)
            });
        }
        private void LabelAnimation1(Label label, bool Expand)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation
            {
                From = label.ActualWidth,
                Duration = TimeSpan.FromSeconds(0.2)
            };
            if (Expand & label.ActualWidth != B1C.ActualWidth / 3 * 2)
            {
                doubleAnimation.To = B1C.ActualWidth / 3 * 2;
                label.BeginAnimation(WidthProperty, doubleAnimation);
            }
            else if (!Expand & label.ActualWidth != B1C.ActualWidth / 6)
            {
                doubleAnimation.To = B1C.ActualWidth / 6;
                label.BeginAnimation(WidthProperty, doubleAnimation);
            }
        }
        private void LabelAnimation2(SolidColorBrush solidColorBrush,bool Selected)
        {
            if (solidColorBrush.Opacity != 0.25 & !Selected)
                solidColorBrush.BeginAnimation(Brush.OpacityProperty, new DoubleAnimation
                {
                    From = 0,
                    To = 0.25,
                    Duration = TimeSpan.FromSeconds(0.2)
                });
            else if (solidColorBrush.Opacity != 0 & Selected)
                solidColorBrush.BeginAnimation(Brush.OpacityProperty, new DoubleAnimation
                {
                    To = 0,
                    From = 0.25,
                    Duration = TimeSpan.FromSeconds(0.2)
                });
        }
        private void LabelAnimation(bool B1Lr, bool B2Lr, bool B3Lr)
        {
            LabelAnimation1(B1L, B1Lr);
            LabelAnimation2(B1SC, B1Lr);
            LabelAnimation1(B2L, B2Lr);
            LabelAnimation2(B2SC, B2Lr);
            LabelAnimation1(B3L, B3Lr);
            LabelAnimation2(B3SC, B3Lr);
        }
        private void B1LClick(object sender, MouseButtonEventArgs e)
        {
            if (A1LS.Point.X != 600)
                Line1Animation(true);
            if (A3LS.Point.X != 750)
                Line2Animation(true);
            LabelAnimation(true, false, false);
            if (Canvas.GetLeft(A4G) != 0)
                A4G.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation
                {
                    From = Canvas.GetLeft(A4G),
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3)
                });
        }
        private void B2LClick(object sender, MouseButtonEventArgs e)
        {
            if (A1LS.Point.X != 150)
                Line1Animation(false);
            if (A3LS.Point.X != 750)
                Line2Animation(true);
            LabelAnimation(false,true, false);
            if (Canvas.GetLeft(A4G) != -A2C.ActualWidth)
                A4G.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation
                {
                    From = Canvas.GetLeft(A4G),
                    To = -A2C.ActualWidth,
                    Duration = TimeSpan.FromSeconds(0.3)
                });
        }
        private void B3LClick(object sender, MouseButtonEventArgs e)
        {
            if (A1LS.Point.X != 150)
                Line1Animation(false);
            if (A3LS.Point.X != 300)
                Line2Animation(false);
            LabelAnimation(false, false, true);
            if (Canvas.GetLeft(A4G) != -A2C.ActualWidth * 2)
                A4G.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation
                {
                    From = Canvas.GetLeft(A4G),
                    To = -A2C.ActualWidth * 2,
                    Duration = TimeSpan.FromSeconds(0.3)
                });
        }

        public static short itemColumnCount;
        public static double itemWidth;
        public static ArrayList itemArr = new ArrayList();

        public class AppItem        //Panel1 的项
        {
            public int index;
            public string path;
            public Point point = new Point();
            public StackPanel stackPanel = new StackPanel();

            public AppItem(int index)
            {
                this.index = index;
                point.X = itemWidth * (index % itemColumnCount);
                point.Y = ((int)(index / itemColumnCount)) * itemWidth;

                Canvas.SetLeft(stackPanel, point.X);
                Canvas.SetTop(stackPanel, point.Y);
                stackPanel.Margin = new Thickness(8);
                stackPanel.Width = MW3.itemWidth - 16;
                stackPanel.Height = stackPanel.Width;
                stackPanel.ClipToBounds = true;
                stackPanel.Background = new SolidColorBrush
                {
                    Color = Colors.White,
                    Opacity = 0
                };
                stackPanel.MouseEnter += StackPanel_MouseEnter;
                stackPanel.MouseLeave += StackPanel_MouseLeave;
                stackPanel.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("./IMG/Add.png"))),
                    Stretch = Stretch.UniformToFill,
                    Width=stackPanel.Width/3,
                    Height=stackPanel.Height/3,
                    Margin=new Thickness(0,stackPanel.Width/3,0,0)
                });
            }

            public AppItem(int index, string path, short itemColumnCount, double itemWidth)
            {
                this.index = index;
                this.path = path;

                point.X = itemWidth * (index % itemColumnCount);
                point.Y = ((int)(index / itemColumnCount)) * itemWidth;

                Canvas.SetLeft(stackPanel, point.X);
                Canvas.SetTop(stackPanel, point.Y);
                stackPanel.Margin = new Thickness(8);
                stackPanel.Width = MW3.itemWidth - 16;
                stackPanel.Height = stackPanel.Width;
                stackPanel.ClipToBounds = true;
                stackPanel.Background= new SolidColorBrush
                {
                    Color = Colors.White,
                    Opacity=0
                };

                stackPanel.Children.Add(new Image
                {
                    Width = stackPanel.Width - 24,
                    Height = stackPanel.Width - 24,
                    Source = GetIcon.c_icon_of_path.icon_of_path_large(path, true, true),     //使用了资源
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(0, 4, 0, 0)
                }) ;

                stackPanel.Children.Add(new Label
                {
                    Content = System.IO.Path.GetFileNameWithoutExtension(path),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.White)
                }) ;
                stackPanel.MouseLeftButtonDown += StackPanel_MouseLeftButtonDown;
                stackPanel.MouseRightButtonDown += StackPanel_MouseRightButtonDown;
                stackPanel.MouseEnter += StackPanel_MouseEnter;
                stackPanel.MouseLeave += StackPanel_MouseLeave;
            }

            private void StackPanel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
            {
                MW3Event.Exc("Popup", (stackPanel.Width * 1.5).ToString(), this);
            }

            private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
            {
                stackPanel.Background.Opacity = 0;
            }

            private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
            {
                stackPanel.Background.Opacity = 0.25;
            }

            private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                MW3Event.Exc("AddToPanel3, ItemType=AppItem", "", this);
                RemoveItem(index);
            }

            public void RemoveItem(int index)
            {
                MW3Event.Exc("A3C.RemoveAt", index.ToString() ,this);
                MW3Event.Exc("itemArr.RemoveAt and Refresh", index.ToString(), this);
                MW3Event.Exc("Refresh A3C.Height", "", null);
            }

            public void Fresh(int index)
            {
                this.index = index;
                point.X = itemWidth * (index % itemColumnCount);
                point.Y = ((int)(index / itemColumnCount)) * itemWidth;

                this.stackPanel.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation
                {
                    From = Canvas.GetLeft(stackPanel),
                    To = point.X,
                    Duration = TimeSpan.FromSeconds(0.2)
                });
                this.stackPanel.BeginAnimation(Canvas.TopProperty, new DoubleAnimation
                {
                    From = Canvas.GetTop(stackPanel),
                    To = point.Y,
                    Duration = TimeSpan.FromSeconds(0.2)
                });
            }
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(A2C);
            Image image = (sender as StackPanel).Children[0] as Image;
            A1Img.Source = image.Source;
            A1Img.Width = image.Width;
            A1Img.Visibility = Visibility.Visible;
            DoubleAnimation doubleAnimation = new DoubleAnimation
            {
                From = p.X,
                To = A2C.ActualWidth,
                Duration = TimeSpan.FromSeconds(0.4)
            };
            DoubleAnimation doubleAnimation1 = new DoubleAnimation
            {
                From = p.Y,
                To = A2C.ActualHeight,
                Duration = TimeSpan.FromSeconds(0.4)
            };
            DoubleAnimation doubleAnimation2 = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.4)
            };
            doubleAnimation2.Completed += DoubleAnimation2_Completed;
            A1Img.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
            A1Img.BeginAnimation(Canvas.TopProperty, doubleAnimation1);
            A1Img.BeginAnimation(OpacityProperty, doubleAnimation2);
            //注意画笔的依赖属性
        }
        
        private void AddToPanel1(string path)
        {
            AppItem appItem = new AppItem(itemArr.Count, path, itemColumnCount, itemWidth);
            itemArr.Add(appItem);
            appItem.stackPanel.MouseLeftButtonDown += StackPanel_MouseLeftButtonDown;
            A3C.Children.Add(appItem.stackPanel);
            MW3Event.Exc("Refresh A3C.Height", "", null);
        }
        //插入到 Panel1 指定位置
        private void AddToPanel1(string path,int index)
        {
            AppItem appItem = new AppItem(itemArr.Count, path, itemColumnCount, itemWidth);
            itemArr.Insert(index, appItem);
            appItem.stackPanel.MouseLeftButtonDown += StackPanel_MouseLeftButtonDown;
            A3C.Children.Insert(index, appItem.stackPanel);
            MW3Event.Exc("Refresh A3C.Height", "", null);
            for (int i = index; i < itemArr.Count; i++)
                (itemArr[i] as AppItem).Fresh(i);
        }
        //添加“+”号
        private void AddToPanel1()
        {
            AppItem appItem = new AppItem(itemArr.Count);
            appItem.stackPanel.MouseLeftButtonDown += StackPanel_MouseLeftButtonDown1;
            itemArr.Add(appItem);
            A3C.Children.Add(appItem.stackPanel);
            MW3Event.Exc("Refresh A3C.Height", "", null);
        }

        private void StackPanel_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "文件|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = true
            };
            var result = dialog.ShowDialog();
            string s = "";
            if (result == true)
            {
                for(short i = 0; i < dialog.FileNames.Length; i++)
                {
                    s += dialog.FileNames[i] + ">";
                    AddToPanel1(dialog.FileNames[i], itemArr.Count - 1);
                    (itemArr[itemArr.Count - 1] as AppItem).Fresh(itemArr.Count - 1);
                }
                File.AppendAllText("./mw3/Run/AppItem.txt",s);
                AppPaths = File.ReadAllText("./mw3/Run/AppItem.txt").Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        string[] AppPaths = null;
        private void InitPanel1()
        {
            AppPaths = File.ReadAllText("./mw3/Run/AppItem.txt").Split(new char[] { '>' },StringSplitOptions.RemoveEmptyEntries);
            for(short i = 0; i < AppPaths.Length; i++)
            {
                if (File.Exists(AppPaths[i]))
                {
                    AddToPanel1(AppPaths[i]); 
                }
            }
            AddToPanel1();
        }
        //面板1代码结束
#endregion
        #region 面板3
        private void AddToPanel3(MassiveItem.ItemType itemType,string path,object sender)
        {
            B3SC.BeginAnimation(Brush.OpacityProperty, new DoubleAnimation
            {
                From = 0.8,
                To = 0.25,
                Duration = TimeSpan.FromSeconds(1)
            });

            MassiveItem massiveItem;
            if (itemType == MassiveItem.ItemType.AppItem)
                massiveItem = new MassiveItem(itemArr2.Count, itemType, path, System.IO.Path.GetFileNameWithoutExtension(path));
            else
            {
                FlowItem flowItem = sender as FlowItem;
                massiveItem = new MassiveItem(itemArr2.Count, MassiveItem.ItemType.FlowItem, flowItem.Path, flowItem.Name);
                massiveItem.owner = flowItem;
            }
            itemArr2.Add(massiveItem);
            A4C.Children.Add(massiveItem.dockPanel);
            FreshA4CInformation();
        }
        public static ArrayList itemArr2 = new ArrayList();
        public static double A4CWidth;
        public static double A4CHeight;
        private void FreshA4CInformation()
        {
            A4CWidth = A4C.ActualWidth;
            A4CHeight = MassiveItem.ItemHeight * itemArr2.Count;
            A4C.Height = A4CHeight;
        }
        public class MassiveItem
        {
            public object owner;
            public static double ItemHeight = 50;
            public int index;
            public DockPanel dockPanel = new DockPanel();
            public string path;
            public ItemType itemType;
            public string name;
            public enum ItemType
            {
                AppItem = 0,
                FlowItem = 1
            }

            public MassiveItem(int index,ItemType itemType,string path,string name)
            {
                this.index = index;
                this.itemType = itemType;
                this.path = path;
                this.name = name;

                dockPanel.Width = A4CWidth;
                dockPanel.Height = ItemHeight - 2;
                dockPanel.Background = new SolidColorBrush
                {
                    Color = Colors.White,
                    Opacity = 0.4
                };
                Canvas.SetTop(dockPanel, index * (dockPanel.Height + 2));
                Canvas.SetLeft(dockPanel, 0);
                dockPanel.FlowDirection = FlowDirection.RightToLeft;        //从右向左
                Label label = new Label
                {
                    Content = "删除",
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Colors.White),
                    Width = 10,
                    Height = dockPanel.Height,
                    Background = new SolidColorBrush(Color.FromRgb(199, 199, 199))
                };
                label.MouseEnter += Label_MouseEnter;
                label.MouseLeave += Label_MouseLeave;
                label.MouseLeftButtonDown += Label_MouseLeftButtonDown;
                dockPanel.Children.Add(label);
                Label label1 = new Label
                {
                    Width = A4CWidth / 2,
                    FontSize = 15,
                    Height = dockPanel.Height,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                };
                if (itemType == ItemType.AppItem)
                    label1.Content = "应用或文件";
                else
                    label1.Content = "流";
                dockPanel.Children.Add(label1);
                dockPanel.Children.Add(new Label
                {
                    Width = A4CWidth / 3 - 50,
                    FontSize = 15,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Height =dockPanel.Height,
                    Content=name
                });
                if (itemType==ItemType.AppItem)
                {
                    dockPanel.Children.Add(new Image
                    {
                        Margin = new Thickness(5),
                        Width = 40,
                        Source = GetIcon.c_icon_of_path.icon_of_path_large(path, false, true),
                        Stretch = Stretch.UniformToFill
                    }); 
                }
                else
                {
                    dockPanel.Children.Add(new Image
                    {
                        Margin = new Thickness(5),
                        Width = 38,
                        Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("./IMG/LOGOtodo.png"))),
                        Stretch = Stretch.Fill
                    });
                }
            }
            private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                if (itemType==ItemType.AppItem)
                {
                    MW3Event.Exc("AddToPanel1", (itemArr.Count - 1).ToString(), this); 
                }
                else
                {
                    MW3Event.Exc("AddToPanel2", "", owner);
                }
                RemoveMassItem();
            }
            private void Label_MouseLeave(object sender, MouseEventArgs e)
            {
                (sender as Label).BeginAnimation(WidthProperty, new DoubleAnimation
                {
                    From = (sender as Label).Width,
                    To = 10,
                    Duration = TimeSpan.FromSeconds(0.2)
                });
                (sender as Label).Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
                {
                    From = Colors.Red,
                    To = Color.FromRgb(199,199,199),
                    Duration = TimeSpan.FromSeconds(0.2)
                });
            }
            private void Label_MouseEnter(object sender, MouseEventArgs e)
            {
                (sender as Label).BeginAnimation(WidthProperty, new DoubleAnimation
                {
                    From = (sender as Label).Width,
                    To = 70,
                    Duration = TimeSpan.FromSeconds(0.2)
                });
                (sender as Label).Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
                {
                    To = Colors.Red,
                    From = Color.FromRgb(199, 199, 199),
                    Duration = TimeSpan.FromSeconds(0.2)
                });
            }
        
            private void RemoveMassItem()
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation
                {
                    From = Canvas.GetLeft(dockPanel),
                    To = 2 * dockPanel.ActualWidth,
                    Duration = TimeSpan.FromSeconds(0.2)
                };
                doubleAnimation.Completed += DoubleAnimation_Completed;
                dockPanel.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
            }
            private void DoubleAnimation_Completed(object sender, EventArgs e)
            { 
                MW3Event.Exc("A4C.RemoveAt", index.ToString(), this);
                itemArr2.RemoveAt(index);
                Refresh();
            }

            private void Refresh()
            {
                for(int i = index; i < itemArr2.Count; i++)
                {
                    (itemArr2[i] as MassiveItem).index = i;
                    DockPanel dockPanel = (itemArr2[i] as MassiveItem).dockPanel;
                    dockPanel.BeginAnimation(Canvas.TopProperty, new DoubleAnimation
                    {
                        From = Canvas.GetTop(dockPanel),
                        To = (itemArr2[i] as MassiveItem).index * (dockPanel.Height + 2),
                        Duration = TimeSpan.FromSeconds(0.2)
                    });
                }
                MW3Event.Exc("FreshA4CInformation", "", null);
            }
        }

        private void RightPanelME(object sender, MouseEventArgs e)
        {
            SolidColorBrush solidColorBrush = (sender as DockPanel).Background as SolidColorBrush;
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
            {
                From = solidColorBrush.Color,
                To = Colors.White,
                Duration = TimeSpan.FromSeconds(0.2)
            });
        }
        private void RightPanelML(object sender, MouseEventArgs e)
        {
            SolidColorBrush solidColorBrush = (sender as DockPanel).Background as SolidColorBrush;
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
            {
                From = solidColorBrush.Color,
                To = Colors.Black,
                Duration = TimeSpan.FromSeconds(0.4)
            });
        }

        private void A1PBtn(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Content.ToString() == "删除")
            {
                Console.WriteLine(operatingAppItem.path);
                RemoveFromPanel1(operatingAppItem);
            }
            A1P.IsOpen = false;
        }

        //删除面板1中的项并更新运行库
        private void RemoveFromPanel1(AppItem appItem)
        {
            appItem.RemoveItem(appItem.index);
            string s = "";
            for(short i = 0; i < AppPaths.Length; i++)
            {
                if (AppPaths[i] != appItem.path)
                    s += AppPaths[i] + ">";
            }
            File.WriteAllText("./mw3/Run/AppItem.txt", s);
            AppPaths = s.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion
        #region 面板2

        static string ProcessListPath = "./users/local/ProcessList.txt";    //用于指定哪个ProcessList的路径

        public static double A5CWidth;
        private void FreshA5CInformation()
        {
            A5CWidth = A5C.ActualWidth;
            A5C.Height = flowItemArr.Count * FlowItem.panelHeight;
        }

        public static ArrayList flowItemArr = new ArrayList();
        public class FlowItem
        {
            public static double panelHeight = 50;
            DockPanel dock = new DockPanel();
            string path, name, structPath;
            int index,time,existIndex;          //time:单位：分
            Label label;
            bool show = true;

            public string Path { get => path; set => path = value; }
            public string Name { get => name; set => name = value; }
            public int Index { get => index; set => index = value; }
            public DockPanel Dock { get => dock; set => dock = value; }
            public string StructPath { get => structPath; set => structPath = value; }
            public int Time { get => time; set => time = value; }
            public int ExistIndex { get => existIndex; set => existIndex = value; }
            public bool Show { get => show; set => show = value; }

            public FlowItem(string path, string name, int index, string structPath, int time,int existIndex)
            {
                this.path = path;
                this.name = name;
                this.index = index;
                this.structPath = structPath;
                this.time = time;
                this.existIndex = existIndex;

                dock.Height = panelHeight - 2;
                dock.Width = A5CWidth;
                dock.Background = new SolidColorBrush
                {
                    Color = Colors.White,
                    Opacity = 0.4
                };
                Canvas.SetTop(dock, index * (dock.Height + 2));
                Canvas.SetLeft(dock, 0);
                dock.FlowDirection = FlowDirection.RightToLeft;

                label = new Label
                {
                    Content = "删除",
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Colors.White),
                    Width = 70,
                    Height = dock.Height,
                    Background = new SolidColorBrush(Color.FromRgb(199, 199, 199))
                };

                Label label1 = new Label
                {
                    Content = "修改",
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    FontSize = 12,
                    Width = label.Width,
                    Height = dock.Height,
                    Background = new SolidColorBrush(Colors.LightGray)
                };
                DockPanel dockPanel = new DockPanel
                {
                    Width = 10
                };
                dockPanel.Children.Add(label);
                dockPanel.Children.Add(label1);
                dockPanel.MouseEnter += DockPanel_MouseEnter;
                dockPanel.MouseLeave += DockPanel_MouseLeave;
                label.MouseLeftButtonDown += Label_MouseLeftButtonDown;
                label1.MouseLeftButtonDown += Label1_MouseLeftButtonDown;
                dock.Children.Add(dockPanel);
                dock.Children.Add(new Label
                {
                    Width = dock.Width / 2,
                    FontSize = 15,
                    Height = dock.Height,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Content = name,
                    FlowDirection = FlowDirection.LeftToRight
                });
                dock.Children.Add(new Label
                {
                    Width = dock.Width / 2,
                    FontSize = 15,
                    Height = dock.Height,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Content = structPath,
                    FlowDirection=FlowDirection.LeftToRight
                });
                dock.MouseLeftButtonDown += Dock_MouseLeftButtonDown;
            }

            private void Dock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                dock.BeginAnimation(Canvas.TopProperty, new DoubleAnimation
                {
                    From = Canvas.GetTop(dock),
                    To = panelHeight * flowItemArr.Count * 2,
                    Duration = TimeSpan.FromSeconds(0.2)
                });
                DoubleAnimation doubleAnimation= new DoubleAnimation
                {
                    From = Canvas.GetLeft(dock),
                    To = dock.ActualWidth * 3,
                    Duration = TimeSpan.FromSeconds(0.2)
                };
                doubleAnimation.Completed += DoubleAnimation_Completed1;
                dock.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
            }
            private void DoubleAnimation_Completed1(object sender, EventArgs e)
            {
                MW3Event.Exc("AddToPanel3, ItemType=FlowItem", "", this);
                this.Show = false;
                this.Dock.Visibility = Visibility.Collapsed;
                RefreshIndexs(0);
                Fresh(0);
            }
            private void DockPanel_MouseEnter(object sender, MouseEventArgs e)
            {
                (sender as DockPanel).BeginAnimation(WidthProperty, new DoubleAnimation
                {
                    From = (sender as DockPanel).Width,
                    To = label.Width*2,
                    Duration = TimeSpan.FromSeconds(0.2)
                });
                label.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
                {
                    To = Colors.Red,
                    From = Color.FromRgb(199, 199, 199),
                    Duration = TimeSpan.FromSeconds(0.2)
                });
            }
            private void DockPanel_MouseLeave(object sender, MouseEventArgs e)
            {
                (sender as DockPanel).BeginAnimation(WidthProperty, new DoubleAnimation
                {
                    From = (sender as DockPanel).Width,
                    To = 10,
                    Duration = TimeSpan.FromSeconds(0.2)
                });
                label.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
                {
                    From = Colors.Red,
                    To = Color.FromRgb(199, 199, 199),
                    Duration = TimeSpan.FromSeconds(0.2)
                });
            }
            private void Label1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
                FlowInformation flowInformation = new FlowInformation(time, name, flowItemArr, ExistIndex,false);
                flowInformation.ShowDialog();
                if (flowInformation.Changed)
                {
                    if (flowInformation.ProcessName != "")
                        name = flowInformation.ProcessName;
                    time = flowInformation.Time * 60;
                    (dock.Children[1] as Label).Content = name;
                }

            }
            private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
                if (ToDoFileProcess.DeleteFlow(path))
                {
                    DeleteItem(true);
                }
            }

            private static string DeletingPath;
            private void DeleteItem(bool v)
            {
                if (v)
                    DeletingPath = path;
                DoubleAnimation d = new DoubleAnimation
                {
                    From = Canvas.GetLeft(dock),
                    To = dock.ActualWidth * 2,
                    Duration = TimeSpan.FromSeconds(0.2)
                };
                d.Completed += D_Completed;
                d.Completed += D_Completed1;
                dock.BeginAnimation(Canvas.LeftProperty, d);
            }
            private void D_Completed1(object sender, EventArgs e)
            {
                if (existIndex<flowItemArr.Count)
                {
                    if ((flowItemArr[existIndex] as FlowItem).path.StartsWith(DeletingPath))
                    {
                        (flowItemArr[existIndex] as FlowItem).DeleteItem(false);
                    }
                    else
                    {
                        Fresh(0);
                        SaveToProcessList();
                    }
                }
                else
                {
                    Fresh(0);
                    SaveToProcessList();
                }
            }
            private void D_Completed(object sender, EventArgs e)
            {
                flowItemArr.RemoveAt(existIndex);
                MW3Event.Exc("A5C.RemoveAt", index.ToString(), null);
                RefreshIndexs(0);
            }

            public static void RefreshIndexs(int start)
            {
                int j = start;
                for(int i = start; i < flowItemArr.Count; i++)
                {
                    (flowItemArr[i] as FlowItem).ExistIndex = i;
                    if((flowItemArr[i] as FlowItem).show)
                    {
                        (flowItemArr[i] as FlowItem).index = j;
                        j++;
                    }
                }
            }
            
            public static void Fresh(int start)
            {
                for(int i = start; i< flowItemArr.Count; i++)
                {
                    if((flowItemArr[i] as FlowItem).show)
                    {
                        (flowItemArr[i] as FlowItem).dock.Visibility = Visibility.Visible;
                        (flowItemArr[i] as FlowItem).dock.BeginAnimation(Canvas.TopProperty, new DoubleAnimation
                        {
                            From = Canvas.GetTop((flowItemArr[i] as FlowItem).dock),
                            To = (flowItemArr[i] as FlowItem).index * panelHeight,
                            Duration = TimeSpan.FromSeconds(0.2)
                        });
                    }
                    else
                    {
                        (flowItemArr[i] as FlowItem).dock.Visibility = Visibility.Collapsed;
                    }
                }
            }
            
            private static void RemoveItem(int existIndex, int index)
            {
                flowItemArr.RemoveAt(existIndex);
                MW3Event.Exc("A5C.RemoveAt", index.ToString(), null);
            }

            public static void SaveToProcessList()
            {
                string s = "";
                for (int i = 0; i < flowItemArr.Count; i++)
                {
                    s += (flowItemArr[i] as FlowItem).StructPath + ";";
                }
                s = s.Replace('.', 'v');
                File.WriteAllText(ProcessListPath, s);
            }

            public void test()
            {
                Console.WriteLine(structPath + " " + path + " " + index + " " + existIndex);
                Console.WriteLine(Canvas.GetLeft(dock)  + " - " + Canvas.GetTop(dock));
            }
        }

        public void AddToPanel2(string path,string name,int index,string structPath, int time, int existIndex)
        {
            FlowItem flowItem = new FlowItem(path, name, index, structPath, time, existIndex);
            flowItemArr.Insert(existIndex,flowItem);
            A5C.Children.Insert(existIndex, flowItem.Dock);
            FreshA5CInformation();
        }

        private void ReturnToPanel2(object owner)
        {
            FlowItem flowItem = owner as FlowItem;
            for(int i = 0; i < flowItemArr.Count; i++)
            {
                if(flowItem.StructPath==(flowItemArr[i] as FlowItem).StructPath)
                {
                    (flowItemArr[i] as FlowItem).Show = true;
                    (flowItemArr[i] as FlowItem).Dock.Visibility = Visibility.Visible;
                    DoubleAnimation doubleAnimation = new DoubleAnimation
                    {
                        To = 0, Duration = TimeSpan.FromSeconds(0.01)
                    };
                    (flowItemArr[i] as FlowItem).Dock.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
                    FlowItem.RefreshIndexs(0);
                    FlowItem.Fresh(0);
                    (flowItemArr[i] as FlowItem).test();
                    break;
                }
            }
        }

        //注意不同的路径
        private void InitPanel2(string RootPath)
        {
            FreshA5CInformation();
            //清空
            flowItemArr = new ArrayList();
            for (int i = A5C.Children.Count; i > 0; i--)
                A5C.Children.RemoveAt(0);
            //初始化
            ArrayList AllProcess = FlowStruct.getArr(RootPath + "ProcessList.txt");
            string str, p, s;
            string[] c;
            for(int i = 0; i < AllProcess.Count; i++)
            {
                str = AllProcess[i] as string;
                p = RootPath + str.Replace('v', '/');
                c = File.ReadAllText(p + "/information.txt").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                s = str.Replace('v', '.');
                AddToPanel2(p, c[0], flowItemArr.Count, s, int.Parse(c[1])/60, flowItemArr.Count);
            }
        }

        //添加流（同时在文件中）
        private void AddFlow(object sender, MouseButtonEventArgs e)
        {
            FlowInformation flowInformation = new FlowInformation(40,"",flowItemArr,0,true);
            flowInformation.ShowDialog();
            if (flowInformation.Changed)
            {
                FlowStruct.UpdateProcessList(ProcessListPath);
                string structPath, name;
                int a;
                if (flowInformation.Index == 0)
                {
                    if (flowItemArr.Count > 0)
                    {
                        string s;
                        int i;
                        int j = 1;
                        for (i = 0; i < flowItemArr.Count; i++)
                        {
                            s = (flowItemArr[i] as FlowItem).StructPath;
                            if (FlowStruct.GetIntArray(s, '.')[0] == j)
                            {
                                j++;
                                i = 0;
                            }
                            else if (FlowStruct.GetIntArray(s, '.')[0] > j)
                                break;
                        }
                        a = i;
                        structPath = j.ToString() + "/";
                    }
                    else
                    {
                        a = flowItemArr.Count;
                        structPath = "1/";
                    }
                }
                else
                {
                    ArrayList arrayList = new ArrayList();
                    for(int i = 0; i < flowItemArr.Count; i++)
                    {
                        arrayList.Add(FlowStruct.GetIntArray((flowItemArr[i] as FlowItem).StructPath, '.'));
                    }
                    structPath = FlowStruct.GetString(FlowStruct.GetLarge(arrayList, flowInformation.Index - 1,out int _),"/");
                    a = FlowStruct.GetMax(arrayList, FlowStruct.GetIntArray(structPath, '/')) + 1;
                }
                if (flowInformation.ProcessName == "")
                    name = "流名称";
                else
                    name = flowInformation.ProcessName;

                ToDoFileClass.AddFile(RootPath,structPath, name, flowInformation.Time);

                AddToPanel2(RootPath + structPath, name, -5, structPath.Replace('/', '.'), flowInformation.Time, a);
                FlowItem.RefreshIndexs(0);
                FlowItem.Fresh(0);


                //InitPanel2(RootPath);
                ////删除已经存在Panel3中的流
                //FlowItem flowItem;
                //MassiveItem massiveItem;
                //for (int i = 0; i < itemArr2.Count; i++)
                //{
                //    massiveItem = itemArr2[i] as MassiveItem;
                //    if (massiveItem.itemType==MassiveItem.ItemType.FlowItem)
                //    {
                //        for (int j = 0; j < flowItemArr.Count; j++)
                //        {
                //            flowItem = flowItemArr[j] as FlowItem;
                //            if ((massiveItem.owner as FlowItem).StructPath == flowItem.StructPath)
                //            {
                //                flowItem.Show = false;
                //                break;
                //            }

                //        } 
                //    }
                //}
                //FlowItem.RefreshIndexs(0);
                //FlowItem.Fresh(0);
            }
        }
        #endregion


        #region Panel2LabelMouse
        private void RightPanelME1(object sender, MouseEventArgs e)
        {
            SolidColorBrush solidColorBrush = (sender as Label).Background as SolidColorBrush;
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
            {
                From = solidColorBrush.Color,
                To = Colors.White,
                Duration = TimeSpan.FromSeconds(0.2)
            });
        }

        private void RightPanelML1(object sender, MouseEventArgs e)
        {
            SolidColorBrush solidColorBrush = (sender as Label).Background as SolidColorBrush;
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
            {
                From = solidColorBrush.Color,
                To = Colors.Black,
                Duration = TimeSpan.FromSeconds(0.4)
            });
        }

        private void ReadyRun(object sender, MouseButtonEventArgs e)
        {
            B3LClick(null, null);
        }

        private void RightPanelME2(object sender, MouseEventArgs e)
        {
            SolidColorBrush solidColorBrush = (sender as Label).Background as SolidColorBrush;
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
            {
                From = solidColorBrush.Color,
                To = Color.FromRgb(31,232,0),
                Duration = TimeSpan.FromSeconds(0.2)
            });
        }
#endregion
        #region Run
        private void Run(object sender, MouseButtonEventArgs e)
        {
            Thread thread = new Thread(r1452);
            thread.Start();
        }
        private void r1452()
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate
            {
                RunMain();
            });
        }
        private void RunMain()
        {
            if (itemArr2.Count > 0)
            {
                IsEnabled = false;
                Window4 window = new Window4("正在启动...", 0, true);
                window.Owner = this;
                window.Show();

                string s2 = "";
                MassiveItem massiveItem;
                for (int i = 0; i < itemArr2.Count; i++)
                {
                    massiveItem = itemArr2[i] as MassiveItem;
                    if (massiveItem.itemType == MassiveItem.ItemType.AppItem)
                    {
                        System.Diagnostics.Process.Start(massiveItem.path);
                        Thread.Sleep(1000);
                    }
                    else
                        s2 += massiveItem.path + "\n";
                }

                //MW2
                if (s2!="")
                {
                    int t = 40;
                    if (int.TryParse(A1TB.Text, out int _))
                        t = int.Parse(A1TB.Text);
                    MW2 mW2 = new MW2(s2.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries),t);
                    mW2.Show(); 
                }

                window.Close();

                MW3Event.Exc("Close", "", null);
            }
            else
            {
                Window4 window4 = new Window4("请至少表明一个运行项", 0);
                window4.ShowDialog();
            }
        }
        #endregion

        private void SetStatic(object sender, MouseButtonEventArgs e)
        {
            A1DP.Visibility = Visibility.Visible;
        }
    }
    public class RecWid : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 2 + 170;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ImgHei : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 9;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class PopupIsOpen : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

public class MW3Args : EventArgs
{
    public string str, parameter;

    public MW3Args(string str, string parameter)
    {
        this.str = str;
        this.parameter = parameter;
    }
}
public static class MW3Event
{
    public static event EventHandler<MW3Args> mw3Event;
    public static void Exc(string str,string parameter, object sender)
    {
        var arg = new MW3Args(str ,parameter);
        mw3Event?.Invoke(sender, arg);
    }
}
public static class ToDoFileProcess
{
    public static bool DeleteFlow(string directoryPath)
    {
        if (MessageBox.Show("确定删除该流（及其子流）吗？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            Directory.Delete(directoryPath, true);
            return true;
        }
        else
            return false;
    }
}


