﻿using CentaurScores.Persistence;

namespace CentaurScores.Model
{
    public class ParticipantListMemberModel
    {
        public int? Id { get; set; } = null;
        /// <summary>This is the name of the participant.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>A default group for the participant.</summary>
        public string Group { get; set; } = string.Empty;
        /// <summary>A default sub-group for the participant.</summary>
        public string Subgroup { get; set; } = string.Empty;
    }
}