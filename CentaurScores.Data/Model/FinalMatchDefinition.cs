using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentaurScores.Model
{
    /// <summary>
    /// Used to define a finals match on the API
    /// </summary>
    public class FinalMatchDefinition
    {
        /// <summary>
        /// Name of the match that is to be created.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// The groups that participate in this final.
        /// </summary>
        public List<FinalMatchGroupDefinition> Groups { get; set; } = [];
    }
}
