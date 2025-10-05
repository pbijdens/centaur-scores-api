namespace CentaurScores.Model;

/// <summary>
/// Represents the discipline beign participated in, e.g. Recurve, Barebow, Compound, Longbow, Traditional.
/// These disciplines, together with the generic CompetitionFormat form the keys of the PersonalBest lists.
/// So an archer can have a personal best for e.g. Barebow: "Indoor, 18 meters, 3 arrows per end, 10 ends"
/// </summary>
public class DisciplineModel : GroupInfo
{
    // inherit Id, Label, Code from parent, only using Code and Label
}

