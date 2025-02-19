using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace CentaurScores.Model
{
    public class HeadToHeadInfoEntry
    {
        /// <summary>
        /// Participant ID of the opponent's participant record
        /// </summary>
        public int OpponentId { get; set; } = -1;

        /// <summary>
        /// Start position where the participant entered the finals
        /// </summary>
        public int InitialPosition { get; set; } = -1;

        /// <summary>
        /// True if the participanty has been declared the winner
        /// </summary>
        public bool IsWinner { get; set; } = false;

        /// <summary>
        /// The number of the bracket, 1-based
        /// </summary>
        public int Bracket { get; set; } = -1;

        /// <summary>
        /// Either 0 or 1, indicates the position in the bracket (i.e. first or second)
        /// </summary>
        public int Position { get; set; } = 0;

        /// <summary>
        /// True if the head2head match uses set scoring, false otherwise. This is stored on
        /// a per-archer basis because a developer was too lazy to add special configuration
        /// to the match to allow storing a per-group setting for just this field.
        /// </summary>
        public bool IsSetScored { get; set; } = true;
    }
}
