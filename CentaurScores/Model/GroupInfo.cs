namespace CentaurScores.Model
{
    /// <summary>
    /// Generic information on a group identified by a code.
    /// </summary>
    public class GroupInfo
    {
        /// <summary>
        /// Not used.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Label used for rendering.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Code for the group. An empty string represents a valid code (this is used intentionally!)
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not GroupInfo other) return false;
            return string.Equals(other.Code, Code);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return $"{Code}".GetHashCode();
        }
    }
}
