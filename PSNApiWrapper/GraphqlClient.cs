using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using PSNApiWrapper.Models;
using Newtonsoft.Json;

namespace PSNApiWrapper
{
    public class GraphqlClient
    {
        public static readonly Uri GraphqlApiUri = new Uri("https://m.np.playstation.com/api/graphql/v1/op");

        private PSNClient PSN;

        public GraphqlClient(PSNClient baseClient)
        {
            this.PSN = baseClient;
        }

        private async Task<HttpContent> SendRequest(string operationName, Dictionary<string, object> variables, string sha256Hash)
        {
            List<object> requestParams = new List<object>
            {
                "operationName=" + operationName,
                "variables=" + JsonConvert.SerializeObject(variables),
                $"extensions={{\"persistedQuery\":{{\"version\":1,\"sha256Hash\":\"{sha256Hash}\"}}}}"
            };

            return await PSN.SendGetRequest(GraphqlApiUri, "?" + string.Join("&", requestParams));
        }

        /// <summary>
        /// Retrieves search results based on the context
        /// </summary>
        /// <param name="searchTerm">The search term to search for </param>
        /// <param name="searchContext">The search context. for example "MobileUniversalSearchGame" or "MobileUniversalSearchSocial"</param>
        /// <param name="displayTitleLocale">The title locale, for example "en-US"</param>
        public async Task<UniversalDomainSearchResponse> GetContextSearchResults(string searchTerm, string searchContext, string displayTitleLocale = "en-US")
        {
            // Search contexts:
            // MobileUniversalSearchGame
            // MobileUniversalSearchSocial

            var variables = new Dictionary<string, object>
            {
                { "searchTerm", searchTerm },
                { "searchContext", searchContext },
                { "displayTitleLocale", displayTitleLocale }
            };

            var content = await SendRequest("metGetContextSearchResults", variables,
                "a2fbc15433b37ca7bfcd7112f741735e13268f5e9ebd5ffce51b85acc126f41d");

            SearchContextResultRoot root = await content.ReadFromJsonAsync<SearchContextResultRoot>();
            UniversalDomainSearchResponse searchResult = root.data.universalContextSearch.results[0];

            return searchResult;
        }

        /// <summary>
        /// Retrieves a list of the trophies for a title which have Game Help available.
        /// You can check against all trophies, or you can limit the request to only check against specific trophy IDs.
        /// Only available for authenticating accounts with a PS+ subscription.
        /// </summary>
        /// <param name="npCommId">Unique ID of the title</param>
        /// <param name="trophyIds">Limit request to these specific trophy IDs</param>
        public async Task<HintAvailability> GetTrophiesWithGameHelpAvailableForTitle(string npCommId, string[] trophyIds = null)
        {
            var variables = new Dictionary<string, object>
            {
                { "npCommId", npCommId }
            };

            if (trophyIds != null)
            {
                variables.Add("trophyIds", trophyIds);
            }

            var content = await SendRequest("metGetHintAvailability", variables,
                "71bf26729f2634f4d8cca32ff73aaf42b3b76ad1d2f63b490a809b66483ea5a7");

            HintAvailabilityRoot root = await content.ReadFromJsonAsync<HintAvailabilityRoot>();
            HintAvailability hints = root.data.hintAvailabilityRetrieve;

            return hints;
        }

        /// <summary>
        /// Retrieves the Game Help which is available for a specific trophy.
        /// </summary>
        /// <param name="npCommId">Unique ID of the title the trophy belongs to	</param>
        /// <param name="trophies">Which trophies to retrieve help for</param>
        public async Task<Tips> GetGameHelpForTrophy(string npCommId, HelpTrophiesParams[] trophies)
        {
            var variables = new Dictionary<string, object>
            {
                { "npCommId", npCommId },
                { "trophies", trophies }
            };

            var content = await SendRequest("metGetTips", variables,
                "93768752a9f4ef69922a543e2209d45020784d8781f57b37a5294e6e206c5630");

            TrophyTipsRoot root = await content.ReadFromJsonAsync<TrophyTipsRoot>();
            Tips tips = root.data.tipsRetrieve;

            if (!tips.hasAccess)
            {
                throw new Exception("The user account does not have a PS+ subscription");
            }

            return tips;
        }

        public async Task<Tips> GetGameHelpForTrophy(string npCommId, HelpTrophiesParams trophy)
        {
            return await GetGameHelpForTrophy(npCommId, new[] { trophy });
        }

        
    }
}
