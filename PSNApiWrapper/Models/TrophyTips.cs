using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper.Models
{
    public class TrophyTipsRoot
    {
        public TrophyTipData data { get; set; }
    }
    public class TrophyTipData
    {
        public Tips tipsRetrieve { get; set; }
    }
    public class Tips
    {
        public string __typename { get; set; }
        /// <summary>
        /// Returns false if authenticating account does not have a PS+ subscription
        /// </summary>
        public bool hasAccess { get; set; }
        /// <summary>
        /// Contains the requested trophies
        /// </summary>
        public IList<TrophyTip> trophies { get; set; }
    }

    public class TrophyTip
    {
        public string __typename { get; set; }
        public IList<TipGroup> groups { get; set; }
        /// <summary>
        /// Combination of the title ID and individual trophy ID (e.g. "NPWR20188_00::18")
        /// </summary>
        public string id { get; set; }
        public int totalGroupCount { get; set; }
        /// <summary>
        /// ID of the trophy
        /// </summary>
        public string trophyId { get; set; }
    }

    public class TipGroup
    {
        public string __typename { get; set; }
        public string groupId { get; set; }
        public string groupName { get; set; }
        /// <summary>
        /// Contains the Game Help content.
        /// Should the help contain multiple steps, each is returned as a separate TipContent object.
        /// </summary>
        public IList<TipContent> tipContents { get; set; }
    }

    public class TipContent
    {
        public string __typename { get; set; }
        public string description { get; set; }
        public string displayName { get; set; }
        public string mediaId { get; set; }
        public string mediaType { get; set; }
        public string mediaUrl { get; set; }
        public string tipId { get; set; }
    }
}
