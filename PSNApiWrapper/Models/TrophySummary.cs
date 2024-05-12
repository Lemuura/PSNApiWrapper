using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper.Models
{
    public class TrophySummary
    {
        /// <summary>
        /// The ID of the account being accessed
        /// </summary>
        public string accountId { get; set; }
        /// <summary>
        /// The overall trophy level
        /// </summary>
        public int trophyLevel { get; set; }
        /// <summary>
        /// The total point value of trophies earned
        /// </summary>
        public int trophyPoint { get; set; }
        /// <summary>
        /// Points required to reach the current trophy level
        /// </summary>
        public int trophyLevelBasePoint { get; set; }
        /// <summary>
        /// Points required to reach the next trophy level
        /// </summary>
        public int trophyLevelNextPoint { get; set; }
        /// <summary>
        /// Percentage process towards the next trophy level
        /// </summary>
        public int progress { get; set; }
        /// <summary>
        /// The tier this trophy level is in
        /// </summary>
        public int tier { get; set; }
        /// <summary>
        /// Number of trophies which have been earned by type
        /// </summary>
        public TrophyTypes earnedTrophies { get; set; }
    }
}
