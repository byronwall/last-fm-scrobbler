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
    public partial class WindowLog
    {
        public WindowLog()
        {
            this.InitializeComponent();
            lstLogOutput.ItemsSource = Log.Instance.LogEvents;
            // Insert code required on object creation below this point.
        }

        private void WindowLogMain_Closed(object sender, EventArgs e)
        {
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The dialog was closed", LogEvent.LogEventStatus.Neutral);
        }
    }
}