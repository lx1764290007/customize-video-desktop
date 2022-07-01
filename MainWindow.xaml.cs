using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Collections;
 

namespace VencentLum
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window

    {
        ArrayList file_paths = new ArrayList();
        UIElement active_border_element;
        Player window_player;

        public MainWindow()
        {
            Loaded += OnLoad;
            InitializeComponent();
        }
        public void OnLoad(object sender, RoutedEventArgs e)
        {

        }
        // 动态加载图片资源
        private void Load_Image()
        {

            Image i = new Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri("./source/224f99e051fda325f914b0132044c4b4.jpg", UriKind.Relative);
            src.EndInit();
            i.Source = src;
            i.Stretch = Stretch.Uniform;
            Grid.SetColumn(i, 0);
            Grid.SetRow(i, 0);
            int q = src.PixelHeight;        // Image loads here
            _ = wrap_pannel.Children.Add(i);
        }

        private void Load_Video(string file_path, int index)
        {
            Border myBorder = new Border();
            myBorder.BorderBrush = Brushes.SlateBlue;
            myBorder.BorderThickness = new Thickness(2);
            myBorder.Background = Brushes.AliceBlue;
            myBorder.Padding = new Thickness(5);
            myBorder.Margin = new Thickness(3);
            myBorder.Width = 240;
            myBorder.CornerRadius = new CornerRadius(1);
            // 绑定右键菜单
            myBorder.ContextMenu = this.Resources["ContextMenu"] as ContextMenu;
            myBorder.MouseEnter += MyBorder_MouseEnter;
            myBorder.MouseLeave += MyBorder_MouseLeave;
            MediaElement mediaElement = new MediaElement();
            // 预加载媒体资源，但不播放
            mediaElement.LoadedBehavior = MediaState.Manual;
            // 设置控件尺寸比例与媒体资源自适应
            mediaElement.Stretch = System.Windows.Media.Stretch.Uniform;

            // 视频的第一帧作为视频封面 start
            mediaElement.ScrubbingEnabled = true;
            mediaElement.Pause();
            mediaElement.Position = TimeSpan.FromTicks(1);
            // 视频的第一帧作为视频封面 end

            // 鼠标经过时开始播放媒体
            mediaElement.MouseEnter += On_MediaPlayer_Play;
            // 鼠标离开时结束播放媒体
            mediaElement.MouseLeave += On_MediaPlayer_Stop;
            // 视频播放失败时回收资源
            mediaElement.MediaFailed += MediaPlayer_Failed;
            // 视频播放完毕时重置播放进度为第一帧
            mediaElement.MediaEnded += new RoutedEventHandler(MediaPlayer_Ended);
            // 设置资源 URI
            mediaElement.Source = new Uri(file_path, UriKind.Absolute);


            // 在布局控件里设置媒体控件
            MediaIndexMark.Mark.SetMarkIndex(myBorder, index);
            myBorder.Child = mediaElement;
            _ = wrap_pannel.Children.Add(myBorder);
        }

        private void MyBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = Brushes.AliceBlue;
        }

        private void MyBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            active_border_element = border;
            border.Background = Brushes.Salmon;

        }

        private void MediaPlayer_Failed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.ToString());
        }

        private void MediaPlayer_Ended(object sender, RoutedEventArgs routedEventArgs)
        {
            MediaElement media = sender as MediaElement;
            media.Position = TimeSpan.FromTicks(1);
            media.Play();
        }
        void On_MediaPlayer_Play(object sender, RoutedEventArgs routedEventArgs)
        {
            MediaElement media = sender as MediaElement;
            media.Play();
        }
        void On_MediaPlayer_Stop(object sender, RoutedEventArgs routedEventArgs)
        {
            MediaElement media = sender as MediaElement;
            media.Stop();
        }
        void On_Select_File(object sender, RoutedEventArgs routedEventArgs)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择数据源文件";
            openFileDialog.Filter = "视频文件|*.MP4|MOV文件|*.MOV|WMV文件|*.WMV|3gp文件|*.3gp";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "mp4";
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }
            string file_path = openFileDialog.FileName;
            Add_Files_Path(file_path);
        }

        void Add_Files_Path(string file_path)
        {
            bool is_includes = ((IList)file_paths).Contains(file_path);

            if (is_includes == false)
            {

                Add_Item_In_View(file_path);
                file_paths.Add(file_path);
            }
            else
            {
                MessageBox.Show("此文件已经添加过");
            }
        }
        void Add_Item_In_View(string path)
        {
            int i = file_paths.Count;
           
            Load_Video(path, i);
        }
        void Add_All_Item_In_View(ArrayList paths)
        {

            for (int i = 0; i < paths.Count; i++)
            {
 
                Load_Video(paths[i].ToString(), i);
            }
        }
        void Set_Window_Background(object sender, RoutedEventArgs routedEventArgs)
        {
            List<MediaElement> initToolBarWeChatUserSp = GetChildObjects<MediaElement>(active_border_element, typeof(MediaElement));
            Uri uri = initToolBarWeChatUserSp[0].Source;
            window_player = new Player();
            window_player.uri = uri;
            window_player.Show();
        }
        void Remove_Item_Handler(object sender, RoutedEventArgs routedEventArgs)
        {
            int has_element = wrap_pannel.Children.IndexOf(active_border_element);

            if (active_border_element != null && has_element != -1)
            {
                int index = MediaIndexMark.Mark.GetMarkIndex(active_border_element);
                try
                {
                    file_paths.RemoveAt(index);
                    wrap_pannel.Children.Remove(active_border_element);
                }
                catch (Exception msg)
                {
                    throw msg;
                }
            }
        }
        void On_Stop_window(object sender, RoutedEventArgs routedEventArgs)
        {
            if(window_player != null && window_player.ShowActivated)
            {
                window_player.Hide();
            }
        }
        /// <summary>
        /// 根据类型查找子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="typename"></param>
        /// <returns></returns>
        public List<T> GetChildObjects<T>(DependencyObject obj, Type typename) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).GetType() == typename))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child, typename));
            }
            return childList;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
