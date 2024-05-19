namespace StravaMultiMapper2.Models
{
    public class Map
    {
        public string id { get; set; }
        public string summary_polyline { get; set; }
    }

    public class StravaActivity
    {
        public string name { get; set; }
        public Map map { get; set; }
    }
}
