using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Linq;

namespace LastFM
{
    class AutoUpdate
    {
        private static object locker = new object();
        private static bool isChecking = false;
        private static bool IsChecking
        {
            get
            {
                return isChecking;
            }
            set
            {
                lock (locker)
                {
                    isChecking = value;
                }
            }
        }
        public static void CheckForUpdates()
        {
            if (IsChecking) return;
            IsChecking = true;
            if (File.Exists("LastFM.exe.bak"))
            {
                File.Delete("LastFM.exe.bak");
            }

            Version assemblyVersion = GetAssemblyVersion();
            Version updatedVersion = GetCurrentVersion();
            if (assemblyVersion < updatedVersion)
            {
                Log.Instance.AddEvent("There is a newer version of the program and it is being downloaded.");
                UpdateCurrentVersion();
            }
            else if (assemblyVersion == updatedVersion)
            {
                Log.Instance.AddEvent("This program is up to date.");
            }
            else
            {
                Log.Instance.AddEvent("The program is newer than the latest version.  Probably corrupt.");
            }
            IsChecking = false;
        }
        private static Version GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
        private static Version GetCurrentVersion()
        {
            Version version = null;
            string url = "http://128.211.190.122/LastFM/version.xml";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = null;
            request.Timeout = 15 * 1000;
            request.KeepAlive = false;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.IgnoreWhitespace = true;
                        using (XmlReader readStream = XmlReader.Create(receiveStream, settings))
                        {
                            //insert processing logic here
                            readStream.ReadToFollowing("version");
                            XElement x = (XElement)XElement.ReadFrom(readStream);
                            int major = (int)(x.Element("major"));
                            int minor = (int)(x.Element("minor"));
                            int build = (int)(x.Element("build"));
                            int revision = (int)(x.Element("revision"));

                            version = new Version(major, minor, build, revision);
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            return version;
        }
        private static string DownloadLatestFiles()
        {
            string filename;
            string output = "LastFM.exe";
            string url = "http://128.211.190.122/LastFM/fileHistory.xml";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = null;
            request.Timeout = 15 * 1000;
            request.KeepAlive = false;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.IgnoreWhitespace = true;
                        using (XmlReader readStream = XmlReader.Create(receiveStream, settings))
                        {
                            //insert processing logic here
                            readStream.ReadToFollowing("fileHistory");
                            XElement x = (XElement)XElement.ReadFrom(readStream);
                            filename = (string)(x.Element("file"));
                        }
                    }
                }
                if (filename != null)
                {
                    WebClient wc = new WebClient();
                    wc.DownloadFile(string.Format("http://128.211.190.122/LastFM/{0}", filename), filename);
                    output = filename;
                }
            }
            catch (Exception e)
            {

            }
            return output;

        }
        private static void UpdateCurrentVersion()
        {
            try
            {
                string newFile = DownloadLatestFiles();
                string runningFile = Assembly.GetExecutingAssembly().Location;

                File.Move(runningFile, "LastFM.exe.bak");
                File.Move(newFile, runningFile);
                Log.Instance.AddEvent("The newest version is downloaded and ready to go.");
                InterfaceHelper.ShowUpdateRestartDialog();
            }
            catch (Exception e)
            {
                Log.Instance.AddEvent(e);
            }
        }
    }
}
