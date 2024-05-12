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

namespace PSNApiWrapper
{
    public class GraphqlClient
    {
        public static readonly Uri GraphqlApiUri = new Uri("https://m.np.playstation.com/api/graphql/v1/op");

        private PSNClient baseClient;

        public GraphqlClient(PSNClient baseClient)
        {
            this.baseClient = baseClient;
        }

        internal async Task<HttpResponseMessage> SendRequest(Uri BaseUri, string operationName, string variables, string extensions)
        {
            var accessToken = baseClient.GetAccessToken().Result;
            var request = new HttpRequestMessage();

            List<object> optionsList = new List<object>();
            optionsList.Add("operationName=" + operationName);
            optionsList.Add("variables=" + variables);
            optionsList.Add("extensions=" + extensions);
            string options = string.Join("&", optionsList);

            request.RequestUri = new Uri(BaseUri, "?" + options);
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

            var response = await baseClient.Http.SendAsync(request);

            Debug.Write("\nREQUEST: " + request);
            Debug.Write("\nRESPONSE: " + response);

            string errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error Content: {errorContent}");

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;

                case (HttpStatusCode)429:
                    Debug.WriteLine(response.Headers);
                    break;
            }
            return response;
        }

        /// <summary>
        /// Retrieves search results based on the context
        /// </summary>
        /// <param name="searchTerm">The search term to search for </param>
        /// <param name="searchContext">The search context. for example "MobileUniversalSearchGame" or "MobileUniversalSearchSocial"</param>
        /// <param name="displayTitleLocale">The title locale, for example "en-US"</param>
        public async Task<UniversalContextSearchResult> GetContextSearchResults(string searchTerm, string searchContext, string displayTitleLocale = "en-US")
        {
            // Search contexts:
            // MobileUniversalSearchGame
            // MobileUniversalSearchSocial


            string variables =
                "{\"searchTerm\":\"" + searchTerm + "\"," +
                "\"searchContext\":\"" + searchContext + "\"," +
                "\"displayTitleLocale\":\"" + displayTitleLocale + "\"}";

            string extensions = 
                "{\"persistedQuery\":" + 
                "{\"version\":1," + 
                "\"sha256Hash\":\"a2fbc15433b37ca7bfcd7112f741735e13268f5e9ebd5ffce51b85acc126f41d\"}}";


            var response = await SendRequest(GraphqlApiUri, "metGetContextSearchResults", variables, extensions);
            Debug.WriteLine(await response.Content.ReadAsStringAsync());

            SearchContextResultRoot root = await response.Content.ReadFromJsonAsync<SearchContextResultRoot>();
            UniversalContextSearchResult searchResult = root.data.universalContextSearch.results[0];

            return searchResult;
        }

        /// <summary>
        /// Retrieves a list of the trophies for a title which have Game Help available.
        /// You can check against all trophies, or you can limit the request to only check against specific trophy IDs.
        /// Only available for authenticating accounts with a PS+ subscription.
        /// </summary>
        /// <param name="npCommId">Unique ID of the title</param>
        /// <param name="trophyIds">Limit request to these specific trophy IDs</param>
        public async Task<TrophyHintAvailabilityRetrieved> GetTrophiesWithGameHelpAvailableForTitle(string npCommId, string[] trophyIds = null)
        {
            string variables =
                "{\"npCommId\":\"" + npCommId + "\"";

            if (trophyIds != null)
            {
                variables += ",\"trophyIds\":[";
                for (int i = 0; i < trophyIds.Length; i++)
                {
                    variables += "\"" + trophyIds[i] + "\"";
                    if (i != trophyIds.Length - 1)
                        variables += ",";
                }
                variables += "]";
            }
            variables += "}";

            string extensions =
                "{\"persistedQuery\":" +
                "{\"version\":1," +
                "\"sha256Hash\":\"71bf26729f2634f4d8cca32ff73aaf42b3b76ad1d2f63b490a809b66483ea5a7\"}}";

            var response = await SendRequest(GraphqlApiUri, "metGetHintAvailability", variables, extensions);
            Debug.WriteLine(await response.Content.ReadAsStringAsync());

            TrophyHintAvailabilityRoot root = await response.Content.ReadFromJsonAsync<TrophyHintAvailabilityRoot>();
            TrophyHintAvailabilityRetrieved hints = root.data.hintAvailabilityRetrieve;

            return hints;
        }

        /// <summary>
        /// Retrieves the Game Help which is available for a specific trophy.
        /// </summary>
        /// <param name="npCommId">Unique ID of the title the trophy belongs to	</param>
        /// <param name="trophyId">ID of the trophy	</param>
        /// <param name="udsObjectId">ID of the Game Help	</param>
        /// <param name="helpType">Type of Game Help</param>
        public async Task<TrophyTipsRetrieved> GetGameHelpForTrophy(string npCommId, string trophyId, string udsObjectId, string helpType)
        {
            // It is possible to request multiple trophies at once. 
            // For the usecase I'm using it for that won't be the case, but implement in the future?
            string variables =
                "{\"npCommId\":\"" + npCommId +
                "\",\"trophies\":[{\"trophyId\":\"" + trophyId +
                "\",\"udsObjectId\":\"" + udsObjectId +
                "\",\"helpType\":\"" + helpType + "\"}]}";

            string extensions =
                "{\"persistedQuery\":" + 
                "{\"version\":1," + 
                "\"sha256Hash\":\"93768752a9f4ef69922a543e2209d45020784d8781f57b37a5294e6e206c5630\"}}";

            var response = await SendRequest(GraphqlApiUri, "metGetTips", variables, extensions);
            Debug.WriteLine(await response.Content.ReadAsStringAsync());

            TrophyTipRoot root = await response.Content.ReadFromJsonAsync<TrophyTipRoot>();
            TrophyTipsRetrieved tips = root.data.tipsRetrieve;

            if (!tips.hasAccess)
            {
                throw new Exception("The user account does not have a PS+ subscription");
            }

            return tips;
        }
    }
}
