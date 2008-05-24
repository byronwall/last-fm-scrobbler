using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace SingleInstance
{
    public class Startup
    {

        [STAThread]
        public static void Main(string[] args)
        {
            SingleInstance.SingleInstanceApplicationWrapper wrapper = new SingleInstanceApplicationWrapper();
            wrapper.MinimumSplashScreenDisplayTime = 0;
            wrapper.Run(args);
            if (wrapper.RestartOnExit)
            {
                Process.Start(Assembly.GetExecutingAssembly().Location, "restart");
            }
        }
    }
    public class SingleInstanceApplicationWrapper : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
    {
        Mutex mutexSafetyNet;
        public bool RestartOnExit { get; set; }
        public SingleInstanceApplicationWrapper()
        {

            this.IsSingleInstance = true;
            RestartOnExit = false;
        }
        private WpfApp app;
        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs eventArgs)
        {
            bool mutex;
            mutexSafetyNet = new Mutex(true, "ByronWall.LastFM.Debug", out mutex);
            if (!mutex)
            {
                Process.Start("http://espn.com");
            }
            GC.KeepAlive(mutexSafetyNet);
            app = new WpfApp(this);
            app.Run();

            return false;
        }
        protected override void OnStartupNextInstance(Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs eventArgs)
        {
            app.MainWindow.Activate();
        }
    }
    public class WpfApp : System.Windows.Application
    {
        SingleInstanceApplicationWrapper parent;
        public WpfApp(SingleInstanceApplicationWrapper parent)
        {
            this.parent = parent;
        }
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);
            //WpfApp.Current = this;

            this.MainWindow = new LastFM.Window1(parent);
            //this.MainWindow.Show();
        }

    }
}
