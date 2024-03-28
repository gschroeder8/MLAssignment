public class Movie
{
    public ulong MediaId { get; set; }
    public string Title { get; set; }
    public List<string> Genres { get; set; }
    public string Director { get; set; }
    public TimeSpan RunningTime { get; set; }
}
