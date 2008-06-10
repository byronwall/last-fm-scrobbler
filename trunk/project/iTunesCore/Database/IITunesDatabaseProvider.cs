using System.Xml;
using System.Xml.Linq;

namespace iTunesCore
{
    public interface IITunesDatabaseProvider
    {
        XDocument DatabaseXMLReader { get; }
    }
}