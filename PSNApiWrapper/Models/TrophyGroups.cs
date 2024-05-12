using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper.Models
{
    public class TrophyGroups : TrophyTitleBase
    {
        /// <summary>
        /// Total number of trophies for the title by type
        /// </summary>
        public TrophyTypes definedTrophies { get; set; }
        /// <summary>
        /// Individual object for each trophy group returned
        /// </summary>
        public IList<TrophyGroup> trophyGroups { get; set; }

    }

    public class TrophyGroup
    {
        /// <summary>
        /// ID for the trophy group (all titles have default, additional groups are 001 incrementing)
        /// </summary>
        public string trophyGroupId { get; set; }
        /// <summary>
        /// Trophy group name
        /// </summary>
        public string trophyGroupName { get; set; }
        /// <summary>
        /// Trophy group description.
        /// PS3, PS4 and PS Vita titles only
        /// </summary>
        public string trophyGroupDetail { get; set; }
        /// <summary>
        /// URL of the icon for the trophy group
        /// </summary>
        public string trophyGroupIconUrl { get; set; }
        /// <summary>
        /// Number of trophies for the trophy group by type
        /// </summary>
        public TrophyTypes definedTrophies { get; set; }
        /// <summary>
        /// Percentage of trophies earned for group
        /// </summary>
        public int progress { get; set; }
        /// <summary>
        /// Number of trophies for the group which have been earned by type
        /// </summary>
        public TrophyTypes earnedTrophies { get; set; }
        /// <summary>
        /// Date most recent trophy earned for the group
        /// </summary>
        public DateTime lastUpdatedDateTime { get; set; }
    }

    public class TrophyGroupSummaryData
    {
        /// <summary>
        /// The current version of the trophy set
        /// </summary>
        public string trophySetVersion { get; set; }
        /// <summary>
        /// Title has been hidden on the accounts trophy list.
        /// Authenticating account only.
        /// Title will not be returned if it has been hidden on another account.
        /// </summary>
        public bool hiddenFlag { get; set; }
        /// <summary>
        /// Percentage of trophies earned for the title
        /// </summary>
        public int progress { get; set; }
        /// <summary>
        /// Number of trophies for the title which have been earned by type
        /// </summary>
        public TrophyTypes earnedTrophies { get; set; }
        /// <summary>
        /// Individual object for each trophy group returned
        /// </summary>
        public IList<TrophyGroup> trophyGroups { get; set; }
        /// <summary>
        /// Date most recent trophy earned for the title
        /// </summary>
        public DateTime lastUpdatedDateTime { get; set; }

    }
}
