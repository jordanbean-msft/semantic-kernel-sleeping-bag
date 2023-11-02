public record HitTVShow
{
    public string Title { get; init; }
    public List<string> Actors { get; init; }
    public List<string> Tags { get; init; }
    public List<int> SeasonAiredYears { get; init; }
}
