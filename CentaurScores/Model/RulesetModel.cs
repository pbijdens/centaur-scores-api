﻿namespace CentaurScores.Model
{
    /// <summary>
    /// This exists as model only.
    /// </summary>
    public class RulesetModel
    {
        /// <summary>
        /// Only rulesets in the same group(s) may be combined in a single competition.
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// Uniquely identifies the ruleset in database objects.
        /// </summary>
        public string Code { get; set; } = string.Empty;
        
        /// <summary>
        /// Friendly name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// If set, limits what users can enter as number of ends for new matches.
        /// </summary>
        public int? RequiredEnds { get; set; }

        /// <summary>
        /// If set, limits what users can enter as number of arrows per end for new matches.
        /// </summary>
        public int? RequiredArrowsPerEnd { get; set; }

        /// <summary>
        /// Classes supported in this competition, non-negotiable.
        /// </summary>
        public List<GroupInfo> RequiredClasses { get; set; } = [];

        /// <summary>
        /// Subclasses supported in this competition, non-negotiable.
        /// </summary>
        public List<GroupInfo> RequiredSubclasses { get; set; } = [];

        /// <summary>
        /// Targets supported in this competition, non-negotiable.
        /// </summary>
        public List<GroupInfo> RequiredTargets { get; set; } = [];

        /// <summary>
        /// Defines one keyboard per target type.
        /// </summary>
        public Dictionary<string, List<ScoreButtonDefinition>> RequiredScoreValues { get; set; } = [];
    }
}