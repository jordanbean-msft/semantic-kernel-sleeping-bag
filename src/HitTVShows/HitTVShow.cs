public record HitTVShow
{
    public string Title { get; init; } = "";
    public List<string> Actors { get; init; } = new List<string>();
    public List<string> Tags { get; init; } = new List<string>();
    public List<int> SeasonAiredYears { get; init; } = new List<int>();
}
