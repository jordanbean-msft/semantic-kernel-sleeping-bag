namespace HistoricalWeatherLookup
{
    public record HistoricalWeatherKey
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public int Month { get; init; }
    };
}
