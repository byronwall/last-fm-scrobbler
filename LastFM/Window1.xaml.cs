using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using iTunes_WPF;
using LastFM.TrackInfo;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace LastFM
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        bool restart = false;
        SingleInstance.SingleInstanceApplicationWrapper ApplicationParent;
        public Window1(SingleInstance.SingleInstanceApplicationWrapper container)
        {
            ApplicationParent = container;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            InitializeComponent();

            Instance = this;
            InterfaceHelper.Instance = new InterfaceHelper(this);
            Show();
            CheckRestart();


        }

        private void CheckRestart()
        {
            if (restart == true)
            {
                RestartProgram();
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, string.Format("Unhandled exception:{0}\tStack:{1}", ex.Message, ex.StackTrace), LogEvent.LogEventStatus.Failure);
            LastFM.LastFMHelper.SaveState("cache.bin");
        }
        static readonly DependencyProperty ITunesConnectedProperty = DependencyProperty.Register("ITunesConnected", typeof(bool), typeof(Window1));
        static readonly DependencyProperty DBConnectedProperty = DependencyProperty.Register("DBConnected", typeof(bool), typeof(Window1));

        static readonly DependencyProperty CacheSizeProperty = DependencyProperty.Register("CacheSize", typeof(int), typeof(Window1));
        static readonly DependencyProperty LastFMConnectionProperty = DependencyProperty.Register("LastFMConnection", typeof(string), typeof(Window1));
        static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register("ProgressValue", typeof(double), typeof(Window1));
        static readonly DependencyProperty ProgressSourceProperty = DependencyProperty.Register("ProgressSource", typeof(string), typeof(Window1));
        static readonly DependencyProperty PlayTimeStatusProperty = DependencyProperty.Register("PlayTimeStatus", typeof(object), typeof(Window1));
        static readonly DependencyProperty PlayingArtistProperty = DependencyProperty.Register("PlayingArtist", typeof(string), typeof(Window1));
        static readonly DependencyProperty PlayingAlbumProperty = DependencyProperty.Register("PlayingAlbum", typeof(string), typeof(Window1));
        static readonly DependencyProperty PlayingTitleProperty = DependencyProperty.Register("PlayingTitle", typeof(string), typeof(Window1));
        public string PlayingTitle
        {
            get
            {
                return (string)base.GetValue(PlayingTitleProperty);
            }
            set
            {
                base.SetValue(PlayingTitleProperty, value);
            }
        }
        public string PlayingAlbum
        {
            get
            {
                return (string)base.GetValue(PlayingAlbumProperty);
            }
            set
            {
                base.SetValue(PlayingAlbumProperty, value);
            }
        }
        public string PlayingArtist
        {
            get
            {
                return (string)base.GetValue(PlayingArtistProperty);
            }
            set
            {
                base.SetValue(PlayingArtistProperty, value);
            }
        }
        public object PlayTimeStatus
        {
            get
            {
                return (object)base.GetValue(PlayTimeStatusProperty);
            }
            set
            {
                base.SetValue(PlayTimeStatusProperty, value);
            }
        }
        public string ProgressSource
        {
            get
            {
                return (string)base.GetValue(ProgressSourceProperty);
            }
            set
            {
                base.SetValue(ProgressSourceProperty, value);
            }
        }
        public double ProgressValue
        {
            get
            {
                return (double)base.GetValue(ProgressValueProperty);
            }
            set
            {
                base.SetValue(ProgressValueProperty, value);
            }
        }
        public string LastFMConnection
        {
            get
            {
                return (string)base.GetValue(LastFMConnectionProperty);
            }
            set
            {
                base.SetValue(LastFMConnectionProperty, value);
            }
        }
        public bool ITunesConnected
        {
            get
            {
                return (bool)base.GetValue(ITunesConnectedProperty);
            }
            set
            {
                base.SetValue(ITunesConnectedProperty, value);
            }
        }
        public bool DBConnected
        {
            get
            {
                return (bool)base.GetValue(DBConnectedProperty);
            }
            set
            {
                base.SetValue(DBConnectedProperty, value);
            }
        }
        public int CacheSize
        {
            get
            {
                return (int)base.GetValue(CacheSizeProperty);
            }
            set
            {
                base.SetValue(CacheSizeProperty, value);
            }
        }
        static readonly DependencyProperty UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(Window1));
        public string UserName
        {
            get
            {
                return (string)base.GetValue(UserNameProperty);
            }
            set
            {
                base.SetValue(UserNameProperty, value);
            }
        }
        public static Window1 Instance;
        private bool full_on = false;

        Timer updateTimer;

        static readonly DependencyProperty CacheFireProperty = DependencyProperty.Register("CacheFire", typeof(DateTime), typeof(Window1));
        public DateTime CacheFire
        {
            get
            {
                return (DateTime)base.GetValue(CacheFireProperty);
            }
            set
            {
                base.SetValue(CacheFireProperty, value);
            }
        }
        public void SetCacheFire(DateTime input)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<DateTime>(SetCacheFire), input);
                return;
            }
            CacheFire = input;
            //insert additional code here
        }


        private void BeginCheckForUpdates(object state)
        {
            //insert code here
            AutoUpdate.CheckForUpdates();
        }

        private void BeginUpdateITunesDB(object state)
        {
            //insert code here
            Log.Instance.AddEvent("The DB is being updated.");
            Database.UpdateDB();
        }

        private void BeginLastFMConnectFromHistory(object state)
        {
            //insert code here
            if (LastFMHelper.ReadUserDetails())
            {
                LastFMHelper.Reconnect();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //CollectionViewSource cvs = new CollectionViewSource();
            //cvs.Source = Log.Instance.LogEvents;
            //cvs.GroupDescriptions.Add(new PropertyGroupDescription("Sender"));
            //lstLogOutput.ItemsSource = cvs.View;
            lstLogOutput.ItemsSource = Log.Instance.LogEvents;
             //if (!full_on) return;
            ThreadPool.QueueUserWorkItem(BeginLastFMConnectFromHistory);           
            iTunes_WPF.iTunesReference.Initialize();
            ThreadPool.QueueUserWorkItem(BeginUpdateITunesDB);
            LastFMHelper.AddTracksToCacheFromFile("cache.bin");
            updateTimer = new Timer(BeginCheckForUpdates, null, 10 * 1000, 24 * 60 * 60 * 1000);
            //ShowUpdateRestartDialog();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LastFMHelper.SaveState("cache.bin");
            Database.DeleteDB();
            //LastFMHelper.SerializeCacheToFile();
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (((Window)sender).WindowState == WindowState.Minimized)
            {
                InterfaceHelper.MinimizeToTrayIcon();
            }
        }
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            //popupMain.IsOpen = true;
            //popupMain.StaysOpen = false;
        }
        private void btnSimilar_Click(object sender, RoutedEventArgs e)
        {
            similarMain.InitializeArtist(PlayingArtist);
            //popupExtraInfo.IsOpen = true;
            //popupExtraInfo.StaysOpen = false;
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LastFMHelper.Connect(textBox1.Text, MD5Helper.MD5(passwordBox1.Password), false);
        }
        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start(string.Format("http://www.last.fm/user/{0}", LastFMHelper.Username));
        }
        private void TextBlock_Drop(object sender, DragEventArgs e)
        {
            string[] filenames = (string[])e.Data.GetData("FileDrop", false);
            foreach (string filename in filenames)
            {
                LastFMHelper.AddTracksToCacheFromFile(filename);
                Log.Instance.AddEvent(filename);
            }
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            //popupExtraInfo.IsOpen = (popupExtraInfo.Tag != null) ? (bool)popupExtraInfo.Tag : false;
            //popupMain.IsOpen = (popupMain.Tag != null) ? (bool)popupMain.Tag : false;
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            //popupExtraInfo.Tag = popupExtraInfo.IsOpen;
            //popupMain.Tag = popupMain.IsOpen;

            //popupExtraInfo.IsOpen = false;
            //popupMain.IsOpen = false;
        }

        #region Methods to be called externally

        public void SetDBConnected(bool connected)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<bool>(SetDBConnected), connected);
                return;
            }
            DBConnected = connected;
        }
        public void SetITunesConnected(bool connected)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<bool>(SetITunesConnected), connected);
                return;
            }
            ITunesConnected = connected;
        }
        public void SetCacheSize(int size)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<int>(SetCacheSize), size);
                return;
            }
            CacheSize = size;
        }
        public void SetUserName(string username)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<string>(SetUserName), username);
                return;
            }
            //code to be executed
            UserName = username;
            userInfoInstance.UpdateCurrentUser(UserName);

        }
        public void SetPlayTimeStatus(object input)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<object>(SetPlayTimeStatus), input);
                return;
            }
            //code to be executed
            PlayTimeStatus = input;
        }
        public void SetProgressValue(double input)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<double>(SetProgressValue), input);
                return;
            }
            //code to be executed            
            ProgressValue = input;
        }
        public void SetPlayingArtist(string artist)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<string>(SetPlayingArtist), artist);
                return;
            }
            //code to be executed
            PlayingArtist = artist;
        }
        public void SetPlayingAlbum(string album)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<string>(SetPlayingAlbum), album);
                return;
            }
            //code to be executed
            PlayingAlbum = album;
        }
        public void SetPlayingTitle(string title)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action<string>(SetPlayingTitle), title);
                return;
            }
            //code to be executed
            PlayingTitle = title;
        }
        public void CloseProgram()
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(CloseProgram));
                return;
            }
            this.Close();
        }
        public void RestoreWindow()
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(RestoreWindow));
                return;
            }
            //code to be executed
            Show();
            Activate();
            WindowState = WindowState.Normal;
        }
        public void HideWindow()
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(HideWindow));
                return;
            }
            //code to be executed
            Hide();
        }

        public void ShowUpdateRestartDialog()
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(ShowUpdateRestartDialog));
                return;
            }
            //code to be executed
            MessageBoxResult result = MessageBox.Show("The program was just updated.  Do you want to restart now and enjoy the new version?", "Restart?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                //restart = true;
                ApplicationParent.RestartOnExit = true;
                Close();
                //using (FileStream fs = new FileStream("restart.dat", FileMode.Create))
                //{
                //    using (StreamWriter sw = new StreamWriter(fs))
                //    {
                //        sw.Write("restart");
                //    }
                //}
            }
        }

        private void RestartProgram()
        {
            Close();
        }
        #endregion

        private void textPlayingArtist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            GetArtistInfo(PlayingArtist);
        }
        public void GetArtistInfo(string artist)
        {
            tabProgramExtras.SelectedIndex = 0;
            similarMain.InitializeArtist(artist);
        }

        private void buttonCheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(BeginCheckForUpdates);
        }
    }
    public sealed class ColumnLeftValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((double)value > 100) value = 100;
            return ((Double)value).ToString() + "*";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public sealed class ColumnRightValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((double)value > 100) value = 100;
            return (100.0 - (Double)value).ToString() + "*";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public sealed class SecondsToTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                int _value = (int)value;
                return string.Format("{0}:{1:00}", _value / 60, _value % 60);
            }
            else
            {
                return (string)value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public sealed class TimeToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime input = (DateTime)value;
            string output = "<no cache>";
            if (input != DateTime.MinValue)
            {
                output = input.ToShortTimeString();
            }
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
