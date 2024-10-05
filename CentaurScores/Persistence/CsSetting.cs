namespace CentaurScores.Persistence
{
    public class CsSetting
    {
        public const string ActiveMatchId = nameof(ActiveMatchId);
        public const string DevicesNeedingForcedSync = nameof(DevicesNeedingForcedSync);

        required public string Name { get; set; }
        public string? JsonValue { get; set; }
    }
}
