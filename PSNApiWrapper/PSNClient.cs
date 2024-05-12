using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System;
using System.Net.Http;

namespace PSNApiWrapper
{
    public class PSNClient
    {
        public static readonly Uri BaseUri = new Uri("https://m.np.playstation.com/");

        public string NpssoKey = null;

        public HttpClient Http = new HttpClient();

        /// <summary>
        /// Methods for obtaining and refreshing authentication
        /// </summary>
        public AuthenticationClient Auth { get; private set; }

        /// <summary>
        /// Methods for obtaining Trophy information
        /// </summary>
        public TrophyClient Trophy { get; private set; }

        /// <summary>
        /// Methods for obtaining Search information
        /// </summary>
        public SearchClient Search { get; private set; }

        /// <summary>
        /// Methods for Graphql information
        /// </summary>
        public GraphqlClient Graphql { get; private set; }

        public PSNClient()
        {
            Init();
        }

        public PSNClient(string npsso)
        {
            NpssoKey = npsso;
            Init();
        }

        private void Init()
        {
            Auth = new AuthenticationClient(this);
            Trophy = new TrophyClient(this);
            Search = new SearchClient(this);
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
    }
}