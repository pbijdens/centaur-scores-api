using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentaurScores.CompetitionLogic.TotalScoreBasedLogic
{
    /// <summary>
    /// Used to encode into RulesetParameterJSON to allow configuring a competition instance.
    /// </summary>
    public class TsbCompetitionParameters
    {
        /// <summary>
        /// The number of best scoring weeks per match type that's used in determining the user's score.
        /// </summary>
        public string ScoringMatches { get; set; } = "5";

        /// <summary>
        /// Weeks as integer.
        /// </summary>
        public int ScoringMatchesAsInt => Convert.ToInt32(ScoringMatches);

        /// <summary>
        /// The scoring style. 
        /// "default" will use as a week-score the point score from the participant.
        /// "F1" will per week assign 12, 10, 8, 7, 6, 5, 4, 3, 2, 1, 1, 1, 1, 1* points to the actual participants that week and use that as a point score.
        /// </summary>
        public string Scoring { get; set; } = "default";
    }
}
