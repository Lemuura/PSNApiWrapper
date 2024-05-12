using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper.Models
{
    public class SearchContextResultRoot
    {
        public SearchContextResultData data { get; set; }
    }

    public class SearchContextResultData
    {
        public UniversalContextSearchResponse universalContextSearch { get; set; }
    }

    public class UniversalContextSearchResponse
    {
        public string __typename { get; set; }
        public IList<UniversalDomainSearchResponse> results { get; set; }
    }

    public class UniversalDomainSearchResponse
    {
        public string __typename { get; set; }
        public string domain { get; set; }
        public string domainTitle { get; set; }
        public string next { get; set; }
        public IList<SearchResultItem> searchResults { get; set; }
        public int totalResultCount { get; set; }
        public bool zeroState { get; set; }
    }

    public class SearchResultItem
    {
        public string __typename { get; set; }
        public PlayerHighlight highlight { get; set; }
        public string id { get; set; }
        public Player result { get; set; }
    }

    public class PlayerHighlight
    {
        public string __typename { get; set; }
        public object firstName { get; set; }
        public object lastName { get; set; }
        public object middleName { get; set; }
        public IList<string> onlineId { get; set; }
        public string verifiedUserName { get; set; }
    }

    public class Player
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
