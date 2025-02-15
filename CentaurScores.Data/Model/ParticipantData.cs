using System.Text.RegularExpressions;

namespace CentaurScores.Model
{
    /// <summary>
    /// Metadata for a single participant. Includes all information and logic required to detrmine if two 
    /// participant records for two matches represent one and the same archer.
    /// </summary>
    public partial class ParticipantData
    {
        /// <summary>
        /// NAme as entered on the score sheet.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Group in which the archer participates.
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// Subgroup in which the archer participates.
        /// </summary>
        public string Subgroup { get; set; } = string.Empty;

        /// <summary>
        /// Normalized version of the archer's name making comparison slightly easier.
        /// </summary>
        public string Normalizedname => NormalizationRegex().Replace($"{Name}".TrimEnd('*').ToLowerInvariant(), (c) => "");

        /// <summary>
        /// ID of the archer in the competition's potential member list. Same Id means it's the same archer.
        /// </summary>
        public int Id { get; set; } = -1;

        /// <summary>Constructor</summary>
        public ParticipantData()
        {
        }

        /// <summary>Constructor</summary>
        public ParticipantData(int? id, string name, string group, string subgroup)
        {
            Id = id.HasValue && id >= 0 ? id.Value : -1;
            Name = name;
            Group = group;
            Subgroup = subgroup;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not ParticipantData p) return false;
            if (!string.Equals(p.Subgroup, Subgroup) || !string.Equals(p.Group, Group)) return false;
            if (p.Id >= 0 && Id >= 0 && p.Id == Id) return true;
            return string.Equals(Normalizedname, p.Normalizedname);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // if hashcodes are different then the records are guaranteed to be different
            // because of the complex logic, we're robbing the system from its hashcodes
            // and are forcing it to compare all objects.
            return 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Name} ({Id}) @{Normalizedname}";
        }

        [GeneratedRegex(@"[^\p{L}]", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-US")]
        private static partial Regex NormalizationRegex();
    }
}
