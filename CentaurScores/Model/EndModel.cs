﻿namespace CentaurScores.Model
{
    public class EndModel
    {
        public int Id { get; set; }
        public int? Score { get; set; }
        public List<int?> Arrows { get; set; } = new List<int?>();
    }
}
