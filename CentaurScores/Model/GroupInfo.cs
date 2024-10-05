namespace CentaurScores.Model
{
    public class GroupInfo
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            GroupInfo? other = obj as GroupInfo;
            if (null == other) return false;
            return string.Equals(other.Code, Code);
        }

        public override int GetHashCode()
        {
            return $"{Code}".GetHashCode();
        }
    }
}
