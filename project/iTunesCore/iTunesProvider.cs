using iTunesLib;

namespace iTunesCore
{
    class iTunesProvider : IiTunesProvider
    {
        public iTunesApp Instance
        {
            get { return iTunesInstance.Instance; }
        }
    }
}