namespace CentaurScores.Model
{
    /// <summary>
    /// Key value pair representing a parameter that can be passed to a ruleset
    /// </summary>
    public class RulesetParameterInfo
    {
        /// <summary>
        /// The key of the parameter.
        /// </summary>
        public required string Key { get; set; }
        /// <summary>
        /// The value type, is either "number" or "string" (TODO: To be determined, feature in design.)
        /// </summary>
        public required string ValueType { get; set; }
    }
}