using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper.Models
{
    public class TrophyTitles : Pagination
    {
        /// <summary>
        /// Individual object for each title returned
        /// </summary>
        public IList<TrophyTitle> trophyTitles { get; set; }
    }

    public class TrophyTitle : TrophyTitleBase
    {
        /// <summary>
        /// "trophy" for PS3, PS4, or PS Vita platforms.
        /// "trophy2" for PS5 platform.
        /// </summary>
        public string npServiceName { get; set; }
        /// <summary>
        /// Unique ID of the title; later required for requesting detailed trophy information for this title
        /// </summary>
        public string npCommunicationId { get; set; }
        /// <summary>
        /// True if the title has multiple groups of trophies 
        /// (eg. DLC trophies which are separate from the main trophy list)
        /// </summary>
        public bool hasTrophyGroups { get; set; }
        /// <summary>
        /// Number of trophies for the title by type
        /// </summary>
        public TrophyTypes definedTrophies { get; set; }
        /// <summary>
        /// Percentage of trophies earned for the title
        /// </summary>
        public int progress { get; set; }
        /// <summary>
        /// Number of trophies for the title which have been earned by type
        /// </summary>
        public TrophyTypes earnedTrophies { get; set; }
        /// <summary>
        /// Title has been hidden on the accounts trophy list.
        /// Authenticating account only.
        /// Title will not be returned if it has been hidden on another account.
        /// </summary>
        public bool hiddenFlag { get; set; }
        /// <summary>
        /// Date most recent trophy earned for the title
        /// </summary>
        public DateTime lastUpdatedDateTime { get; set; }
    }
}
