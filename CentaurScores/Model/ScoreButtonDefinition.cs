namespace CentaurScores.Model
{
    /// <summary>
    /// Single button for entering scores with.
    /// </summary>
    public class ScoreButtonDefinition
    {
        /// <summary>
        /// Not used.
        /// </summary>
        public int Id { get; set; } = -1;
        /// <summary>
        /// The label that should be shown on the button.
        /// </summary>
        public string Label { get; set; } = string.Empty;
        /// <summary>
        /// The value to be entered when the button is pressed. Use '0' for a miss and use null 
        /// to indicate that an arrow score is to be cleared (del).
        /// </summary>
        public int? Value { get; set; } = null;
    }
}
