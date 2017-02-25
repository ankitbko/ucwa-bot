using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using UcwaSfboConsole;
using UcwaSfboConsole1;
using UcwaSfboConsole3;
using UcwaSfboConsole5;
//using UcwaSfboConsole6;

namespace SFBBot_UCWA.UcwaSfbo
{
    public class UcwaReciveMessage
    {

        async public static void GetMessage(HttpClient httpClient, AuthenticationResult ucwaAuthenticationResult, bool IsNextMsg = false)
        {
            ConfigData.ucwaAuthenticationResult = ucwaAuthenticationResult;
            GetIM_Step01_Application(httpClient);
        }

        async public static void GetIM_Step01_Application(HttpClient httpClient)
        {
            string destinationAddress = Program.destinationAddress;

            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaApplication;// + ConfigData.ucwaPeopleContact;

                ConfigData.Log("1", String.Format("Step 01 : POST : {0}", url_00));

                ConfigData.Log("3", String.Format(">> Request: {0}", "Get"));
                ConfigData.Log("3", String.Format(">> URL: {0}", url_00));


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
                    ConfigData.Log("2", String.Format(">> GetIM completed normally. {0}", "STEP01"));
                    ApplicationRoot obj = new ApplicationRoot();
                    JsonConvert.PopulateObject(res_00_content, obj);
                    if (obj != null)
                    {
                        ConfigData.ucwaApplication = obj._links.self.href;
                        ConfigData.ucwaEvents = obj._links.events.href;
                    }

                    //GetIM_Step02_Contact(httpClient);
                    GetIM_Step03_Events(httpClient);
                }
                else
                {
                    ConfigData.Log("2", String.Format(">> GetIM ended abnormally. {0}", "STEP01"));
                }
            }
            catch (Exception ex)
            {
                ConfigData.Log("2", String.Format(">> Error in step 01. {0}", ex.InnerException.Message));
            }
        }

        async public static void GetIM_Step03_Events(HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaEvents;

                ConfigData.Log("1", String.Format("Step 03 : GET : {0}", url_00));

                ConfigData.Log("3", String.Format(">> Request: {0}", "GET"));
                ConfigData.Log("3", String.Format(">> URL: {0}", url_00));

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
                    bool hendle = false;

                    EventRoot eventRoot = JsonConvert.DeserializeObject<EventRoot>(res_00_content);
                    //JsonConvert.PopulateObject(res_00_content, eventRoot);
                    if (eventRoot.sender?.Exists(s => s.events?.Exists(e => (e.status?.Equals("Success", StringComparison.OrdinalIgnoreCase) ?? false) && (e.type?.Equals("completed", StringComparison.OrdinalIgnoreCase) ?? false) && (e._embedded?.message?.direction?.Equals("Incoming", StringComparison.OrdinalIgnoreCase) ?? false)) ?? false) ?? false)
                    //if (res_00_content.Contains("Incoming") && res_00_content.Contains("completed"))
                    {
                        if (eventRoot != null)
                        {

                            if (eventRoot._links != null)
                            {
                                hendle = true;

                                List<UcwaSfboConsole3.Sender> sender = eventRoot.sender.FindAll(x => x.rel.Equals("conversation"));
                                if (sender != null)
                                {
                                    foreach (var item in sender)
                                    {
                                        List<UcwaSfboConsole3.Event> msgInvitation = item.events.FindAll(x => x.link.rel.Equals("message"));
                                        if (msgInvitation != null)
                                        {
                                            foreach (var item1 in msgInvitation)
                                            {
                                                if (item1._embedded != null && item1._embedded.message != null && item1._embedded.message.direction == "Incoming")
                                                {
                                                    string SendMessageUrl = item1._embedded.message._links.messaging.href;
                                                    ConfigData.ucwaEvents = eventRoot._links.next.href;

                                                    string message = string.Empty;

                                                    if (item1._embedded.message._links.htmlMessage != null)
                                                    {
                                                        message = Utilities.GetMessageFromHtml(item1._embedded.message._links.htmlMessage.href);
                                                    }
                                                    else if (item1._embedded.message._links.plainMessage != null)
                                                    {
                                                        message = Utilities.GetMessageFromHref(item1._embedded.message._links.plainMessage.href);
                                                    }

                                                    var conversationId = item.href.Split('/').Last();
                                                    var fromId = item1._embedded.message._links.contact.href.Split('/').Last();

                                                    Activity activity = new Activity()
                                                    {
                                                        From = new ChannelAccount { Id = conversationId, Name = conversationId },
                                                        Conversation = new ConversationAccount { Id = fromId },
                                                        Recipient = new ChannelAccount { Id = "Bot" },
                                                        ServiceUrl = "https://skype.botframework.com",
                                                        ChannelId = "skype",
                                                        ChannelData = SendMessageUrl
                                                    };

                                                    activity.Text = message;

                                                    using (var scope = Microsoft.Bot.Builder.Dialogs.Conversation
                                                        .Container.BeginLifetimeScope(DialogModule.LifetimeScopeTag, builder => Configure(builder)))
                                                    {
                                                        scope.Resolve<IMessageActivity>
                                                            (TypedParameter.From((IMessageActivity)activity));
                                                        DialogModule_MakeRoot.Register
                                                            (scope, () => new EchoDialog());
                                                        var postToBot = scope.Resolve<IPostToBot>();
                                                        await postToBot.PostAsync(activity, CancellationToken.None);
                                                    }

                                                    //await UcwaSendMessage.SendIM_Step05(httpClient, "echo " + message, SendMessageUrl);
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                            GetIM_Step03_Events(httpClient);
                        }
                    }
                    else if (eventRoot.sender?.Exists(s => s.events?.Exists(e => (e?._embedded?.messagingInvitation?.state?.Equals("Connecting", StringComparison.OrdinalIgnoreCase) ?? false) && (e.type?.Equals("started", StringComparison.OrdinalIgnoreCase) ?? false) && (e._embedded?.messagingInvitation?.direction?.Equals("Incoming", StringComparison.OrdinalIgnoreCase) ?? false)) ?? false) ?? false)
                    //else if (res_00_content.Contains("Incoming") && res_00_content.Contains("Connecting"))
                    {
                        string message = string.Empty;
                        string acceptUrl = string.Empty;
                        if (eventRoot != null)
                        {
                            if (eventRoot._links != null)
                            {
                                hendle = true;
                                ConfigData.ucwaEvents = eventRoot._links.next.href;

                                List<UcwaSfboConsole3.Sender> sender = eventRoot.sender.FindAll(x => x.rel.Equals("communication"));
                                if (sender != null)
                                {

                                    foreach (var item in sender)
                                    {
                                        List<UcwaSfboConsole3.Event> msgInvitation = item.events.FindAll(x => x.link.rel.Equals("messagingInvitation"));

                                        if (msgInvitation != null)
                                        {
                                            foreach (var item1 in msgInvitation)
                                            {
                                                if (item1._embedded != null && item1._embedded.messagingInvitation._links != null && item1._embedded.messagingInvitation._links.accept != null)
                                                {
                                                    acceptUrl = item1._embedded.messagingInvitation._links.accept.href;
                                                    message = Utilities.GetMessageFromHref(item1._embedded.messagingInvitation._links.message.href);
                                                    await GetIM_Step04_MessageAccept(httpClient, acceptUrl, message);
                                                }
                                            }
                                        }
                                    }
                                }
                                GetIM_Step03_Events(httpClient);
                            }
                        }
                    }
                    else // if (hendle == false)
                    {
                        MessageRoot obj = new MessageRoot();
                        JsonConvert.PopulateObject(res_00_content, obj);
                        if (obj != null)
                        {
                            if (obj._links != null)
                            {
                                ConfigData.ucwaEvents = obj._links.next.href;
                            }
                        }
                        GetIM_Step03_Events(httpClient);
                    }

                }
                else
                {
                    ConfigData.Log("2", String.Format(">> Error in step 03. {0}", "No OK received"));
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "A task was canceled.")
                {
                    GetIM_Step03_Events(httpClient);
                }
                else
                    ConfigData.Log("2", String.Format(">> Error in step 03. {0}", ex.InnerException.Message));
            }
        }

        private static void Configure(ContainerBuilder builder)
        {
            builder.RegisterType<BotToUserLync>()
               .As<IBotToUser>()
               .InstancePerLifetimeScope();
        }

        async public static Task GetIM_Step04_MessageAccept(HttpClient httpClient, string acceptUrl, string message)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                string url_00 = ConfigData.ucwaApplications + acceptUrl;

                ConfigData.Log("1", String.Format("Step 04 : GET : {0}", url_00));

                ConfigData.Log("3", String.Format(">> Request: {0}", "GET"));
                ConfigData.Log("3", String.Format(">> URL: {0}", url_00));
                //ConfigData.Log("3", String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));
                var res_00 = await httpClient.PostAsync(url_00, null);

                string res_00_request = res_00.RequestMessage.ToString();
                string res_00_headers = res_00.Headers.ToString();
                string res_00_status = res_00.StatusCode.ToString();
                var res_00_content = await res_00.Content.ReadAsStringAsync();

                ConfigData.Log("3", String.Format(">> Response: {0}", res_00_status));
                ConfigData.Log("3", String.Format("{0}", res_00_headers));
                ConfigData.Log("3", String.Format("\r\n{0}", res_00_content));
                if (res_00_status == "NoContent")
                {
                    //await GetIM_Step05_Event_Send_Msg(httpClient, message);
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
        async public static Task GetIM_Step05_Event_Send_Msg(HttpClient httpClient, string message)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaEvents;

                ConfigData.Log("1", String.Format("Step 05 : GET : {0}", url_00));

                ConfigData.Log("5", String.Format(">> Request: {0}", "GET"));
                ConfigData.Log("5", String.Format(">> URL: {0}", url_00));

                var res_00 = await httpClient.GetAsync(url_00);

                string res_00_request = res_00.RequestMessage.ToString();
                string res_00_headers = res_00.Headers.ToString();
                string res_00_status = res_00.StatusCode.ToString();
                var res_00_content = await res_00.Content.ReadAsStringAsync();


                ConfigData.Log("5", String.Format(">> Response: {0}", res_00_status));
                ConfigData.Log("5", String.Format("{0}", res_00_headers));
                ConfigData.Log("5", String.Format("\r\n{0}", res_00_content));

                if (res_00_status == "OK")
                {
                    SendMessageRoot obj = new SendMessageRoot();
                    JsonConvert.PopulateObject(res_00_content, obj);
                    if (obj != null)
                    {
                        if (obj._links != null)
                            ConfigData.ucwaEvents = obj._links.next.href;

                        List<UcwaSfboConsole5.Sender> senders = obj.sender.FindAll(x => x.rel.Equals("conversation") && x.events.Find(e => e._embedded != null && e._embedded.messaging != null && e._embedded.messaging._links != null && e._embedded.messaging._links.sendMessage != null) != null);
                        if (senders != null && senders.Count > 0)
                        {
                            string msgUrl = senders[0].events[0].link.href;
                            await UcwaSendMessage.SendIM_Step05(httpClient, "echo " + message, msgUrl);
                        }
                    }

                    // GetIM_Step03_Events(httpClient);
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
        async public static void GetIM_Step02_Contact(HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaApplication + ConfigData.ucwaPeopleContact;
                //string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaEvents;;
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
                    PeopleRoot obj = new PeopleRoot();
                    JsonConvert.PopulateObject(res_00_content, obj);
                    if (obj != null)
                    {
                        //ConfigData.ucwaApplication = eventRoot._links.self.href;
                        // ConfigData.ucwaApplications += ConfigData.ucwaApplication;
                        // ConfigData.ucwaEvents = eventRoot._links.events.href;
                    }

                    //if (string.IsNullOrEmpty(ConfigData.ucwaConversation))
                    GetIM_Step03_Events(httpClient);
                    //else
                    //    SendIM_Step04(httpClient);
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
        async public static void GetIM_Step06(HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                //httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

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
                    ConfigData.Log("6", String.Format(">> GetIM completed normally. {0}", "STEP06"));
                    EventRoot obj = new EventRoot();
                    JsonConvert.PopulateObject(res_00_content, obj);
                    if (obj != null)
                    {
                        ConfigData.ucwaEvents = obj._links.next.href;
                    }
                    GetIM_Step07(httpClient);
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
        async public static void GetIM_Step07(HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                //httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaEvents;
                ConfigData.Log("1", String.Format("Step 07 : GET : {0}", url_00));

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
                    ConfigData.Log("6", String.Format(">> GetIM completed normally. {0}", "STEP06"));
                    EventRoot obj = new EventRoot();
                    JsonConvert.PopulateObject(res_00_content, obj);
                    if (obj != null)
                    {
                        ConfigData.ucwaEvents = obj._links.next.href;
                    }
                    GetIM_Step07(httpClient);
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
        async public static Task GetIM_End(HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Remove("Accept");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigData.ucwaAuthenticationResult.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                string url_00 = ConfigData.ucwaApplications + ConfigData.ucwaApplication + ConfigData.ucwaFilter;

                ConfigData.Log("1", String.Format("Step 01 : POST : {0}", url_00));

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
                    GetIM_Step02_Contact(httpClient);
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
        #endregion
    }
}
