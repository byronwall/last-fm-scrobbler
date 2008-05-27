using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace LastFMVersionControl
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                const string serverFolder = @"C:\Program Files\Apache Software Foundation\Apache2.2\htdocs\LastFM\";
                const string versionFilename = @"C:\Program Files\Apache Software Foundation\Apache2.2\htdocs\LastFM\version.xml";
                const string historyFolder = "F:\\My Documents\\Visual Studio 2008\\Projects\\LastFM\\History\\";
                string executable;

                if (args.Length > 0)
                {
                    executable = args[0];
                }
                else
                {
                    throw new ArgumentException("Need to send the filename in the argument");
                }


                Assembly assembly = Assembly.LoadFile(executable);
                Version newestVersion = assembly.GetName().Version;
                Version currentVersion;

                string newestHistoryFolder = string.Format("{0}{1}", historyFolder, newestVersion.ToString());

                if (!Directory.Exists(newestHistoryFolder))
                {
                    Directory.CreateDirectory(newestHistoryFolder);
                }
                File.Copy(executable, string.Format("{0}\\LastFM{1}.exe", newestHistoryFolder, newestVersion.ToString()), true);
                File.Copy(executable, string.Format("{0}LastFM{1}.exe", serverFolder, newestVersion.ToString(), true));

                XDocument fileDoc = new XDocument();
                XElement fileRoot = new XElement(("fileHistory"),
                    new XElement("file", string.Format("LastFM{0}.exe", newestVersion.ToString())));
                fileDoc.Add(fileRoot);
                fileDoc.Save("fileHistory.xml");
                File.Copy("fileHistory.xml", string.Format("{0}fileHistory.xml", serverFolder), true);
                File.Delete("fileHistory.xml");

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                using (XmlReader readStream = XmlReader.Create(versionFilename, settings))
                {
                    //insert processing logic here
                    readStream.ReadToFollowing("version");
                    XElement x = (XElement)XElement.ReadFrom(readStream);
                    int major = (int)(x.Element("major"));
                    int minor = (int)(x.Element("minor"));
                    int build = (int)(x.Element("build"));
                    int revision = (int)(x.Element("revision"));

                    currentVersion = new Version(major, minor, build, revision);
                }
                if (currentVersion == newestVersion)
                {
                    Console.WriteLine("File is current.  No change");
                }
                else if (currentVersion < newestVersion)
                {
                    Console.WriteLine("The file needs to be updated from {0} to {1}.", currentVersion.ToString(), newestVersion.ToString());
                    XDocument doc = new XDocument();
                    XElement root = new XElement(("version"),
                     new XElement("major", newestVersion.Major),
                     new XElement("minor", newestVersion.Minor),
                     new XElement("build", newestVersion.Build),
                     new XElement("revision", newestVersion.Revision));
                    doc.Add(root);
                    doc.Save("version.xml");
                    File.Copy("version.xml", versionFilename, true);
                    File.Delete("version.xml");

                }
                else
                {
                    Console.WriteLine("Odd version found.  Look into.");
                }
            }
            catch (Exception e)
            {
                using (FileStream fs = new FileStream("error.txt"))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(e.ToString());
                    }
                }
            }
        }
        static private void CopyFileToHistory()
        {

        }
        static private void CreateAndCopyVersionInfo()
        {

        }
    }
}
