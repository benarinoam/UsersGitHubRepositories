using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UsersGitHubRepositories
{
    public class GitHubClient
    {
        //https://api.github.com
        private static HttpClient mobjClient = new HttpClient();

        /// <summary>
        /// Sets the HttpClient Header configurations.
        /// </summary>
        public static void OpenClient()
        {
            if (mobjClient != null)
            {
                mobjClient.BaseAddress = new Uri("https://api.github.com/");
                mobjClient.DefaultRequestHeaders.Accept.Clear();
                mobjClient.DefaultRequestHeaders.Add("Accepts", "application/vnd.github.v3+json; charset=utf-8");
                mobjClient.DefaultRequestHeaders.Add("access_token", "1b501e43d7af30d6655bb2b0dd3f799f44ca8219");
                mobjClient.DefaultRequestHeaders.Add("User-Agent", "No-No");
                mobjClient.Timeout = new TimeSpan(0, 0, 15);
            }
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
                throw new Exception("Failed on request Search Users", Ex);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                throw new Exception("Failed to get Users Async.",Ex);
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
                throw new Exception("Failed on request User's Repositories Async.", Ex);
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
                        Console.WriteLine(string.Format("Not all User Data Updated for user: {0}", objUser.Login);
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
    }

}
