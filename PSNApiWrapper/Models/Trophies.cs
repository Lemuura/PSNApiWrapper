using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper.Models
{
    public class Trophies : Pagination
    {
        /// <summary>
        /// The current version of the trophy set
        /// </summary>
        public string trophySetVersion { get; set; }
        /// <summary>
        /// True if this title has additional trophy groups
        /// </summary>
        public bool hasTrophyGroups { get; set; }
        /// <summary>
        /// Date most recent trophy earned for the title
        /// </summary>
        public DateTime lastUpdatedDateTime { get; set; }
        /// <summary>
        /// Individual object for each trophy
        /// </summary>
        public IList<Trophy> trophies { get; set; }
        /// <summary>
        /// Individual object for each trophy.
        /// Returns the trophy where earned is true with the lowest trophyEarnedRate.
        /// Returns nothing if no trophies are earned.
        /// </summary>
        public IList<Trophy> rarestTrophies { get; set; }
        


        // Not used by the API.
        public Dictionary<int, Trophy> cachedTrophies = new Dictionary<int, Trophy>();
    }

    public class Trophy
    {
        /// <summary>
        /// Unique ID for this trophy (unique within the title and not just the group)
        /// </summary>
        public int trophyId { get; set; }
        /// <summary>
        /// True if this is a secret trophy (ie. further details are not displayed by default unless earned)
        /// </summary>
        public bool trophyHidden { get; set; }
        /// <summary>
        /// True if this trophy has been earned.
        /// </summary>
        public bool earned { get; set; }
        /// <summary>
        /// PS5 titles only.
        /// If the trophy tracks progress towards unlock this is number of steps currently completed (ie. 73/300).
        /// Only returned if the trophy tracks progress and earned is false.
        /// </summary>
        public string progress { get; set; }
        /// <summary>
        /// PS5 titles only.
        /// If the trophy tracks progress towards unlock this is the current percentage complete.
        /// Only returned if the trophy tracks progress and earned is false.
        /// </summary>
        public int progressRate { get; set; }
        /// <summary>
        /// PS5 titles only.
        /// If the trophy tracks progress towards unlock, and some progress has been made, then this returns the date progress was last updated.
        /// Only returned if the trophy tracks progress, some progress has been made, and earned is false.
        /// </summary>
        public DateTime progressedDateTime { get; set; }
        /// <summary>
        /// Date trophy was earned.
        /// Only returned if earned is true.
        /// </summary>
        public DateTime earnedDateTime { get; set; }
        /// <summary>
        /// Type of the trophy
        /// </summary>
        public string trophyType { get; set; }
        /// <summary>
        /// Rarity of the trophy. 0 = Ultra Rare, 1 = Very Rare, 2 = Rare, 3 = Common.
        /// </summary>
        public int trophyRare { get; set; }
        /// <summary>
        /// Percentage of all users who have earned the trophy.
        /// </summary>
        public string trophyEarnedRate { get; set; }
        /// <summary>
        /// Name of the trophy
        /// </summary>
        public string trophyName { get; set; }
        /// <summary>
        /// Description of the trophy
        /// </summary>
        public string trophyDetail { get; set; }
        /// <summary>
        /// URL for the graphic associated with the trophy
        /// </summary>
        public string trophyIconUrl { get; set; }
        /// <summary>
        /// ID of the trophy group this trophy belongs to
        /// </summary>
        public string trophyGroupId { get; set; }
        /// <summary>
        /// PS5 titles only.
        /// If the trophy tracks progress towards unlock this is the total required.
        /// Only returned if trophy tracks progress
        /// </summary>
        public string trophyProgressTargetValue { get; set; }
        /// <summary>
        /// PS5 titles only.
        /// Name of the reward earning the trophy grants.
        /// Only returned if the trophy has a reward associated with it.
        /// </summary>
        public string trophyRewardName { get; set; }
        /// <summary>
        /// PS5 titles only.
        /// URL for the graphic associated with the reward.
        /// Only returned if the trophy has a reward associated with it.
        /// </summary>
        public string trophyRewardImageUrl { get; set; }
    }
}
