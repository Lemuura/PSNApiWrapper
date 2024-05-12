using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO.Compression;
using System.IO;

namespace PSNApiWrapper
{
    public class SearchClient
    {
        public static readonly Uri SearchApiUri = new Uri("https://m.np.playstation.com/api/search/v1/universalSearch");

        private PSNClient baseClient;

        public SearchClient(PSNClient baseClient)
        {
            this.baseClient = baseClient;
        }

        internal async Task<HttpResponseMessage> SendSearchRequest(Uri BaseUri, string searchTerm, string domain)
        {
            var accessToken = baseClient.GetAccessToken().Result;
            var request = new HttpRequestMessage();

            //var accountId = "2933778291329052827"; // My accountId
            //var accountId = "4491584107107829144"; // Alice's accountId
            var accountId = "Lemuura";
            searchTerm = "lemuura";
            var alternateSearch = searchTerm;
            domain = "SocialFriends";
            request.Content = new StringContent(JsonConvert.SerializeObject(new
            {
                searchTerm,
                accountId,
                domainRequests = new[] { new { domain } }
            }), Encoding.UTF8, "application/json");

            // Domain valid values:
            // SocialAllAccounts
            // SocialFriends seems valid?

            // 14 known properties:
            // "inputDevice", "alternateSearch", "countryCode", "suggestion",
            // "languageCode", "searchContextRequest", "fallbackDomainRequests",
            // "strandPaginationRequest", "searchTerm", "explain", "age",
            // "suggestionSize", "domainRequests", "accountId"

            // Domainrequests:
            // 17 known properties: \"domain\", \"subscriptionHubSearchParams\",
            // \"videoSearchParams\", \"minScoreThreshold\", \"storeSearchParams\",
            // \"displayTitleLocale\", \"featureFlags\", \"tingsSearchParams\",
            // \"conceptSearchParams\", \"fields\", \"pagination\", \"socialSearchParams\",
            // \"facets\", \"blendedSearchParams\", \"settingsSearchParams\", \"partnerSearchParams\", \"sort\"

            request.RequestUri = BaseUri;
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            request.Method = HttpMethod.Post;
            

            var response = await baseClient.Http.SendAsync(request);

            Debug.Write("\nREQUEST: " + request);
            Debug.Write("\nRESPONSE: " + response);

            if (!response.IsSuccessStatusCode)
            {
                // Handle error
                if (response.Content.Headers.ContentEncoding.Any(x => x.Equals("gzip", StringComparison.OrdinalIgnoreCase)))
                {
                    // Decompress the content if it is gzip-encoded
                    using (var decompressedStream = new GZipStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
                    using (var reader = new StreamReader(decompressedStream))
                    {
                        string errorContent = await reader.ReadToEndAsync();
                        Console.WriteLine($"Error Content: {errorContent}");

                    }
                }
                else
                {
                    // Handle non-gzip-encoded content
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error Content: {errorContent}");
                }
            }
            else
            {
                // Handle successful response
                string successContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Success: {successContent}");
            }

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

        public async void MakeUniversalSearch(string searchTerm, string domain = null)
        {
            await SendSearchRequest(SearchApiUri, searchTerm, domain);
        }
    }
}
