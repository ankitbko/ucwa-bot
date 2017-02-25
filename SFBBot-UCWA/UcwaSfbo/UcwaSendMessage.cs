using System;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UcwaSfboConsole1;
using System.Threading.Tasks;
using SFBBot_UCWA;

namespace SFBBot_UCWA.UcwaSfbo
{
    public class UcwaSendMessage
    {
        // Presence options for UcwaPresenceOptions https://msdn.microsoft.com/en-us/library/office/dn323684.aspx
        public static bool IsNextMsg = false;
        public class UcwaMessagesObject
        {
            public string message { get; set; }
        }


        async public static void SendIM(HttpClient httpClient, AuthenticationResult ucwaAuthenticationResult)
        {
            ConfigData.ucwaAuthenticationResult = ucwaAuthenticationResult;
            // http://ucwa.lync.com/documentation/KeyTasks-Communication-OutgoingIMCall  
            // -----------------------------------------------------------------------------  
            // (ucwaMessagingInvitations == "") goto buttonSendIM_End;
            // -----------------------------------------------------------------------------  

            //await SendIM_End(httpClient);
            if (UcwaSendMessage.IsNextMsg)
            {
                Console.WriteLine("Please enter your message to send");
                string msg = Console.ReadLine();
                await SendIM_Step05(httpClient, msg);//, "", message);
            }
            else
            {
                UcwaSendMessage.IsNextMsg = true;
                SendIM_Step01(httpClient);
            }
             ;
        }

        async public static void SendIM_Step01(HttpClient httpClient)
        {
            string destinationAddress = Program.destinationAddress;

            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaApplication + ConfigData.ucwaMessagingInvitations;

                ConfigData.Log("1", String.Format("Step 01 : POST : {0}", url_00));
                string guiId_sessionContext = Guid.NewGuid().ToString();
                string guiId_operationId = Guid.NewGuid().ToString();
                string postdata_01 = "{\"importance\":\"Normal\"," +
                "\"sessionContext\":\"" + guiId_sessionContext + "\"," +
                "\"subject\":\"Test\"," +
                "\"telemetryId\":null," +
                "\"to\":\"" + destinationAddress + "\"," +
                "\"operationId\":\"" + guiId_operationId + "\"" +
                "}";
                HttpContent json_01 = new StringContent(postdata_01, Encoding.UTF8, "application/json");

                ConfigData.Log("3", String.Format(">> Request: {0}", "POST"));
                ConfigData.Log("3", String.Format(">> URL: {0}", url_00));
                // ConfigData.Log("3", String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));
                ConfigData.Log("3", String.Format(">> {0}={1}", "importance", "Normal"));
                ConfigData.Log("3", String.Format(">> {0}={1}", "sessionContext", guiId_sessionContext));
                ConfigData.Log("3", String.Format(">> {0}={1}", "subject", "Test"));
                ConfigData.Log("3", String.Format(">> {0}={1}", "telemetryId", "null"));
                ConfigData.Log("3", String.Format(">> {0}={1}", "to", destinationAddress));
                ConfigData.Log("3", String.Format(">> {0}={1}", "operationId", guiId_operationId));

                var res_00 = await httpClient.PostAsync(url_00, json_01);
                string res_00_request = res_00.RequestMessage.ToString();
                string res_00_headers = res_00.Headers.ToString();
                string res_00_status = res_00.StatusCode.ToString();
                var res_00_content = await res_00.Content.ReadAsStringAsync();

                ConfigData.Log("3", String.Format(">> Response: {0}", res_00_status));
                ConfigData.Log("3", String.Format("{0}", res_00_headers));
                ConfigData.Log("3", String.Format("\r\n{0}", res_00_content));

                if (res_00_status == "Created")
                {
                    ConfigData.Log("2", String.Format(">> SendIM completed normally. {0}", "STEP01"));
                    SendIM_Step02(httpClient);
                }
                else
                {
                    ConfigData.Log("2", String.Format(">> SendIM ended abnormally. {0}", "STEP01"));
                }
            }
            catch (Exception ex)
            {
                ConfigData.Log("2", String.Format(">> Error in step 01. {0}", ex.InnerException.Message));
            }
        }

        async public static void SendIM_Step02(HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaEvents;// .Replace('#', '2');
                ConfigData.Log("1", String.Format("Step 02 : GET : {0}", url_00));

                ConfigData.Log("3", String.Format(">> Request: {0}", "GET"));
                ConfigData.Log("3", String.Format(">> URL: {0}", url_00));
                // ConfigData.Log("3", String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));

                var res_00 = await httpClient.GetAsync(url_00);

                string res_00_request = res_00.RequestMessage.ToString();
                string res_00_headers = res_00.Headers.ToString();
                string res_00_status = res_00.StatusCode.ToString();
                var res_00_content = await res_00.Content.ReadAsStringAsync();

                ConfigData.Log("3", String.Format(">> Response: {0}", res_00_status));
                ConfigData.Log("3", String.Format("{0}", res_00_headers));
                ConfigData.Log("3", String.Format("\r\n{0}", res_00_content));

                if (res_00_status == "OK")
                {
                    MessageRoot obj = new MessageRoot();
                    JsonConvert.PopulateObject(res_00_content, obj);
                    ConfigData.ucwaEvents = obj._links.next.href;
                    if (obj != null)
                    {
                        if (obj.sender != null && obj.sender.Count > 0)
                        {
                            Sender sender = obj.sender.FindLast(x => x.rel == "conversation");
                            if (sender != null)
                                ConfigData.ucwaConversation = sender.href;

                            Sender sender1 = obj.sender.Find(x => x.rel == "stopMessaging");
                            if (sender1 != null)
                                ConfigData.ucwaStopMessaging = sender1.href;

                            Sender sender2 = obj.sender.Find(x => x.rel == "next");
                            if (sender2 != null)
                                ConfigData.ucwaEvents = sender2.href;
                        }
                    }
                    if (string.IsNullOrEmpty(ConfigData.ucwaConversation))
                        SendIM_Step03(httpClient);
                    else
                        SendIM_Step04(httpClient);
                }
                else
                {
                    ConfigData.Log("2", String.Format(">> Error in step 02. {0}", "No OK received"));
                }
            }
            catch (Exception ex)
            {
                ConfigData.Log("2", String.Format(">> Error in step 02. {0}", ex.InnerException.Message));
            }
        }
        async public static void SendIM_Step03(HttpClient httpClient, bool retry = false)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaEvents;//.Replace('#', '3');
                if (retry)
                    url_00 = ConfigData.ucwaApplications + ConfigData.ucwaEvents.Replace('#', '1');

                ConfigData.Log("1", String.Format("Step 03 : GET : {0}", url_00));

                ConfigData.Log("3", String.Format(">> Request: {0}", "GET"));
                ConfigData.Log("3", String.Format(">> URL: {0}", url_00));
                //ConfigData.Log("3", String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));

                var res_00 = await httpClient.GetAsync(url_00);

                string res_00_request = res_00.RequestMessage.ToString();
                string res_00_headers = res_00.Headers.ToString();
                string res_00_status = res_00.StatusCode.ToString();
                var res_00_content = await res_00.Content.ReadAsStringAsync();

                ConfigData.Log("3", String.Format(">> Response: {0}", res_00_status));
                ConfigData.Log("3", String.Format("{0}", res_00_headers));
                ConfigData.Log("3", String.Format("\r\n{0}", res_00_content));

                if (res_00_status == "OK")
                {

                    MessageRoot obj = new MessageRoot();
                    JsonConvert.PopulateObject(res_00_content, obj);

                    if (obj.sender != null && obj.sender.Count > 0)
                    {
                        Sender sender = obj.sender.FindLast(x => x.rel == "conversation");
                        if (sender != null)
                            ConfigData.ucwaConversation = sender.href;

                        Sender sender1 = obj.sender.Find(x => x.rel == "stopMessaging");
                        if (sender1 != null)
                            ConfigData.ucwaStopMessaging = sender1.href;

                        Sender sender2 = obj.sender.Find(x => x.rel == "next");
                        if (sender2 != null)
                            ConfigData.ucwaEvents = sender2.href;
                    }
                    SendIM_Step04(httpClient);
                }
                else
                {
                    ConfigData.Log("2", String.Format(">> Error in step 03. {0}", "No OK received"));
                }
            }
            catch (Exception ex)
            {
                ConfigData.Log("2", String.Format(">> Error in step 03. {0}", ex.InnerException.Message));
            }
        }

        async public static void SendIM_Step04(HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaConversation;

                ConfigData.Log("1", String.Format("Step 04 : GET : {0}", url_00));

                ConfigData.Log("3", String.Format(">> Request: {0}", "GET"));
                ConfigData.Log("3", String.Format(">> URL: {0}", url_00));
                //ConfigData.Log("3", String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));

                var res_00 = await httpClient.GetAsync(url_00);

                string res_00_request = res_00.RequestMessage.ToString();
                string res_00_headers = res_00.Headers.ToString();
                string res_00_status = res_00.StatusCode.ToString();
                var res_00_content = await res_00.Content.ReadAsStringAsync();

                ConfigData.Log("3", String.Format(">> Response: {0}", res_00_status));
                ConfigData.Log("3", String.Format("{0}", res_00_headers));
                ConfigData.Log("3", String.Format("\r\n{0}", res_00_content));
                if (res_00_status == "NotFound")
                {
                    SendIM_Step02(httpClient);
                }
                else if (res_00_status == "OK")
                {

                    ConfigData.ucwaMessaging = ConfigData.ucwaConversation + "/messaging";
                    MessageRoot obj = new MessageRoot();
                    JsonConvert.PopulateObject(res_00_content, obj);
                    //ConfigData.ucwaEvents = obj._links.next.href;
                    if (obj != null)
                    {
                        if (obj.sender != null && obj.sender.Count > 0)
                        {
                            Sender sender = obj.sender.Find(x => x.rel == "messaging");
                            if (sender != null)
                                ConfigData.ucwaMessaging = sender.href;
                        }
                    }

                    //    switch (xml04.GetAttribute("rel"))  case "messaging": ConfigData.ucwaMessaging));
                    await SendIM_Step05(httpClient);
                }
                else
                {
                    ConfigData.Log("2", String.Format(">> Error in step 04. {0}", "No OK received"));
                }
            }
            catch (Exception ex)
            {
                ConfigData.Log("2", String.Format(">> Error in step 04. {0}", ex.InnerException.Message));
            }
        }
        async public static Task SendIM_Step05(HttpClient httpClient, string msg = "Hello", string url = "")
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaMessaging + "/messages?OperationContext=" + Guid.NewGuid().ToString();

                if (url != "")
                {
                    url_00 = ConfigData.ucwaApplications + url + "/messages?OperationContext=" + Guid.NewGuid().ToString();
                }
                else
                    url_00 = ConfigData.ucwaApplications + ConfigData.ucwaMessaging + "/messages?OperationContext=" + Guid.NewGuid().ToString();

                ConfigData.Log("1", String.Format("Step 05 : POST : {0}", url_00));

                string postdata_05 = msg;
                HttpContent plain_05 = new StringContent(postdata_05, Encoding.UTF8, "text/plain");

                ConfigData.Log("3", String.Format(">> Request: {0}", "POST"));
                ConfigData.Log("3", String.Format(">> URL: {0}", url_00));
                // ConfigData.Log("3", String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));
                ConfigData.Log("3", String.Format("{0}", postdata_05));

                var res_00 = await httpClient.PostAsync(url_00, plain_05);
                string res_00_request = res_00.RequestMessage.ToString();
                string res_00_headers = res_00.Headers.ToString();
                string res_00_status = res_00.StatusCode.ToString();
                var res_00_content = await res_00.Content.ReadAsStringAsync();

                ConfigData.Log("3", String.Format(">> Response: {0}", res_00_status));
                ConfigData.Log("3", String.Format("{0}", res_00_headers));
                ConfigData.Log("3", String.Format("\r\n{0}", res_00_content));

                if (res_00_status == "Created")
                {
                    // SendIM_Step06(httpClient);
                    // ConfigData.Log("2", "Please enter your message to send");   
                    ConfigData.Log("2", "Enter command(login | get | msg | contact | meeting | presence | help | exit) > ");
                }
                else
                {
                    ConfigData.Log("2", String.Format(">> SendIM ended abnormally. {0}", "STEP05"));
                }
            }
            catch (Exception ex)
            {
                ConfigData.Log("2", String.Format(">> Error in step 05. {0}", ex.InnerException.Message));
            }
        }


        #region Not Used
        async public static Task SendIM_End(HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                // string url_00 = ConfigData.ucwaApplications +ConfigData.ucwaMessaging + ConfigData.ucwaStopMessaging;

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaApplication + ConfigData.ucwaFilter;

                ConfigData.Log("1", String.Format("Step 01 : POST : {0}", url_00));

                // HttpContent json_01 = new StringContent(postdata_01, Encoding.UTF8, "application/json");

                ConfigData.Log("3", String.Format(">> Request: {0}", "POST"));
                ConfigData.Log("3", String.Format(">> URL: {0}", url_00));


                var res_00 = await httpClient.PostAsync(url_00, null);//, json_01);
                string res_00_request = res_00.RequestMessage.ToString();
                string res_00_headers = res_00.Headers.ToString();
                string res_00_status = res_00.StatusCode.ToString();
                var res_00_content = await res_00.Content.ReadAsStringAsync();

                ConfigData.Log("3", String.Format(">> Response: {0}", res_00_status));
                ConfigData.Log("3", String.Format("{0}", res_00_headers));
                ConfigData.Log("3", String.Format("\r\n{0}", res_00_content));

                if (res_00_status == "Created")
                {
                    ConfigData.Log("2", String.Format(">> SendIM completed normally. {0}", "STEP01"));
                    SendIM_Step02(httpClient);
                }
                else
                {
                    ConfigData.Log("2", String.Format(">> SendIM ended abnormally. {0}", "STEP01"));
                }
            }
            catch (Exception ex)
            {
                ConfigData.Log("2", String.Format(">> Error in step 01. {0}", ex.InnerException.Message));
            }
        }

        public static String GetPresenceUri(String createUcwaAppsResults, String ucwaApplicationHostRootUri)
        {
            dynamic createUcwaAppsResultsObject = JObject.Parse(createUcwaAppsResults);
            string getPresenceUri = String.Empty;

            try
            {
                getPresenceUri = ucwaApplicationHostRootUri +
                   createUcwaAppsResultsObject._embedded.me._links.presence.href;
            }
            catch
            {

            }
            Console.WriteLine("getPresenceUri is " + getPresenceUri);
            return getPresenceUri;
        }

        //const string ucwaApplications = "https://webpoolpnqin102.infra.lync.com/ucwa/oauth/v1/applications/102765521395";
        public static void SendMessage(HttpClient httpClient, AuthenticationResult ucwaAuthenticationResult, String getPresenceUri, UcwaMessagesObject message)
        {
            getPresenceUri = "https://webpoolpnqin102.infra.lync.com/ucwa/oauth/v1/applications/102765521395/communication/conversations/102997947959/messaging/sendMessage";
            string setPresenceResults = string.Empty;
            Console.WriteLine("URI is " + getPresenceUri);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ucwaAuthenticationResult.AccessToken);
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //var setPresencePostData = JsonConvert.SerializeObject(message);
            var setPresencePostData = "hello";
            Console.WriteLine("SetPresence POST data is " + setPresencePostData);
            var httpResponseMessage =
                httpClient.PostAsync(getPresenceUri, new StringContent(setPresencePostData, Encoding.UTF8,
                "text/plain")).Result;
            Console.WriteLine("Send Message response is " + httpResponseMessage.Content.ReadAsStringAsync().Result);
            Console.WriteLine("Send Message response should be empty");
        }
        async public static void SendIM_Step06(HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaEvents;
                ConfigData.Log("1", String.Format("Step 06 : GET : {0}", url_00));

                ConfigData.Log("3", String.Format(">> Request: {0}", "GET"));
                ConfigData.Log("3", String.Format(">> URL: {0}", url_00));
                // ConfigData.Log("3", String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));

                var res_00 = await httpClient.GetAsync(url_00);

                string res_00_request = res_00.RequestMessage.ToString();
                string res_00_headers = res_00.Headers.ToString();
                string res_00_status = res_00.StatusCode.ToString();
                var res_00_content = await res_00.Content.ReadAsStringAsync();

                ConfigData.Log("3", String.Format(">> Response: {0}", res_00_status));
                ConfigData.Log("3", String.Format("{0}", res_00_headers));
                ConfigData.Log("3", String.Format("\r\n{0}", res_00_content));

                if (res_00_status == "OK")
                {
                    XmlTextReader xml06 = new XmlTextReader(new StringReader(res_00_content));
                    //while (xml06.Read())
                    //{
                    //    switch (xml06.GetAttribute("rel"))
                    //    {
                    //        case "next":
                    //            ConfigData.ucwaEvents = xml06.GetAttribute("href");
                    //            ConfigData.Log("2", String.Format(">> ucwaEvents : {0}", ConfigData.ucwaEvents));
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}
                    SendIM_Step07(httpClient);
                }
                else
                {
                    ConfigData.Log("2", String.Format(">> Error in step 06. {0}", "No OK received"));
                }
            }
            catch (Exception ex)
            {
                ConfigData.Log("2", String.Format(">> Error in step 06. {0}", ex.InnerException.Message));
            }
        }
        async public static void SendIM_Step07(HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaMessaging + ConfigData.ucwaStopMessaging;
                ConfigData.Log("1", String.Format("Step 07 : POST : {0}", url_00));

                string postdata_07 = "";
                HttpContent plain_07 = new StringContent(postdata_07, Encoding.UTF8, "text/plain");

                ConfigData.Log("3", String.Format(">> Request: {0}", "POST"));
                ConfigData.Log("3", String.Format(">> URL: {0}", url_00));
                // ConfigData.Log("3", String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));

                var res_00 = await httpClient.PostAsync(url_00, plain_07);
                string res_00_request = res_00.RequestMessage.ToString();
                string res_00_headers = res_00.Headers.ToString();
                string res_00_status = res_00.StatusCode.ToString();
                var res_00_content = await res_00.Content.ReadAsStringAsync();

                ConfigData.Log("3", String.Format(">> Response: {0}", res_00_status));
                ConfigData.Log("3", String.Format("{0}", res_00_headers));
                ConfigData.Log("3", String.Format("\r\n{0}", res_00_content));

                if (res_00_status == "NoContent")
                {
                    ConfigData.Log("2", String.Format(">> SendIM completed. {0}", "Conversation ended"));
                }
                else
                {
                    ConfigData.Log("2", String.Format(">> SendIM ended abnormally. {0}", "STEP07"));
                }
            }
            catch (Exception ex)
            {
                ConfigData.Log("2", String.Format(">> Error in step 07. {0}", ex.InnerException.Message));
            }
        }
        #endregion

    }
}
