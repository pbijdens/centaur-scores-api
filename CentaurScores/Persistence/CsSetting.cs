namespace CentaurScores.Persistence
{
    public class CsSetting
    {
        public const string ActiveMatchId = nameof(ActiveMatchId);

        required public string Name { get; set; }
        public string? JsonValue { get; set; }
    }
}
