using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;

namespace PSNApiWrapper
{
    public class AuthenticationClient
    {
        private PSNClient PSN;
        public static readonly Uri AuthBaseUri = new Uri("https://ca.account.sony.com/api/authz/v3/oauth/");
        private AuthenticationData Data = new AuthenticationData();
        public DateTime TokenExpiresAt = DateTime.UtcNow;

        public AuthenticationClient(PSNClient baseClient)
        {
            this.PSN = baseClient;
        }

        public bool IsAccessTokenValid
        {
            get 
            {
                if (Data.Access_Token == null)
                    return false;

                if (DateTime.UtcNow >= TokenExpiresAt)
                    return false;
                return true;
            }
        }

        public async Task<string> GetOrRefreshAccessToken(string npsso)
        {
            // Change this so that if you insert a new NPSSO key it forces a new update
            if (IsAccessTokenValid && npsso == PSN.NpssoKey)
            {
                PSN.WriteLine("Access token is still valid. It expires at " + TokenExpiresAt.ToString());
                return Data.Access_Token;
            }
                

            if (npsso == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                throw new Exception(
                    "No NPSSO token was provided. \n" +
                    "To get a new NPSSO token, visit https://ca.account.sony.com/api/v1/ssocookie."
                    );
            }

            if (PSN.NpssoKey == null || PSN.NpssoKey != npsso)
            {
                PSN.NpssoKey = npsso;
            }

            var accessCode = await GetCodeFromNpsso(npsso);
            return await GetAccessTokenFromCode(accessCode);


        }

        private async Task<string> GetCodeFromNpsso(string npsso)
        {
            string queryString =
                "access_type=offline" +
                "&client_id=09515159-7237-4370-9b40-3806e67c0891" +
                "&response_type=code" +
                "&scope=psn:mobile.v2.core psn:clientapp" +
                "&redirect_uri=com.scee.psxandroid.scecompcall://redirect";

            var request = HttpWebRequest.CreateHttp(AuthBaseUri + "authorize?$" + queryString);
            request.Headers.Add("Cookie", "npsso=" + npsso);
            request.AllowAutoRedirect = false;

            var response = await request.GetResponseAsync();

            if (response.Headers["Location"] == null || !response.Headers["Location"].Contains("?code="))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                throw new Exception(
                    "There was a problem retrieving your PSN access code. Is your NPSSO token valid? \n" +
                    "To get a new NPSSO token, visit https://ca.account.sony.com/api/v1/ssocookie."
                    );
            }

            Debug.Write("\n\n" + response.Headers["Location"]);
            // com.scee.psxandroid.scecompcall://redirect/?code=v3.   etc etc etc
            var queryIndex = response.Headers["Location"].IndexOf("?");
            string accessCode = response.Headers["Location"].Substring(queryIndex + 1);

            Debug.Write("\n" + accessCode);

            return accessCode;
        }

        private async Task<string> GetAccessTokenFromCode(string accessCode)
        {
            StringContent body = new StringContent(
                accessCode +
                "&redirect_uri=com.scee.psxandroid.scecompcall://redirect" +
                "&grant_type=authorization_code" +
                "&token_format=jwt");

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(AuthBaseUri, "token");
            request.Method = HttpMethod.Post;
            request.Content = body;
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            request.Headers.Add("Authorization", "Basic MDk1MTUxNTktNzIzNy00MzcwLTliNDAtMzgwNmU2N2MwODkxOnVjUGprYTV0bnRCMktxc1A=");

            var response = await PSN.Http.SendAsync(request);

            Debug.Write("\nREQUEST:\n" + request);
            Debug.Write("\nRESPONSE:\n" + response);

            Data = await response.Content.ReadFromJsonAsync<AuthenticationData>();

            if (Data.Access_Token == null)
                return null;


            PSN.WriteLine("Access Token successfully granted.");
            Debug.WriteLine("ACCESS TOKEN:  " + Data.Access_Token);
            Debug.WriteLine("TOKEN TYPE:    " + Data.Token_Type);
            Debug.WriteLine("EXPIRES IN:    " + Data.Expires_In);
            Debug.WriteLine("SCOPE:         " + Data.Scope);
            Debug.WriteLine("ID TOKEN:      " + Data.Id_Token);

            TokenExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(Data.Expires_In);
            PSN.WriteLine("TOKEN EXPIRES AT: " + TokenExpiresAt + " UTC");

            return Data.Access_Token;

            
        }
    }

    public class AuthenticationData
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public double Expires_In { get; set; }
        public string Scope { get; set; }
        public string Id_Token { get; set; }
    }
}
