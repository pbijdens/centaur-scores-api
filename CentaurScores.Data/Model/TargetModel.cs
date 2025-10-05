namespace CentaurScores.Model;

/// <summary>
/// Represents a target that is used in specific competition formats. Per competition format, 
/// discipline and division there typically are spefcific targets that are used.
/// </summary>
public class TargetModel : GroupInfo
{
    public IEnumerable<ScoreButtonDefinition> Keyboard { get; set; } = [];
}

