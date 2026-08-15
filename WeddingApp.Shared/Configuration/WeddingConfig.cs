namespace WeddingApp.Shared.Configuration
{
    public class WeddingConfig
    {
        public string GroomName { get; set; } = "Matthew Gernale";
        public string BrideName { get; set; } = "Erika Joyce Gatchalian";
        public string Hashtag { get; set; } = "#ErikaFoundHerPerfectMattch";
        
        public WeddingDate Date { get; set; } = new();
        public Ceremony Ceremony { get; set; } = new();
        public Reception Reception { get; set; } = new();
        public RSVPInfo RSVP { get; set; } = new();
        public GiftInfo Gifts { get; set; } = new();
    }

    public class WeddingDate
    {
        public DateTime CeremonyTime { get; set; } = new(2026, 10, 8, 13, 30, 0);
        public string FormattedDate => CeremonyTime.ToString("MMMM dd, yyyy");
        public string FormattedTime => CeremonyTime.ToString("h:mm tt");
    }

    public class Ceremony
    {
        public string Name { get; set; } = "St. Gabriel the Archangel Parish Church";
        public string Address { get; set; } = "San Gabriel, Santa Maria, Bulacan";
        public string MapEmbedUrl { get; set; } = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3862.196897968348!2d121.34867342342317!3d14.791749472250888!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3397ac8d4e4f1111%3A0x1234567890abcdef!2sSt.%20Gabriel%20the%20Archangel%20Parish%20Church!5e0!3m2!1sen!2sph!4v1699000000000";
    }

    public class Reception
    {
        public string VenueName { get; set; } = "Brick House Gardens";
        public string Address { get; set; } = "Sitio Laot, Brgy. Buenavista, Parada, Santa Maria, Bulacan";
        public string MapEmbedUrl { get; set; } = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3862.5!2d121.35!3d14.79!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3397b12345678!2sBrick%20House%20Gardens!5e0!3m2!1sen!2sph!4v1699000000000";
        public TimeSpan StartsAfterCeremonyEnds { get; set; } = TimeSpan.FromHours(1.5);
    }

    public class RSVPInfo
    {
        public string GoogleFormUrl { get; set; } = "https://docs.google.com/forms/d/e/1FAIpQLSdxnnsS1Bqio97ULqz33YF9o17Zk0Q6jEvbxqIFlE6v4_hi8w/viewform";
        public DateTime Deadline { get; set; } = new(2026, 9, 1);
        public string FormattedDeadline => Deadline.ToString("MMMM dd, yyyy");
        
        public List<string> ContactNumbers { get; set; } = new()
        {
            "+639858648265",
            "+639500686215"
        };
    }

    public class GiftInfo
    {
        public string PaymentNote { get; set; } = "Monetary gifts are not necessary, but appreciated.";
        public string QRCodePlaceholder { get; set; } = "To be updated with actual QR code";
    }
}
