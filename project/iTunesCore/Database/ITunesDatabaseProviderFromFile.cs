using System.Xml.Linq;

namespace iTunesCore
{
    public class ITunesDatabaseProviderFromFile : IITunesDatabaseProvider
    {
        private readonly string filename;

        public ITunesDatabaseProviderFromFile(string filename)
        {
            this.filename = filename;
        }

        public XDocument DatabaseXMLReader
        {
            get { return XDocument.Load(this.filename); }
        }
    }
}