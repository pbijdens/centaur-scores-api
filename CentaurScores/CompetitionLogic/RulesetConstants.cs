using CentaurScores.Model;

namespace CentaurScores.CompetitionLogic
{
    /// <summary>
    /// Pre-defined values that can be freely used by all rulesets.
    /// </summary>
    public static class RulesetConstants
    {
        /// <summary></summary>
        public static string CompetitionFormatIndoor25M1P => "Indoor 25m1p, 25 pijlen";
        /// <summary></summary>
        public static string CompetitionFormatIndoor18M3P => "Indoor 18m3p, 30 pijlen";
        /// <summary></summary>
        public static string CompetitionFormatIndoor25M3P => "Indoor 25m3p, 30 pijlen";
        /// <summary></summary>
        public static string CompetitionFormatIndoorFun => "Indoor Fun";
        /// <summary></summary>
        public static string CompetitionFormatIndoorLancasterQualifier => "Indoor Lancaster 18m3p, 30 pijlen, 0-11";
        /// <summary></summary>
        public static string CompetitionFormatIndoorLancasterFinals => "Indoor Lancaster 18m3p, 15 pijlen, 0-12";

        /// <summary>
        /// Classes that are normally used in AHV Centaur.
        /// </summary>
        public static readonly List<GroupInfo> Classes = [
                new() { Code = "", Label = "Recurve"  },
                new() { Code = "C", Label = "Compound"  },
                new() { Code = "H", Label = "Hout"  },
                new() { Code = "B", Label = "Barebow"  }
            ];

        /// <summary>
        /// All officially available subclasses
        /// </summary>
        public static readonly List<GroupInfo> KHSNSubclasses = [
                new() { Code = "", Label = "Senioren"  },
                new() { Code = "O12", Label = "Onder 12"  },
                new() { Code = "O14", Label = "Onder 14"  },
                new() { Code = "O18", Label = "Onder 18"  },
                new() { Code = "O21", Label = "Onder 21"  },
                new() { Code = "50+", Label = "50+"  },
                new() { Code = "60+", Label = "60+"  },
            ];

        /// <summary>
        /// Subclasses that are normally used in AHV Centaur.
        /// </summary>
        public static readonly List<GroupInfo> CentaurSubclassesFull = [
                new() { Code = "", Label = "Senioren"  },
                new() { Code = "O12", Label = "Onder 12"  },
                new() { Code = "O14", Label = "Onder 14"  },
                new() { Code = "O18", Label = "Onder 18"  },
                new() { Code = "50+", Label = "50+"  },
            ];

        /// <summary>
        /// Subclasses that are used for the indoor competition for Centaur.
        /// </summary>
        public static readonly List<GroupInfo> CentaurSubclassesCompetities = [
                new() { Code = "", Label = "Senioren"  },
                new() { Code = "O18", Label = "Onder 18"  },
            ];

        // Standard keyboard that can be reused everywhere
        private static readonly List<ScoreButtonDefinition> StandardKeyboard1to10 = [
                          new() { Label = "10", Value = 10 },
                                  new() { Label = "9", Value = 9 },
                                  new() { Label = "8", Value = 8 },
                                  new() { Label = "7", Value = 7 },
                                  new() { Label = "6", Value = 6 },
                                  new() { Label = "5", Value = 5 },
                                  new() { Label = "4", Value = 4 },
                                  new() { Label = "3", Value = 3 },
                                  new() { Label = "2", Value = 2 },
                                  new() { Label = "1", Value = 1 },
                                  new() { Label = "Mis", Value = 0 },
                                  new() { Label = "Del", Value = null },
                                ];

        // Standard keyboard that can be reused everywhere
        private static readonly List<ScoreButtonDefinition> StandardKeyboard6to10 = [
                                  new() { Label = "10", Value = 10 },
                                  new() { Label = "9", Value = 9 },
                                  new() { Label = "8", Value = 8 },
                                  new() { Label = "7", Value = 7 },
                                  new() { Label = "6", Value = 6 },
                                  new() { Label = "Mis", Value = 0 },
                                  new() { Label = "Del", Value = null },
                                ];

        /// <summary>
        /// Valid indoor 18M target faces.
        /// </summary>
        public static readonly List<GroupInfo> Targets18M = [
            new () { Code = "40CM", Label = "40 CM FITA (1-10)" },
            new () { Code = "40CMDT", Label = "40 CM FITA Dutch Target (6-10)" },
        ];

        /// <summary>
        /// Keyboards for the specified target faces.
        /// </summary>
        public static readonly Dictionary<string, List<ScoreButtonDefinition>> Keyboards18M = new() {
                { "40CM",       StandardKeyboard1to10 },
                { "40CMDT",     StandardKeyboard6to10 },
            };

        /// <summary>
        /// Valid indoor 25M target faces.
        /// </summary>
        public static readonly List<GroupInfo> Targets25M = [
            new () { Code = "60CM", Label = "60 CM FITA (1-10)" },
            new () { Code = "60CMDT", Label = "60 CM FITA Dutch Target (6-10)" },
        ];

        /// <summary>
        /// Keyboards for the specified target faces.
        /// </summary>
        public static readonly Dictionary<string, List<ScoreButtonDefinition>> Keyboards25M = new() {
                { "60CM",       StandardKeyboard1to10 },
                { "60CMDT",     StandardKeyboard6to10 },
            };

        /// <summary>
        /// Valid indoor 18M target faces for lancaster qualifiers.
        /// </summary>
        public static readonly List<GroupInfo> TargetsLancaster = [
            new () { Code = "LAN", Label = "Lancaster (1-11)" },
            new () { Code = "LAN3S", Label = "Lancaster 3-spot (6-11)" },
        ];

        /// <summary>
        /// Keyboards for the specified target faces.
        /// </summary>
        public static readonly Dictionary<string, List<ScoreButtonDefinition>> KeyboardsLancaster = new() {
                { "LAN",        [ new() { Label = "11", Value = 11 },
                                  new() { Label = "10", Value = 10 },
                                  new() { Label = "9", Value = 9 },
                                  new() { Label = "8", Value = 8 },
                                  new() { Label = "7", Value = 7 },
                                  new() { Label = "6", Value = 6 },
                                  new() { Label = "5", Value = 5 },
                                  new() { Label = "4", Value = 4 },
                                  new() { Label = "3", Value = 3 },
                                  new() { Label = "2", Value = 2 },
                                  new() { Label = "1", Value = 1 },
                                  new() { Label = "Mis", Value = 0 },
                                  new() { Label = "Del", Value = null },
                                ]
                },
                { "LAN3S",      [ new() { Label = "11", Value = 11 },
                                  new() { Label = "10", Value = 10 },
                                  new() { Label = "9", Value = 9 },
                                  new() { Label = "8", Value = 8 },
                                  new() { Label = "7", Value = 7 },
                                  new() { Label = "6", Value = 6 },
                                  new() { Label = "Mis", Value = 0 },
                                  new() { Label = "Del", Value = null },
                                ]
                },
            };

        /// <summary>
        /// Valid indoor 18M target faces for lancaster final rounds.
        /// </summary>
        public static readonly List<GroupInfo> TargetsLancasterFinale = [
            new () { Code = "LANF", Label = "Lancaster Finale (1-12)" },
            new () { Code = "LAN3SF", Label = "Lancaster Finale 3-SPot (6-12)" },
        ];

        /// <summary>
        /// Keyboards for the specified target faces.
        /// </summary>
        public static readonly Dictionary<string, List<ScoreButtonDefinition>> KeyboardsLancasterFinale = new() {
                { "LANF",       [ new() { Label = "12", Value = 12 },
                                  new() { Label = "11", Value = 11 },
                                  new() { Label = "10", Value = 10 },
                                  new() { Label = "9", Value = 9 },
                                  new() { Label = "8", Value = 8 },
                                  new() { Label = "7", Value = 7 },
                                  new() { Label = "6", Value = 6 },
                                  new() { Label = "5", Value = 5 },
                                  new() { Label = "4", Value = 4 },
                                  new() { Label = "3", Value = 3 },
                                  new() { Label = "2", Value = 2 },
                                  new() { Label = "1", Value = 1 },
                                  new() { Label = "Mis", Value = 0 },
                                  new() { Label = "Del", Value = null },
                                ]
                },
                { "LAN3SF",     [ new() { Label = "12", Value = 12 },
                                  new() { Label = "11", Value = 11 },
                                  new() { Label = "10", Value = 10 },
                                  new() { Label = "9", Value = 9 },
                                  new() { Label = "8", Value = 8 },
                                  new() { Label = "7", Value = 7 },
                                  new() { Label = "6", Value = 6 },
                                  new() { Label = "Mis", Value = 0 },
                                  new() { Label = "Del", Value = null },
                                ]
                },
            };

        /// <summary>
        /// Valid indoor FUN targets.
        /// </summary>
        public static readonly List<GroupInfo> TargetsFun = [
            new () { Code = "40CM", Label = "40 CM FITA (1-10)" },
            new () { Code = "40CMDT", Label = "40 CM FITA Dutch Target (6-10)" },
            new () { Code = "60CM", Label = "60 CM FITA (1-10)" },
            new () { Code = "60CMDT", Label = "60 CM FITA Dutch Target (6-10)" },
            new () { Code = "80CM", Label = "80 CM FITA (1-10)" },
            new () { Code = "80CMDT", Label = "80 CM FITA Dutch Target (6-10)" },
            new () { Code = "FUN1", Label = "Funblazoen (1-10)" },
        ];

        /// <summary>
        /// Keyboards for the specified target faces.
        /// </summary>
        public static readonly Dictionary<string, List<ScoreButtonDefinition>> KeyboardsFun = new() {
                { "40CM",       StandardKeyboard1to10 },
                { "40CMDT",     StandardKeyboard6to10 },
                { "60CM",       StandardKeyboard1to10 },
                { "60CMDT",     StandardKeyboard6to10 },
                { "80CM",       StandardKeyboard1to10 },
                { "80CMDT",     StandardKeyboard6to10 },
                { "FUN1",       StandardKeyboard1to10 },
            };
    }
}
