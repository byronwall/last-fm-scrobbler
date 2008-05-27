using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace LastFM
{
    public partial class WindowStatus
    {
        public WindowStatus()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
        }

        private void WindowStatusMain_Closed(object sender, EventArgs e)
        {
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The dialog was closed", LogEvent.LogEventStatus.Neutral);
        }

        private void WindowStatusMain_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = Window1.Instance;
        }
    }
}