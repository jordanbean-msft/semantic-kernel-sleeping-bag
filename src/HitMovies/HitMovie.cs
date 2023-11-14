namespace HitMovies
{
    public record HitMovie
    {
        public string Title { get; init; } = "";
        public List<string> Actors { get; init; } = new List<string>();
        public List<string> Tags { get; init; } = new List<string>();
    }
}