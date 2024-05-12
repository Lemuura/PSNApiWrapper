using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.Linq;

namespace PSNApiWrapper
{
    public class PSNClient
    {
        // Most summaries throughout this project is taken from https://andshrew.github.io/PlayStation-Trophies/#/APIv2
        // Get your NPSSO key from https://ca.account.sony.com/api/v1/ssocookie

        public static readonly Uri BaseUri = new Uri("https://m.np.playstation.com/");
        public HttpClient Http = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });

        public string NpssoKey = null;
        public string Language = "en-US";        

        /// <summary>
        /// Methods for obtaining and refreshing authentication
        /// </summary>
        public AuthenticationClient Auth { get; private set; }

        /// <summary>
        /// Methods for obtaining Trophy information
        /// </summary>
        public TrophyClient Trophy { get; private set; }

        /// <summary>
        /// Methods for Graphql information
        /// </summary>
        public GraphqlClient Graphql { get; private set; }

        public PSNClient(string npsso, string language = "en-US")
        {
            NpssoKey = npsso;
            Language = language;
            Auth = new AuthenticationClient(this);
            Trophy = new TrophyClient(this);
            Graphql = new GraphqlClient(this);
        }

        internal async Task<string> GetAccessToken()
        {
            return await Auth.GetOrRefreshAccessToken(NpssoKey);
        }

        public void WriteLine(string line, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        internal async Task<HttpContent> SendGetRequest(Uri BaseUri, string RelativeUri)
        {
            var accessToken = await GetAccessToken();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(BaseUri, RelativeUri)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            request.Headers.Add("Accept-Language", Language);

            var response = await Http.SendAsync(request);

            Debug.WriteLine("REQUEST: " + request);
            Debug.WriteLine("RESPONSE: " + response);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent;
                if (response.Content.Headers.ContentEncoding.Any(x => x.Equals("gzip", StringComparison.OrdinalIgnoreCase)))
                {
                    using (var decompressedStream = new GZipStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
                    using (var reader = new StreamReader(decompressedStream))
                    {
                        errorContent = await reader.ReadToEndAsync();
                    }
                }
                else
                {
                    errorContent = await response.Content.ReadAsStringAsync();
                }

                Console.WriteLine($"Error Content: {errorContent}");

                switch (response.StatusCode)
                {
                    case (HttpStatusCode)429: // Too Many Requests
                    default:
                        throw new Exception($"Status Code {response.StatusCode}. {response.ReasonPhrase}.");

                }
            }
            else
            {
                string successContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Success: {successContent}");
            }

            return response.Content;
        }
    }
}