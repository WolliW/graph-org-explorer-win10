using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;

namespace Windows10OrgExplorer
{
    public class EmployeeModel
    {
        public Guid objectId { get; set; }
        public string displayName { get; set; }
    }

    public class EmployeeOrgModel
    {
        public EmployeeModel Employee { get; set; }
        public EmployeeModel Manager { get; set; }
        public List<EmployeeModel> DirectReports { get; set; }

        public async static Task<EmployeeOrgModel> GetEmployeeModel(string path)
        {
            EmployeeOrgModel data = new EmployeeOrgModel();

            //get the access token for calling into the graph
            var token = await GetAccessTokenForResource("https://graph.microsoft.com/");

            //get the user
            var json = await GetJson(String.Format("https://graph.microsoft.com/beta/{0}", path), token);
            data.Employee = JsonConvert.DeserializeObject<EmployeeModel>(json);

            //get the manager...might not exist
            json = await GetJson(String.Format("https://graph.microsoft.com/beta/{0}/manager", path), token);
            if (json == null)
                data.Manager = new EmployeeModel();
            else
                data.Manager = JsonConvert.DeserializeObject<EmployeeModel>(json);

            //get the direct reports
            json = await GetJson(String.Format("https://graph.microsoft.com/beta/{0}/directReports", path), token);
            data.DirectReports = JObject.Parse(json).SelectToken("value").ToObject<List<EmployeeModel>>();

            return data;
        }

        private async static Task<string> GetJson(string endpoint, string accessToken)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            using (HttpResponseMessage response = await client.GetAsync(new Uri(endpoint)))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                    return null;
            }

        }

        private async static Task<string> GetAccessTokenForResource(string resource)
        {
            string token = null;

            //first try to get the token silently
            WebAccountProvider aadAccountProvider = await WebAuthenticationCoreManager.FindAccountProviderAsync("https://login.windows.net");
            WebTokenRequest webTokenRequest = new WebTokenRequest(aadAccountProvider, String.Empty, App.Current.Resources["ida:ClientID"].ToString(), WebTokenRequestPromptType.Default);
            webTokenRequest.Properties.Add("authority", "https://login.windows.net");
            webTokenRequest.Properties.Add("resource", resource);
            WebTokenRequestResult webTokenRequestResult = await WebAuthenticationCoreManager.GetTokenSilentlyAsync(webTokenRequest);
            if (webTokenRequestResult.ResponseStatus == WebTokenRequestStatus.Success)
            {
                WebTokenResponse webTokenResponse = webTokenRequestResult.ResponseData[0];
                token = webTokenResponse.Token;
            }
            else if (webTokenRequestResult.ResponseStatus == WebTokenRequestStatus.UserInteractionRequired)
            {
                //get token through prompt
                webTokenRequest = new WebTokenRequest(aadAccountProvider, String.Empty, App.Current.Resources["ida:ClientID"].ToString(), WebTokenRequestPromptType.ForceAuthentication);
                webTokenRequest.Properties.Add("authority", "https://login.windows.net");
                webTokenRequest.Properties.Add("resource", resource);
                webTokenRequestResult = await WebAuthenticationCoreManager.RequestTokenAsync(webTokenRequest);
                if (webTokenRequestResult.ResponseStatus == WebTokenRequestStatus.Success)
                {
                    WebTokenResponse webTokenResponse = webTokenRequestResult.ResponseData[0];
                    token = webTokenResponse.Token;
                }
            }

            return token;
        }
    }
}
