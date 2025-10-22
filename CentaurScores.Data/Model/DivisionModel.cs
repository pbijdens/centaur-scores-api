namespace CentaurScores.Model;

/// <summary>
/// Represents the divisions that participants can compete in, could be as simple as
/// H, A, B, C, D (we'll order these by Code), or could be more complex like
/// Junioren Hoofdklasse, Junioren 1e Klasse, Junioren 2e Klasse, Junioren 3e Klasse,
/// Masters Hoofdklasse, Masters 1e Klasse, Masters 2e Klasse, Masters 3e Klasse, etc.
/// </summary>
public class DivisionModel : GroupInfo
{
    // inherit Id, Label, Code from parent, only using Code and Label
}

