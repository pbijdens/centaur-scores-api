namespace CentaurScores.Persistence
{
    /// <summary>
    /// DB ID
    /// </summary>
    public class CsSetting
    {
        /// <summary>
        /// Active match constant
        /// </summary>
        public const string ActiveMatchId = nameof(ActiveMatchId);
        /// <summary>
        /// Which devices need synchronization?
        /// </summary>
        public const string DevicesNeedingForcedSync = nameof(DevicesNeedingForcedSync);

        /// <summary>
        /// Record name or key.
        /// </summary>
        required public string Name { get; set; }

        /// <summary>
        /// JSON-encoded value.
        /// </summary>
        public string? JsonValue { get; set; }
    }
}
