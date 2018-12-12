using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UsersGitHubRepositories
{
    public class GitHubClient : IDisposable
    {
        //https://api.github.com
        private static HttpClient mobjClient = new HttpClient();

        /// <summary>
        /// Sets the HttpClient Header configurations.
        /// </summary>
        public static void OpenClient(string strLogin)
        {

            byte[] objBuffer;

            if (mobjClient != null)
            {
                if (mobjClient.BaseAddress == null)
                {
                    mobjClient.BaseAddress = new Uri("https://api.github.com/");
                }
                mobjClient.DefaultRequestHeaders.Accept.Clear();
                mobjClient.DefaultRequestHeaders.Add("Accept", "aapplication/vnd.github.v3+json; charset=utf-8");

                if (mobjClient.DefaultRequestHeaders.UserAgent.Count == 0)
                {
                    mobjClient.DefaultRequestHeaders.Add("User-Agent", "UsersGitHubRepositories");
                }
                objBuffer = Encoding.ASCII.GetBytes(strLogin);
                mobjClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.
                    AuthenticationHeaderValue("Basic", Convert.ToBase64String(objBuffer));               
            }

        }

        private static async Task<AuthorizationResponde> GetLoginCode(string strLogin)
        {
            string strState;
            string strSearchPath;
            string strResponseData;
            string[] objResponseData;
            AuthorizationResponde objAuthorizationResponde;

            strState = Guid.NewGuid().ToString();

            objResponseData = new string[] { "", strState };

            var byteArray = Encoding.ASCII.GetBytes("benarinoam:Shalevb10");
            mobjClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            AuthorizationObject objAuthorization = new AuthorizationObject()
            {
                scopes = new string[] { "public_repo" },
                notes = "UsersGitHubRepositories",
                note_url = @"https://github.com/benarinoam/UsersGitHubRepositories",
                client_id = "Iv1.4ebc8adcda71679e",
                client_secret = "cdc539480bb33ba1adc936f5923660d629591354",
                fingerprint = Guid.NewGuid().ToString()
            };


            strSearchPath = JsonConvert.SerializeObject(objAuthorization);

            StringContent objContent = new StringContent(strSearchPath);

            HttpResponseMessage objResponse = mobjClient.PostAsync("authorizations", objContent).Result;

            //strSearchPath = string.Format(@"https://github.com/login/oauth/authorize?" +
            //@"client_id=Iv1.4ebc8adcda71679e" +
            //@"&state={0}" +
            //@"&login={1}", strState, strLogin);



            //HttpResponseMessage objResponse = mobjClient.GetAsync(strSearchPath).Result;

            // objResponse.EnsureSuccessStatusCode();

            //if (objResponse.IsSuccessStatusCode)
            //{
            strResponseData = await objResponse.Content.ReadAsStringAsync();
            objAuthorizationResponde = 
                JsonConvert.DeserializeObject<AuthorizationResponde>(strResponseData);
            //         }

            return objAuthorizationResponde;
        }
        /// <summary>
        /// Return up to 1000 Users From github Search.
        /// </summary>
        /// <param name="strQueryPrameter">User Login name to search</param>
        /// <returns></returns>
        public static async Task<GitHubUsers> GetUsersAsync(string strQueryPrameter)
        {

            string strSearchPath;
            string strResponseData;
            GitHubUsers objGitHubUsers = null;
            int intNumberOfPages = 10;

            try
            {

                for (int intPageNumber = 1; intPageNumber <= intNumberOfPages; intPageNumber++)
                {
                    System.Threading.Thread.Sleep(200);

                    strSearchPath = string.Format(@"search/users?q={0}+type:user&page={1}&per_page=100", strQueryPrameter, intPageNumber);

                    HttpResponseMessage objResponse = mobjClient.GetAsync(strSearchPath).Result;

                    objResponse.EnsureSuccessStatusCode();

                    if (objResponse.IsSuccessStatusCode)
                    {

                        strResponseData = await objResponse.Content.ReadAsStringAsync();

                        if (objGitHubUsers == null)
                        {
                            objGitHubUsers = JsonConvert.DeserializeObject<GitHubUsers>(strResponseData);
                        }
                        else
                        {
                            objGitHubUsers.Items.AddRange(
                                JsonConvert.DeserializeObject<GitHubUsers>(strResponseData).Items);
                        }


                        if (objGitHubUsers.Total_Count < 1000)
                        {
                            intNumberOfPages = (objGitHubUsers.Total_Count / 100) + 1;
                        }
                    }
                }
                return objGitHubUsers;
            }
            catch (HttpRequestException HttpEx)
            {
                Console.WriteLine(HttpEx.Message);
                throw new Exception("Failed on request Search Users", HttpEx);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                throw new Exception("Failed to get Users Async.", Ex);
            }
        }
        /// <summary>
        /// Read User's Repositories List from GitHub.
        /// </summary>
        /// <param name="objGitHubUser">GitHub User from the search result</param>
        /// <returns></returns>
        public static async Task<List<Repository>> GetUserRepsitoriesAsync(User objGitHubUser)
        {

            string strSearchPath;
            string strResponseData;
            List<Repository> objUserRepositories = null;

            try
            {
                System.Threading.Thread.Sleep(200);

                strSearchPath = string.Format(@"users/{0}/repos", objGitHubUser.Login);

                HttpResponseMessage objResponse = mobjClient.GetAsync(strSearchPath).Result;

                objResponse.EnsureSuccessStatusCode();

                if (objResponse.IsSuccessStatusCode)
                {

                    strResponseData = await objResponse.Content.ReadAsStringAsync();


                    objUserRepositories = JsonConvert.DeserializeObject<List<Repository>>(strResponseData);

                }

                return objUserRepositories;
            }
            catch (HttpRequestException HttpEx)
            {
                Console.WriteLine(HttpEx.Message);
                throw new Exception("Failed on request User's Repositories Async.", HttpEx);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                throw new Exception("Failed to get User's Repositories Async.", Ex);
            }

        }
        /// <summary>
        /// Updates Extra Users Data from GitHub. 
        /// GitHub requests Limit to 5000.
        /// </summary>
        /// <param name="objGitHubUsers">List of GitHub users from the search result</param>
        /// <returns></returns>
        public static async Task<GitHubUsers> GetUsersDataAsync(GitHubUsers objGitHubUsers)
        {

            string strSearchPath;
            string strResponseData;
            User objUserData = null;

            try
            {
                foreach (User objUser in objGitHubUsers.Items)
                {
                    try
                    {
                        strSearchPath = string.Format(@"users/{0}", objUser.Login);

                        System.Threading.Thread.Sleep(200);

                        HttpResponseMessage objResponse = mobjClient.GetAsync(strSearchPath).Result;

                        objResponse.EnsureSuccessStatusCode();

                        if (objResponse.IsSuccessStatusCode)
                        {

                            strResponseData = await objResponse.Content.ReadAsStringAsync();

                            objUserData = JsonConvert.DeserializeObject<User>(strResponseData);

                            objUser.Update(objUserData);
                        }
                    }
                    catch (HttpRequestException HttpEx)
                    {
                        Console.WriteLine(string.Format("Not all User Data Updated for user: {0}", objUser.Login));
                    }

                }

                return objGitHubUsers;
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                throw new Exception("Failed to get User's Data Async.", Ex);
            }
        }

        public void Dispose()
        {
            mobjClient.Dispose();
        }

        private class AuthorizationObject
        {
            public string[] scopes { get; set; }
            public string notes { get; set; }
            public string note_url { get; set; }
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string fingerprint { get; set; }
        }
        private class AuthorizationResponde
        {
            public long Id { get; set; }
            public string url { get; set; }
            public string token { get; set; }
            public string hashed_token { get; set; }
            public string token_last_eight { get; set; }
            public string note { get; set; }
            public string note_url { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string[] scopes { get; set; }
            public string fingerprint { get; set; }
        }
    }

}
