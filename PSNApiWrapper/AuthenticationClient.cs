using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using PSNApiWrapper.Models;
using System.Collections.Generic;

namespace PSNApiWrapper
{
    public class AuthenticationClient
    {
        private PSNClient PSN;
        public static readonly Uri AuthBaseUri = new Uri("https://ca.account.sony.com/api/authz/v3/oauth/");
        private AuthenticationData Data = new AuthenticationData();
        public DateTime TokenExpiresAt = DateTime.UtcNow;

        Exception psnAccessException = new Exception(
                "There was a problem retrieving your PSN access code. Is your NPSSO token valid? \n" +
                "To get a new NPSSO token, visit https://ca.account.sony.com/api/v1/ssocookie."
                );

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
            if (IsAccessTokenValid && npsso == PSN.NpssoKey)
            {
                PSN.WriteLine("Access token is still valid. It expires at " + TokenExpiresAt.ToLocalTime());
                return Data.Access_Token;
            }

            if (npsso == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                throw new ArgumentNullException(nameof(npsso), new Exception(
                    "No NPSSO token was provided. \n" +
                    "To get a new NPSSO token, visit https://ca.account.sony.com/api/v1/ssocookie."
                    ));
            }

            if (npsso != PSN.NpssoKey)
                PSN.NpssoKey = npsso;

            return await GetAccessTokenFromCode(await GetCodeFromNpsso(npsso));
        }

        private async Task<string> GetCodeFromNpsso(string npsso)
        {
            // Idk why using the HttpClient doesn't work for me here.
            // So this will do for now. Hope to fix it in the future.

            string[] requestParams =
            {
                "access_type=offline",
                "client_id=09515159-7237-4370-9b40-3806e67c0891",
                "response_type=code",
                "scope=psn:mobile.v2.core psn:clientapp",
                "redirect_uri=com.scee.psxandroid.scecompcall://redirect"
            };

            var request = WebRequest.CreateHttp(AuthBaseUri + "authorize?$" + string.Join("&", requestParams));
            request.Headers.Add("Cookie", $"npsso={npsso}");
            request.AllowAutoRedirect = false;

            var response = await request.GetResponseAsync();
            string location = response.Headers["Location"] ?? throw psnAccessException;
            Debug.WriteLine(location);

            string accessCode = location.Substring(location.IndexOf("?code=v3") + 1) ?? throw psnAccessException;
            return accessCode;
        }

        private async Task<string> GetAccessTokenFromCode(string accessCode)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(AuthBaseUri, "token"))
            {
                Content = new StringContent(
                accessCode +
                "&redirect_uri=com.scee.psxandroid.scecompcall://redirect" +
                "&grant_type=authorization_code" +
                "&token_format=jwt")
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            request.Headers.Add("Authorization", "Basic MDk1MTUxNTktNzIzNy00MzcwLTliNDAtMzgwNmU2N2MwODkxOnVjUGprYTV0bnRCMktxc1A=");
            var test = await request.Content.ReadAsStringAsync();
            Debug.WriteLine("REQUEST CONTENT:\n" + test);
            var response = await PSN.Http.SendAsync(request);
            Debug.WriteLine("REQUEST:\n" + request);
            
            Debug.WriteLine("RESPONSE:\n" + response);

            Data = await response.Content.ReadFromJsonAsync<AuthenticationData>();

            if (Data.Access_Token == null)
            {
                throw psnAccessException;
            }

            TokenExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(Data.Expires_In);
            PSN.WriteLine("Access Token successfully granted.");
            PSN.WriteLine("Access Token expires at: " + TokenExpiresAt.ToLocalTime());

            return Data.Access_Token;
        }
    }
}
