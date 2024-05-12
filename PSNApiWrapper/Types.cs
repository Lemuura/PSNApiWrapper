using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper
{
    public class TrophyTitlesTotal
    {
        public IList<TrophyTitleData> trophyTitles { get; set; }
        public int totalItemCount { get; set; }
        public int? nextOffset { get; set; }
        public int? previousOffset { get; set; }

    }
    public class TrophyTitleData
    {
        public string npServiceName { get; set; }
        public string npCommunicationId { get; set; }
        public string trophySetVersion { get; set; }
        public string trophyTitleName { get; set; }
        public string trophyTitleDetail { get; set; }
        public string trophyTitleIconUrl { get; set; }
        public string trophyTitlePlatform { get; set; }
        public bool hasTrophyGroups { get; set; }
        public TrophyTypes definedTrophies { get; set; }
        public int progress { get; set; }
        public TrophyTypes earnedTrophies { get; set; }
        public bool hiddenFlag { get; set; }
        public DateTime lastUpdatedDateTime { get; set; }
    }

    public class TrophyTypes
    {
        public int bronze { get; set; }
        public int silver { get; set; }
        public int gold { get; set; }
        public int platinum { get; set; }
    }

    public class TrophiesForTitleData
    {
        public string trophySetVersion { get; set; }
        public bool hasTrophyGroups { get; set; }
        public DateTime lastUpdatedDateTime { get; set; }
        public IList<TrophyData> trophies { get; set; }
        public IList<TrophyData> rarestTrophies { get; set; }
        public int totalItemCount { get; set; }
        public int nextOffset { get; set; }
        public int previousOffset { get; set; }


        // Not used by the API.
        public Dictionary<int, TrophyData> cachedTrophies = new Dictionary<int, TrophyData>();
    }

    public class TrophyData
    {
        public int trophyId { get; set; }
        public bool trophyHidden { get; set; }
        public bool earned { get; set; }
        public string progress { get; set; }
        public int progressRate { get; set; }
        public DateTime progressedDateTime { get; set; }
        public DateTime earnedDateTime { get; set; }
        public string trophyType { get; set; }
        public int trophyRare { get; set; }
        public string trophyEarnedRate { get; set; }
        public string trophyName { get; set; }
        public string trophyDetail { get; set; }
        public string trophyIconUrl { get; set; }
        public string trophyGroupId { get; set; }
        public string trophyProgressTargetValue { get; set; }
        public string trophyRewardName { get; set; }
        public string trophyRewardImageUrl { get; set; }
    }

    public class TrophySummaryData
    {
        public string accountId { get; set; }
        public int trophyLevel { get; set; }
        public int trophyPoint { get; set; }
        public int trophyLevelBasePoint { get; set; }
        public int trophyLevelNextPoint { get; set; }
        public int progress { get; set; }
        public int tier { get; set; }
        public TrophyTypes earnedTrophies { get; set; }
    }

    public class TitleTrophyGroupsData
    {
        public string trophySetVersion { get; set; }
        public string trophyTitleName { get; set; }
        public string trophyTitleDetail { get; set; }
        public string trophyTitleIconUrl { get; set; }
        public string trophyTitlePlatform { get; set; }
        public TrophyTypes definedTrophies { get; set; }
        public IList<TrophyGroupsData> trophyGroups { get; set; }

    }

    public class TrophyGroupsData
    {
        public string trophyGroupId { get; set; }
        public string trophyGroupName { get; set; }
        public string trophyGroupDetail { get; set; }
        public string trophyGroupIconUrl { get; set; }
        public TrophyTypes definedTrophies { get; set; }
        public int progress { get; set; }
        public TrophyTypes earnedTrophies { get; set; }
        public DateTime lastUpdatedDateTime { get; set; }
    }

    // Trophy title summary for specific title ID
    // PS+

    public class TrophyGroupSummaryData
    {
        public string trophySetVersion { get; set; }
        public bool hiddenFlag { get; set; }
        public int progress { get; set; }
        public TrophyTypes earnedTrophies { get; set; }
        public IList<TrophyGroupsData> trophyGroups { get; set; }
        public DateTime lastUpdatedDateTime { get; set; }

    }

    public class TrophyTipRoot
    {
        public TrophyTipData data { get; set; }
    }
    public class TrophyTipData
    {
        public TrophyTipsRetrieved tipsRetrieve { get; set; }
    }
    public class TrophyTipsRetrieved
    {
        public string __typename { get; set; }
        public bool hasAccess { get; set; }
        public IList<TrophyTip> trophies { get; set; }
    }

    public class TrophyTip
    {
        public string __typename { get; set; }
        public IList<TrophyTipGroup> groups { get; set; }
        public string id { get; set; }
        public int totalGroupCount { get; set; }
        public string trophyId { get; set; }
    }

    public class TrophyTipGroup
    {
        public string __typename { get; set; }
        public string groupId { get; set; }
        public string groupName { get; set; }
        public IList<TrophyTipContent> tipContents { get; set; }
    }

    public class TrophyTipContent
    {
        public string __typename { get; set; }
        public string description { get; set; }
        public string displayName { get; set; }
        public string mediaId { get; set; }
        public string mediaType { get; set; }
        public string mediaUrl { get; set; }
        public string tipId { get; set; }
    }

    public class TrophyHintAvailabilityRoot
    {
        public TrophyHintAvailabilityData data { get; set; }
    }

    public class TrophyHintAvailabilityData
    {
        public TrophyHintAvailabilityRetrieved hintAvailabilityRetrieve { get; set; }
    }

    public class TrophyHintAvailabilityRetrieved
    {
        public string __typename { get; set; }
        public IList<TrophyInfoWithHintAvailable> trophies { get; set; }
    }

    public class TrophyInfoWithHintAvailable
    {
        public string __typename { get; set; }
        public string helpType { get; set; }
        public string id { get; set; }
        public string trophyId { get; set; }
        public string udsObjectId { get; set; }
    }

    public class SearchContextResultRoot
    {
        public SearchContextResultData data { get; set; }
    }

    public class SearchContextResultData
    {
        public UniversalContextSearch universalContextSearch { get; set; }
    }

    public class UniversalContextSearch
    {
        public string __typename { get; set; }
        public IList<UniversalContextSearchResult> results { get; set; }
    }

    public class UniversalContextSearchResult
    {
        public string __typename { get; set; }
        public string domain { get; set; }
        public string domainTitle { get; set; }
        public string next { get; set; }
        public IList<UniversalContextSearchSearchResults> searchResults { get; set; }
        public int totalResultCount { get; set; }
        public bool zeroState { get; set; }
    }

    public class UniversalContextSearchSearchResults
    {
        public string __typename { get; set; }
        public SearchHighlight highlight { get; set; }
        public string id { get; set; }
        public SearchResult result { get; set; }
    }

    public class SearchHighlight
    {
        public string __typename { get; set; }
        public object firstName { get; set; }
        public object lastName { get; set; }
        public object middleName { get; set; }
        public IList<string> onlineId { get; set; }
        public string verifiedUserName { get; set; }
    }

    public class SearchResult
    {
        public string __typename { get; set; }
        public string accountId { get; set; }
        public string avatarUrl { get; set; }
        public string displayName { get; set; }
        public IList<object> displayNameHighlighted { get; set; }
        public string firstName { get; set; }
        public string id { get; set; }
        public bool isPsPlus { get; set; }
        public string itemType { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public string onlineId { get; set; }
        public IList<string> onlineIdHighlighted { get; set; }
        public string profilePicUrl { get; set; }
        public string relationshipState { get; set; }
    }
}
