namespace CentaurScores.Model
{
    /// <summary>
    /// Represents a single line in a competition result, including a participant plus their scores.
    /// </summary>
    public class CompetitionScoreForParticipantModel
    {
        /// <summary>
        /// Should be rendered as the participant's name, but may include some markup data to indicate 'special' situations such as a draw.
        /// </summary>
        public string ParticipantInfo { get; set; } = string.Empty;

        /// <summary>
        /// Data structure identifying the participant, their actual name, their class and their subclass.
        /// </summary>
        public ParticipantData ParticipantData { get; set; } = new();

        /// <summary>
        /// The positiuon of the participant in the ranking. May be equal to other participants.
        /// </summary>
        public int Position { get; set; } = 0;

        /// <summary>
        /// Total score of the participant. Determines the ranking.
        /// </summary>
        public int TotalScore {  get; set; }

        /// <summary>
        /// Per ruleset (in the ruleset group for the competition) the score data of this participant.
        /// </summary>
        public Dictionary<string, CompetitionRulesetResultEntry> PerRuleset { get; set; } = [];

    }
}