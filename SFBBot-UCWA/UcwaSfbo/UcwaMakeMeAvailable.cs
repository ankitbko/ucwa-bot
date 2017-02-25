using System;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections.Generic;

namespace SFBBot_UCWA.UcwaSfbo
{
    public class UcwaMakeMeAvailable
    {
        public class UcwaMakeMeAvailableObject
        {
            public string signInAs { get; set; }
            public string phoneNumber { get; set; }

            public List<string> supportedMessageFormats { get; set; }
            public List<string> supportedModalities { get; set; }
        }

        public static String GetMakeMeAvailableUri(String createUcwaAppsResults, String ucwaApplicationHostRootUri)
        {
            dynamic createUcwaAppsResultsObject = JObject.Parse(createUcwaAppsResults);
            string getMakeMeAvailableUri = ucwaApplicationHostRootUri + ConfigData.ucwaApplication + "/communication/makeMeAvailable";

            //try
            //{
            //    getMakeMeAvailableUri = ucwaApplicationHostRootUri +
            //       createUcwaAppsResultsObject._embedded.me._links.makeMeAvailable.href;
            //}
            //catch
            //{
                
            //}
            Console.WriteLine("getMakeMeAvailableUri is " + getMakeMeAvailableUri);
            return getMakeMeAvailableUri;
        }

        public static bool MakeMeAvailable(HttpClient httpClient, AuthenticationResult ucwaAuthenticationResult, String ucwaMakeMeAvailableRootUri,
            UcwaMakeMeAvailableObject ucwaMyPresenceObject)
        {
            string makeMeAvailableResults = string.Empty;
            Console.WriteLine("URI is " + ucwaMakeMeAvailableRootUri);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var makeMeAvailablePostData = JsonConvert.SerializeObject(ucwaMyPresenceObject);
            Console.WriteLine("MakeMeAvailable POST data is " + makeMeAvailablePostData);
            var httpResponseMessage =
                httpClient.PostAsync(ucwaMakeMeAvailableRootUri, new StringContent(makeMeAvailablePostData, Encoding.UTF8,
                "application/json")).Result;
            Console.WriteLine("MakeMeAvailable response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
            Console.WriteLine("MakeMeAvailable response should be empty");
            string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            if (result == String.Empty)
            {
                Console.WriteLine("MakeMeAvailable call succeeded");
                return true;
            }

            Console.WriteLine("MakeMeAvailable call failed");
            return false;
        }
    }
}
