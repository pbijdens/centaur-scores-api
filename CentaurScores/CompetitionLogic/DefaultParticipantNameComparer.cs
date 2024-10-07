using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CentaurScores.CompetitionLogic
{
    public class DefaultParticipantNameComparer : IParticipantNameComparer
    {
        public bool Equals(string? name1, string? name2)
        {
            string name1a = Regex.Replace($"{name1}".ToLowerInvariant(), @"[^\p{L}]", (c) => "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            string name2a = Regex.Replace($"{name2}".ToLowerInvariant(), @"[^\p{L}]", (c) => "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return string.Equals(name1a, name2a);
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            string name1a = Regex.Replace($"{obj}".ToLowerInvariant(), @"[^\p{L}]", (c) => "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return name1a.GetHashCode();
        }
    }
}
