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
    public partial class WindowSettings
    {
        public WindowSettings()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LastFMHelper.Connect(textBox1.Text, MD5Helper.MD5(passwordBox1.Password), false);
        }

        private void WindowSettingsMain_Closed(object sender, EventArgs e)
        {
            Log.Instance.AddEvent(LogEvent.LogEventSender.Other, "The dialog was closed", LogEvent.LogEventStatus.Neutral);
        }
    }
}