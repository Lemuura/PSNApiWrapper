using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using PSNApiWrapper.Models;

namespace PSNApiWrapper
{
    public class TrophyClient
    {
        public static readonly Uri TrophyApiUri = new Uri("https://m.np.playstation.com/api/trophy/v1/");

        private PSNClient PSN;

        public Dictionary<string, TrophyTitle> CachedPS5Titles = new Dictionary<string, TrophyTitle>();
        public Dictionary<string, TrophyTitle> CachedOtherTitles = new Dictionary<string, TrophyTitle>();
        public Dictionary<string, Trophies> CachedTitleTrophies = new Dictionary<string, Trophies>();

        public TrophyClient(PSNClient baseClient)
        {
            this.PSN = baseClient;
        }

        private async Task<HttpContent> SendRequest(Uri BaseUri, string RelativeUri, int? limit = null, int? offset = null, string serviceName = null)
        {
            List<object> requestParams = new List<object>();
            if (serviceName != null)
                requestParams.Add("npServiceName=" + serviceName);
            if (limit != null)
                requestParams.Add("limit=" + limit);
            if (offset != null)
                requestParams.Add("offset=" + offset);

            return await PSN.SendGetRequest(BaseUri, RelativeUri + "?" + string.Join("&", requestParams));
        }

        /// <summary>
        /// Retrieves a list of the titles associated with an account, and a summary of trophies earned from them.
        /// The results are presented in order of the "lastUpdatedDateTime" for the title, so the first result will be the title for which a trophy was recently earned, 
        /// or synced for the first time in the case of a game with 0% progress.
        /// </summary>
        /// <param name="accountId">The account whos trophy list is being accessed. Use "me" for the authenticating account</param>
        /// <param name="limit">Limit the number of titles returned</param>
        /// <param name="offset">Returns title data from this result onwards</param>
        public async Task<TrophyTitles> GetTrophyTitlesForUser(string accountId = "me", int? limit = null, int? offset = null)
        {
            var content = await SendRequest(TrophyApiUri, $"users/{accountId}/trophyTitles", limit, offset);
            TrophyTitles titles = await content.ReadFromJsonAsync<TrophyTitles>();

            Console.Write("Titles found: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(titles.totalItemCount.ToString() + "\n");
            Console.ResetColor();
            for (int i = 0; i < titles.totalItemCount; i++)
            {
                if (titles.nextOffset != null && titles.totalItemCount > titles.nextOffset)
                {
                    PSN.WriteLine("Total amount of titles is bigger than the limit. Please provide a limit of up to 800. \n" +
                        "Support for pagination hasn't been added yet.\n", ConsoleColor.Red);
                    break;
                }
                var t = titles.trophyTitles[i];

                string adjustedName = SimplifyString(t.trophyTitleName);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[" + (i + 1) + "] " + t.trophyTitleName);
                Console.ResetColor();
                Console.WriteLine("\tCommunication ID: " + t.npCommunicationId);
                Console.WriteLine("\tService Name: " + t.npServiceName);
                Console.WriteLine("\tSet Version: " + t.trophySetVersion);
                Console.WriteLine("\tPlatform: " + t.trophyTitlePlatform + "\n");

                try
                {
                    if (t.npServiceName.Equals("trophy", StringComparison.OrdinalIgnoreCase) && !CachedOtherTitles.ContainsKey(adjustedName))
                        CachedOtherTitles.Add(adjustedName, t);
                    if (t.npServiceName.Equals("trophy2", StringComparison.OrdinalIgnoreCase) && !CachedPS5Titles.ContainsKey(adjustedName))
                        CachedPS5Titles.Add(adjustedName, t);
                }
                catch (Exception e)
                {

                    throw;
                }

            }

            return titles;

        }

        public string SimplifyString(string str)
        {
            char[] specialChar = new char[] { '™', '®', '[', ']' };
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (!specialChar.Contains(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().ToLower();
        }

        /// <summary>
        /// Retrieves the individual trophy detail of a single - or all - trophy groups for a title. 
        /// A title can have multiple groups of trophies (a "default" group which all titles have, and additional groups named "001" incrementing for each additional group.
        /// To retrieve trophies from all groups within a title (ie. the full trophy set) then "groupId" should be set to "all".
        /// </summary>
        /// <param name="npCommId">Unique ID of the title</param>
        /// <param name="groupId">"all" to return all trophies for the title, otherwise restrict results to a specific trophy group</param>
        /// <param name="serviceName">"trophy" for PS3, PS4, or PS Vita platforms
        /// "trophy2" for the PS5 platform</param>
        /// <param name="limit">Limit the number of trophies returned. If no limit is specified all trophies will be returned.</param>
        /// <param name="offset">Returns trophy data from this result onwards</param>
        public async Task<Trophies> GetTrophiesForTitle(string npCommId, string groupId = "all", string serviceName = "trophy2", int? limit = null, int? offset = null)
        {
            if (groupId.Equals("all", StringComparison.OrdinalIgnoreCase) && CachedTitleTrophies.TryGetValue(npCommId, out var trophies))
            {
                return trophies;
            }

            string subUri =
                "npCommunicationIds/" + npCommId +
                "/trophyGroups/" + groupId +
                "/trophies";
            var content = await SendRequest(TrophyApiUri, subUri, limit, offset, serviceName);

            Trophies data = await content.ReadFromJsonAsync<Trophies>();

            Console.Write("Trophies for " + npCommId + ": ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(data.totalItemCount.ToString() + "\n");
            Console.ResetColor();
            for (int i = 0; i < data.totalItemCount; i++)
            {
                var trophy = data.trophies[i];

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[" + trophy.trophyId + "] " + trophy.trophyName);
                Console.ResetColor();
                Console.WriteLine("\tType: " + trophy.trophyType);
                Console.WriteLine("\tHidden: " + trophy.trophyHidden);
                Console.WriteLine("\tDetail: " + trophy.trophyDetail);
                Console.WriteLine("\tIcon Url: " + trophy.trophyIconUrl);
                Console.WriteLine("\tGroup ID: " + trophy.trophyGroupId);
                Console.WriteLine("\tProgress Target Value: " + trophy.trophyProgressTargetValue);
                Console.WriteLine("\tReward Name: " + trophy.trophyRewardName);
                Console.WriteLine("\tReward Image Url: " + trophy.trophyRewardImageUrl + "\n");


                if (groupId.Equals("all", StringComparison.OrdinalIgnoreCase) && !data.cachedTrophies.ContainsKey(trophy.trophyId))
                {
                    data.cachedTrophies.Add(trophy.trophyId, trophy);
                }
            }

            if (groupId.Equals("all", StringComparison.OrdinalIgnoreCase) && !CachedTitleTrophies.ContainsKey(npCommId))
            {
                CachedTitleTrophies.Add(npCommId, data);
            }

            return data;
        }



        /// <summary>
        /// Retrieves the earned status of trophies for a user from either a single - or all - trophy groups in a title.
        /// A title can have multiple groups of trophies (a "default" group which all titles have, and additional groups named "001" incrementing for each additional group.
        /// To retrieve trophies from all groups within a title (ie. the full trophy set) then "groupId" should be set to "all".
        /// </summary>
        /// <param name="npCommId">Unique ID of the title</param>
        /// <param name="accountId">The account whos trophy list is being accessed. Use "me" for the authenticating account</param>
        /// <param name="groupId">"all" to return all trophies for the title, otherwise restrict results to a specific trophy group</param>
        /// <param name="serviceName">"trophy" for PS3, PS4, or PS Vita platforms
        /// "trophy2" for the PS5 platform</param>
        /// <param name="limit">Limit the number of trophies returned. If no limit is specified all trophies will be returned.</param>
        /// <param name="offset">Returns trophy data from this result onwards</param>
        public async Task<Trophies> GetTrophiesEarnedForTitle(string npCommId, string accountId = "me", string groupId = "all", string serviceName = "trophy2", int? limit = null, int? offset = null)
        {
            string relativeUri = $"users/{accountId}/npCommunicationIds/{npCommId}/trophyGroups/{groupId}/trophies";
            var content = await SendRequest(TrophyApiUri, relativeUri, limit, offset, serviceName);

            Trophies data = await content.ReadFromJsonAsync<Trophies>();

            for (int i = 0; i < data.totalItemCount; i++)
            {
                var trophy = data.trophies[i];
                Debug.WriteLine(
                    "ID: " + trophy.trophyId +
                    " - Hidden: " + trophy.trophyHidden +
                    " - Earned: " + trophy.earned +
                    " - EarnedDateTime: " + trophy.earnedDateTime +
                    " - Type: " + trophy.trophyType +
                    " - Rarity: " + trophy.trophyRare +
                    " - Progress: " + trophy.progress +
                    " - ProgressRate: " + trophy.progressRate +
                    " - ProgressedDateTime: " + trophy.progressedDateTime);
            }

            return data;
        }

        /// <summary>
        /// Retrieves an overall summary of the numbner of trophies earned for a user broken down by type, as well as their current overall trophy level,
        /// progress towards the next level and which tier their current level falls in to. 
        /// </summary>
        /// <param name="accountId">The account whos trophy list is being accessed. Use "me" for the authenticating account</param>
        public async Task<TrophySummary> GetTrophyProfileSummaryForUser(string accountId = "me")
        {
            var content = await SendRequest(TrophyApiUri, $"users/{accountId}/trophySummary");

            TrophySummary data = await content.ReadFromJsonAsync<TrophySummary>();

            Debug.WriteLine(
                "AccountID: " + data.accountId +
                "\nTrophyLevel: " + data.trophyLevel +
                "\nTrophyPoint: " + data.trophyPoint +
                "\nLevelBasePoint: " + data.trophyLevelBasePoint +
                "\nLevelNextPoint: " + data.trophyLevelNextPoint +
                "\nProgress: " + data.progress +
                "\nTier: " + data.tier +
                "\nBronze: " + data.earnedTrophies.bronze +
                "\nSilver: " + data.earnedTrophies.silver +
                "\nGold: " + data.earnedTrophies.gold +
                "\nPlatinum: " + data.earnedTrophies.platinum);

            return data;
        }

        /// <summary>
        /// Retrieves a summary of all of the trophy groups associated with the title.
        /// </summary>
        /// <param name="npCommId">Unique ID of the title</param>
        /// <param name="serviceName">"trophy" for PS3, PS4, or PS Vita platforms
        /// "trophy2" for the PS5 platform</param>
        public async void GetTrophyGroupsForTitle(string npCommId, string serviceName = "trophy2")
        {
            var content = await SendRequest(TrophyApiUri, $"npCommunicationIds/{npCommId}/trophyGroups", null, null, serviceName);

            TrophyGroups data = await content.ReadFromJsonAsync<TrophyGroups>();

            Debug.WriteLine(
                "SetVersion: " + data.trophySetVersion +
                "\nTitleName: " + data.trophyTitleName +
                "\nTitleDetail: " + data.trophyTitleDetail +
                "\nIconUrl: " + data.trophyTitleIconUrl +
                "\nPlatform: " + data.trophyTitlePlatform +
                "\nDefined Trophies:" +
                "\nBronze: " + data.definedTrophies.bronze +
                "\nSilver: " + data.definedTrophies.silver +
                "\nGold: " + data.definedTrophies.gold +
                "\nPlatinum: " + data.definedTrophies.platinum +
                "\nTrophyGroups: \n"
                );

            for (int i = 0; i < data.trophyGroups.Count; i++)
            {
                var g = data.trophyGroups[i];
                Debug.WriteLine(
                    "GroupId: " + g.trophyGroupId +
                    "\nGroupName: " + g.trophyGroupName +
                    "\nGroupDetail: " + g.trophyGroupDetail +
                    "\nGroupIconUrl: " + g.trophyGroupIconUrl +
                    "\nDefinedTrophies:" +
                    "\nBronze: " + g.definedTrophies.bronze +
                    "\nSilver: " + g.definedTrophies.silver +
                    "\nGold: " + g.definedTrophies.gold +
                    "\nPlatinum: " + g.definedTrophies.platinum
                    );
            }
        }

        /// <summary>
        /// Retrieves a summary of the trophies earned for a user broken down by trophy group within a title.
        /// </summary>
        /// <param name="npCommId">Unique ID of the title</param>
        /// <param name="accountId">The account whos trophy list is being accessed. Use "me" for the authenticating account</param>
        /// <param name="serviceName">"trophy" for PS3, PS4, or PS Vita platforms
        /// "trophy2" for the PS5 platform</param>
        public async void GetTrophySummaryInTitleForUserByTrophyGroup(string npCommId, string accountId = "me", string serviceName = "trophy2")
        {
            var content = await SendRequest(
                TrophyApiUri, $"users/{accountId}/npCommunicationIds/{npCommId}/trophyGroups", null, null, serviceName);

            TrophyGroupSummaryData data = await content.ReadFromJsonAsync<TrophyGroupSummaryData>();

            Debug.WriteLine(
                "SetVersion: " + data.trophySetVersion +
                "\nHidden: " + data.hiddenFlag +
                "\nProgress: " + data.progress +
                "\nEarned Trophies:" +
                "\nBronze: " + data.earnedTrophies.bronze +
                "\nSilver: " + data.earnedTrophies.silver +
                "\nGold: " + data.earnedTrophies.gold +
                "\nPlatinum: " + data.earnedTrophies.platinum +
                "\nLastUpdatedTime: " + data.lastUpdatedDateTime +
                "\nTrophyGroups: \n"
                );

            for (int i = 0; i < data.trophyGroups.Count; i++)
            {
                var g = data.trophyGroups[i];
                Debug.WriteLine(
                    "GroupId: " + g.trophyGroupId +
                    "\nProgress: " + g.progress +
                    "\nLastUpdatedDateTime: " + g.lastUpdatedDateTime +
                    "\nEarned Trophies:" +
                    "\nBronze: " + g.earnedTrophies.bronze +
                    "\nSilver: " + g.earnedTrophies.silver +
                    "\nGold: " + g.earnedTrophies.gold +
                    "\nPlatinum: " + g.earnedTrophies.platinum
                    );
            }
        }

        /// <summary>
        /// Retrieves a summary of the trophies earned by a user for specific titles.
        /// </summary>
        /// <param name="npTitleIds">Unique ID of the title. Can be a single title ID, or it can be a comma separated list of IDs.
        /// Limit of 5 per request</param>
        /// <param name="accountId">The account whos trophy list is being accessed. Use "me" for the authenticating account</param>
        /// <param name="includeNotEarnedTrophyIds">The response will include the IDs for the individual trophies which have not been earned</param>
        public async void GetTrophySummaryInSpecificTitlesForUser(string npTitleIds, string accountId = "me", bool includeNotEarnedTrophyIds = false)
        {


        }
    }
}
