using System;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using System.Collections.Generic;
using UcwaSfboConsole;

namespace SFBBot_UCWA.UcwaSfbo
{
    public class UcwaApplications
    {
        public class UcwaMyAppsObject
        {
            public string UserAgent { get; set; }
            public string EndpointId { get; set; }
            public string Culture { get; set; }

            //public List<string> supportedModalities { get; set; }
        }

       public static string CreateUcwaApps(HttpClient httpClient, AuthenticationResult ucwaAuthenticationResult, string ucwaApplicationsRootUri,
            UcwaMyAppsObject ucwaAppsObject)
        {
            string createUcwaAppsResults = string.Empty;

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var createUcwaPostData = JsonConvert.SerializeObject(ucwaAppsObject);
            Console.WriteLine("CreateUcwaApps POST data is " + createUcwaPostData);
            var httpResponseMessage =
                httpClient.PostAsync(ucwaApplicationsRootUri, new StringContent(createUcwaPostData, Encoding.UTF8,
                "application/json")).Result;
            Console.WriteLine("CreateUcwaApps response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                createUcwaAppsResults = httpResponseMessage.Content.ReadAsStringAsync().Result;
                ApplicationRoot obj = new ApplicationRoot();
                JsonConvert.PopulateObject(createUcwaAppsResults, obj);
                if (obj != null)
                {
                    ConfigData.ucwaApplication = obj._links.self.href;
                    // ConfigData.ucwaApplications += ConfigData.ucwaApplication;
                    ConfigData.ucwaEvents = obj._links.events.href;
                }
                //        switch (xml02.GetAttribute("rel"))
                //        case "application":ucwaApplication
                //        case "me": ConfigData.ucwaMe));
                //        case "events":ConfigData.ucwaEvents));
                //        case "makeMeAvailable":ConfigData.ucwaMakeMeAvailable));
                //        case "startMessaging":ConfigData.ucwaMessagingInvitations));
                //        case "startPhoneAudio": ConfigData.ucwaStartPhoneAudio));                
            }
            return createUcwaAppsResults;


        }
    }
}
