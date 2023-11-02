namespace HistoricalWeatherLookup
{
    public record HistoricalWeatherInput
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public int MonthOfYear { get; init; }
    };
}
