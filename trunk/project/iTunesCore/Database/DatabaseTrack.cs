namespace iTunesCore
{
    public class DatabaseTrack
    {
        public string Filename { get; set; }
        public int PlayCount { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int Length { get; set; }
        public int TrackNumber { get; set; }
        public int NewPlays { get; set; }

        public static DatabaseTrack FromIITTrack(iTunesLib.IITFileOrCDTrack track)
        {
            return new DatabaseTrack()
                       {
                           Filename = track.Location,
                           PlayCount = track.PlayedCount,
                           NewPlays = 0,
                           Album = track.Album,
                           Artist = track.Artist,
                           Length = track.Duration,
                           Title = track.Name,
                           TrackNumber = track.TrackNumber
                       };

        }
    }
}