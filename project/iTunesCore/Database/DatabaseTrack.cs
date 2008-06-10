namespace iTunesCore
{
    public struct DatabaseTrack
    {
        public string Filename { get; set; }
        public int PlayCount { get; set; }

        public static DatabaseTrack FromIITTrack(iTunesLib.IITFileOrCDTrack track)
        {
            return new DatabaseTrack() { Filename = track.Location, PlayCount = track.PlayedCount };
        }
    }
}