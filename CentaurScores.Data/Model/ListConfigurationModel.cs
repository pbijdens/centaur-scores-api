using CentaurScores.CompetitionLogic;

namespace CentaurScores.Model;

public class ListConfigurationModel
{
    public static ListConfigurationModel NationalDefaults = new ListConfigurationModel
    {
        Name = "Standaard NL",
        Disciplines = [
            new DisciplineModel { Code = "R", Label = "Recurve" },
            new DisciplineModel { Code = "C", Label = "Compound" },
            new DisciplineModel { Code = "B", Label = "Barebow" },
            new DisciplineModel { Code = "H", Label = "Traditioneel" },
            new DisciplineModel { Code = "L", Label = "Longbow" },
            ],
        Divisions = [
            new DivisionModel { Code = "O120", Label = "Onder 12 Hoofdklasse" },
            new DivisionModel { Code = "O121", Label = "Onder 12 Klasse 1" },
            new DivisionModel { Code = "O122", Label = "Onder 12 Klasse 2" },
            new DivisionModel { Code = "O123", Label = "Onder 12 Klasse 3" },
            new DivisionModel { Code = "O124", Label = "Onder 12 Klasse 4" },
            new DivisionModel { Code = "O125", Label = "Onder 12 Klasse 5" },
            new DivisionModel { Code = "O126", Label = "Onder 12 Klasse 6" },

            new DivisionModel { Code = "O140", Label = "Onder 14 Hoofdklasse" },
            new DivisionModel { Code = "O141", Label = "Onder 14 Klasse 1" },
            new DivisionModel { Code = "O142", Label = "Onder 14 Klasse 2" },
            new DivisionModel { Code = "O143", Label = "Onder 14 Klasse 3" },
            new DivisionModel { Code = "O144", Label = "Onder 14 Klasse 4" },
            new DivisionModel { Code = "O145", Label = "Onder 14 Klasse 5" },
            new DivisionModel { Code = "O146", Label = "Onder 14 Klasse 6" },

            new DivisionModel { Code = "O160", Label = "Onder 16 Hoofdklasse" },
            new DivisionModel { Code = "O161", Label = "Onder 16 Klasse 1" },
            new DivisionModel { Code = "O162", Label = "Onder 16 Klasse 2" },
            new DivisionModel { Code = "O163", Label = "Onder 16 Klasse 3" },
            new DivisionModel { Code = "O164", Label = "Onder 16 Klasse 4" },
            new DivisionModel { Code = "O165", Label = "Onder 16 Klasse 5" },
            new DivisionModel { Code = "O166", Label = "Onder 16 Klasse 6" },

            new DivisionModel { Code = "O180", Label = "Onder 18 Hoofdklasse" },
            new DivisionModel { Code = "O181", Label = "Onder 18 Klasse 1" },
            new DivisionModel { Code = "O182", Label = "Onder 18 Klasse 2" },
            new DivisionModel { Code = "O183", Label = "Onder 18 Klasse 3" },
            new DivisionModel { Code = "O184", Label = "Onder 18 Klasse 4" },
            new DivisionModel { Code = "O185", Label = "Onder 18 Klasse 5" },
            new DivisionModel { Code = "O186", Label = "Onder 18 Klasse 6" },

            new DivisionModel { Code = "O210", Label = "Onder 21 Hoofdklasse" },
            new DivisionModel { Code = "O211", Label = "Onder 21 Klasse 1" },
            new DivisionModel { Code = "O212", Label = "Onder 21 Klasse 2" },
            new DivisionModel { Code = "O213", Label = "Onder 21 Klasse 3" },
            new DivisionModel { Code = "O214", Label = "Onder 21 Klasse 4" },
            new DivisionModel { Code = "O215", Label = "Onder 21 Klasse 5" },
            new DivisionModel { Code = "O216", Label = "Onder 21 Klasse 6" },

            new DivisionModel { Code = "S0", Label = "Senioren Hoofdklasse" },
            new DivisionModel { Code = "S1", Label = "Senioren Klasse 1" },
            new DivisionModel { Code = "S2", Label = "Senioren Klasse 2" },
            new DivisionModel { Code = "S3", Label = "Senioren Klasse 3" },
            new DivisionModel { Code = "S4", Label = "Senioren Klasse 4" },
            new DivisionModel { Code = "S5", Label = "Senioren Klasse 5" },
            new DivisionModel { Code = "S6", Label = "Senioren Klasse 6" },

            new DivisionModel { Code = "50+0", Label = "50+ Hoofdklasse" },
            new DivisionModel { Code = "50+1", Label = "50+ Klasse 1" },
            new DivisionModel { Code = "50+2", Label = "50+ Klasse 2" },
            new DivisionModel { Code = "50+3", Label = "50+ Klasse 3" },
            new DivisionModel { Code = "50+4", Label = "50+ Klasse 4" },
            new DivisionModel { Code = "50+5", Label = "50+ Klasse 5" },
            new DivisionModel { Code = "50+6", Label = "50+ Klasse 6" },

            new DivisionModel { Code = "60+0", Label = "60+ Hoofdklasse" },
            new DivisionModel { Code = "60+1", Label = "60+ Klasse 1" },
            new DivisionModel { Code = "60+2", Label = "60+ Klasse 2" },
            new DivisionModel { Code = "60+3", Label = "60+ Klasse 3" },
            new DivisionModel { Code = "60+4", Label = "60+ Klasse 4" },
            new DivisionModel { Code = "60+5", Label = "60+ Klasse 5" },
            new DivisionModel { Code = "60+6", Label = "60+ Klasse 6" },
            ],
        Targets = [
            new () { Code = "40CM", Label = "40 CM FITA (1-10)", Keyboard = RulesetConstants.StandardKeyboard1to10 },
            new () { Code = "40CMDT", Label = "40 CM FITA Dutch Target (6-10)", Keyboard = RulesetConstants.StandardKeyboard6to10 },
            new () { Code = "60CM", Label = "60 CM FITA (1-10)", Keyboard = RulesetConstants.StandardKeyboard1to10 },
            new () { Code = "60CMDT", Label = "60 CM FITA Dutch Target (6-10)", Keyboard = RulesetConstants.StandardKeyboard6to10 },
            ],
        CompetitionFormats = RulesetConstants.DefaultCompetitionFormats.ToList(),
    };

    public static ListConfigurationModel CentaurIndoorDefaults = new ListConfigurationModel
    {
        Name = "Centaur Indoor",
        Disciplines = [
            new DisciplineModel { Code = "R", Label = "Recurve" },
            new DisciplineModel { Code = "C", Label = "Compound" },
            new DisciplineModel { Code = "B", Label = "Barebow" },
            new DisciplineModel { Code = "H", Label = "Hout" },
            ],
        Divisions = [
            new DivisionModel { Code = "O120", Label = "Onder 12 Hoofdklasse" },
            new DivisionModel { Code = "O121", Label = "Onder 12 Klasse A" },
            new DivisionModel { Code = "O122", Label = "Onder 12 Klasse B" },
            new DivisionModel { Code = "O123", Label = "Onder 12 Klasse C" },

            new DivisionModel { Code = "O210", Label = "Onder 21 Hoofdklasse" },
            new DivisionModel { Code = "O211", Label = "Onder 21 Klasse A" },
            new DivisionModel { Code = "O212", Label = "Onder 21 Klasse B" },
            new DivisionModel { Code = "O213", Label = "Onder 21 Klasse C" },

            new DivisionModel { Code = "S0", Label = "Senioren Hoofdklasse" },
            new DivisionModel { Code = "S1", Label = "Senioren Klasse A" },
            new DivisionModel { Code = "S2", Label = "Senioren Klasse B" },
            new DivisionModel { Code = "S3", Label = "Senioren Klasse C" },
            ],
        Targets = [
            new () { Code = "40CM", Label = "40 CM FITA (1-10)", Keyboard = RulesetConstants.StandardKeyboard1to10 },
            new () { Code = "40CMDT", Label = "40 CM FITA Dutch Target (6-10)", Keyboard = RulesetConstants.StandardKeyboard6to10 },
            new () { Code = "60CM", Label = "60 CM FITA (1-10)", Keyboard = RulesetConstants.StandardKeyboard1to10 },
            new () { Code = "60CMDT", Label = "60 CM FITA Dutch Target (6-10)", Keyboard = RulesetConstants.StandardKeyboard6to10 },
            ],
        CompetitionFormats = RulesetConstants.DefaultCompetitionFormats.ToList(),
    };

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// These are the only competition formats supported.
    /// </summary>
    public List<string> CompetitionFormats { get; set; } = [];
    public List<DisciplineModel> Disciplines { get; set; } = [];
    public List<DivisionModel> Divisions { get; set; } = [];
    public List<TargetModel> Targets { get; set; } = [];
}

