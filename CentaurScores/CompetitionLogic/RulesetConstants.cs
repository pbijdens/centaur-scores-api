using CentaurScores.Model;

namespace CentaurScores.CompetitionLogic
{
    public static class RulesetConstants
    {
        public static readonly List<GroupInfo> Classes = [
                new() { Code = "", Label = "Recurve"  },
                new() { Code = "C", Label = "Compound"  },
                new() { Code = "H", Label = "Hout, Barebow"  }
            ];

        public static readonly List<GroupInfo> KHSNSubclasses = [
                new() { Code = "", Label = "Senioren"  },
                new() { Code = "O12", Label = "Onder 12"  },
                new() { Code = "O14", Label = "Onder 14"  },
                new() { Code = "O18", Label = "Onder 18"  },
                new() { Code = "O21", Label = "Onder 21"  },
                new() { Code = "50+", Label = "50+"  },
                new() { Code = "60+", Label = "60+"  },
            ];

        public static readonly List<GroupInfo> CentaurSubclassesFull = [
                new() { Code = "", Label = "Senioren"  },
                new() { Code = "O12", Label = "Onder 12"  },
                new() { Code = "O14", Label = "Onder 14"  },
                new() { Code = "O18", Label = "Onder 18"  },
                new() { Code = "50+", Label = "50+"  },
            ];

        public static readonly List<GroupInfo> CentaurSubclassesCompetities = [
                new() { Code = "", Label = "Senioren"  },
                new() { Code = "O18", Label = "Onder 18"  },
            ];

        public static List<GroupInfo> Targets18M = [
            new () { Code = "40CM", Label = "40 CM FITA (1-10)" },
            new () { Code = "40CMDT", Label = "40 CM FITA Dutch Target (6-10)" },
        ];

        public static List<GroupInfo> Targets25M = [
            new () { Code = "60CM", Label = "60 CM FITA (1-10)" },
            new () { Code = "60CMDT", Label = "60 CM FITA Dutch Target (6-10)" },
        ];

        public static List<GroupInfo> TargetsLancaster = [
            new () { Code = "LAN", Label = "Lancaster (1-11)" },
            new () { Code = "LAN3S", Label = "Lancaster 3-spot (6-11)" },
        ];

        public static List<GroupInfo> TargetsLancasterFinale = [
            new () { Code = "LANF", Label = "Lancaster Finale (1-12)" },
            new () { Code = "LAN3SF", Label = "Lancaster Finale 3-SPot (6-11)" },
        ];

        public static Dictionary<string, List<ScoreButtonDefinition>> Keyboards18M = new() {
                { "40CM",       [ new() { Label = "10", Value = 10 },
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
                { "40CMDT",     [ new() { Label = "10", Value = 10 },
                                  new() { Label = "9", Value = 9 },
                                  new() { Label = "8", Value = 8 },
                                  new() { Label = "7", Value = 7 },
                                  new() { Label = "6", Value = 6 },
                                  new() { Label = "Mis", Value = 0 },
                                  new() { Label = "Del", Value = null },
                                ]
                },
            };

        public static Dictionary<string, List<ScoreButtonDefinition>> Keyboards25M = new() {
                { "60CM",       [ new() { Label = "10", Value = 10 },
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
                { "60CMDT",     [ new() { Label = "10", Value = 10 },
                                  new() { Label = "9", Value = 9 },
                                  new() { Label = "8", Value = 8 },
                                  new() { Label = "7", Value = 7 },
                                  new() { Label = "6", Value = 6 },
                                  new() { Label = "Mis", Value = 0 },
                                  new() { Label = "Del", Value = null },
                                ]
                },
            };

        public static Dictionary<string, List<ScoreButtonDefinition>> KeyboardsLancaster = new() {
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

        public static Dictionary<string, List<ScoreButtonDefinition>> KeyboardsLancasterFinale = new() {
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
    }
}
