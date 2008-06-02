using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastFM
{
    class InterfaceHelper
    {
        private static InterfaceHelper instance = null;

        internal static InterfaceHelper Instance
        {
            get
            {
                //if (instance == null)
                //{
                //    instance = new InterfaceHelper();
                //}
                return InterfaceHelper.instance;
            }
            set { InterfaceHelper.instance = value; }
        }

        private static Window1 WindowInstance = null;
        public InterfaceHelper(Window1 windowRef)
        {
            WindowInstance = windowRef;
        }

        public static void SetDBConnected(bool connected)
        {
            WindowInstance.SetDBConnected(connected);
            //Window1.Instance.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate() { Window1.Instance.DBConnected = connected; });
            //Log.Instance.AddEvent(LogEvent.LogEventSender.Other, String.Format("DBConnected was changed to {0}.", connected.ToString()), LogEvent.LogEventStatus.Neutral);//change
        }
        public static void SetITunesConnected(bool connected)
        {
            WindowInstance.SetITunesConnected(connected);
            //Window1.Instance.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate() { Window1.Instance.ITunesConnected = connected; });
        }
        public static void SetCacheSize(int size)
        {
            WindowInstance.SetCacheSize(size);
            //Window1.Instance.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate() { Window1.Instance.CacheSize = size; });
        }
        public static void SetLastFMConnected(string username)
        {
            WindowInstance.SetUserName(username);
            //Window1.Instance.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate() { Window1.Instance.LastFMConnection = username; });
        }
        public static void SetPlayTimeStatus(object playTimeStatus)
        {
            WindowInstance.SetPlayTimeStatus(playTimeStatus);
            //Window1.Instance.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate() { Window1.Instance.PlayTimeStatus = playTimeStatus; });
        }
        public static void SetProgressProperties(double value)
        {
            WindowInstance.SetProgressValue(value);
            //Window1.Instance.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate()
            //{
            //    Window1.Instance.ProgressValue = value;
            //    if (Window1.Instance.ProgressSource != content)
            //    {
            //        Window1.Instance.ProgressSource = content;
            //    }
            //});
        }
        public static void SetPlayingArtist(string artist)
        {
            WindowInstance.SetPlayingArtist(artist);
            //Window1.Instance.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate() { Window1.Instance.PlayingArtist = artist; });
        }
        public static void SetPlayingAlbum(string album)
        {
            WindowInstance.SetPlayingAlbum(album);
            //Window1.Instance.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate() { Window1.Instance.PlayingAlbum = album; });
        }
        public static void SetPlayingTitle(string title)
        {
            WindowInstance.SetPlayingTitle(title);
            //Window1.Instance.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate() { Window1.Instance.PlayingTitle = title; });
        }

        internal static void CloseProgram()
        {
            WindowInstance.CloseProgram();
            //Window1.Instance.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate() { Window1.Instance.Close(); });
        }
        public static void SetCacheFire(DateTime nextFire)
        {
            WindowInstance.SetCacheFire(nextFire);
        }
        public static void ShowArtistInfo(string artist)
        {
            WindowInstance.GetArtistInfo(artist);
        }
        public static void RestoreFromTrayIcon()
        {
            WindowInstance.RestoreWindow();
            Tray.HideIcon();
        }
        public static void MinimizeToTrayIcon()
        {
            WindowInstance.HideWindow();
            Tray.ShowIcon();
        }

        internal static void ShowUpdateRestartDialog()
        {
            WindowInstance.ShowUpdateRestartDialog();
        }
    }
}
