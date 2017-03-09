using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFBBot_UCWA.UcwaSfbo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static SFBBot_UCWA.UcwaSfbo.UcwaApplications;

namespace SFBBot_UCWA
{
    class Program
    {
        public delegate void DispatchedHandler();
        // replace tenant with the name of your Azure AD instance
        // this is usually in the form of your-tenant.onmicrosoft.com

        // replace clientID with the clientID of the SFBO native app you created
        // in your Azure AD instance.  

        //my azure AD
        private static string tenant = "";
        private static string clientId = "";

        // make sure you grant at least these three permissions to your app
        // Create Skype Meetings
        // Initiate conversations and join meetings
        // Read/write Skype user information (preview)

        private static string hardcodedUsername = "Ankit@OmBot.onmicrosoft.com";
        private static string hardcodedPassword = "Mind@123";
        internal static string destinationAddress = "sip:Shinha@OmBot.onmicrosoft.com";


        // replace redirectUri with the redirect URI of the native app you created
        // in your Azure AD instance.  you will only need this if you choose to login
        // using the dialog option.  If you're following the example in the README, you will have
        // used the value https://demo-sfbo-ucwa
        private static string redirectUri = "https://localhost:44362/";

        // NO NEED TO CHANGE ANYTHING BELOW THIS LINE

        // sfboResourceAppId is a constant you don't have to change
        private static string sfboResourceAppId = "00000004-0000-0ff1-ce00-000000000000";
        private static string aadInstance = "https://login.microsoftonline.com/{0}";

        // authenticationContext is initialized with the values of your
        // aadInstance and tenant

        // if aadInstance and tenant are null, you won't be able to launch
        private static AuthenticationContext authenticationContext = null;

        // ucwaApplicationsUri - stores the UCWA applications resource uri
        // as returned by the autodiscovery process
        // i.e. https://webpoolXY.infra.lync.com/ucwa/oauth/v1/applications
        private static string ucwaApplicationsUri = "";

        // ucwaApplicationsHost - stores the UCWA application resource
        // protocol and hostname, as derived from ucwaApplicationsUri
        // i.e. https://webpoolXY.infra.lync.com/
        // you will combine ucwaApplicationsHost with links to individual
        // UCWA application resources as stored in the createUcwaAppsResults
        // string as described next

        private static string ucwaApplicationsHost = "";

        // createUcwaAppsResults - stores the result of making a POST call to
        // ucwaApplicationsUri.  This is a JSON string that contains the link to
        // UCWA application resources, such as:
        // me, people, onlineMeetings, and communciation

        private static string createUcwaAppsResults = "";

        // ucwaAuthenticationResult - stores the result of the Azure AD auth call
        // against ucwaApplicationsHost.  It's used in API calls against
        // UCWA app resources

        private static AuthenticationResult ucwaAuthenticationResult = null;

        // Be resource efficient and declare and re-use single System.Net.Http.HttpClient 
        // for use across your entire app.  Otherwise you'll run out of resources over time
        // httpClient is thread and re-entrant safe

        // You will need to pass an httpClient to each UCWA network operation

        internal static HttpClient httpClient = new HttpClient();

        static void Main(string[] args)
        {
            string commandString = string.Empty;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("**************************************************");
            Console.WriteLine("*  Azure AD + Skype for Business Online via UCWA *");
            Console.WriteLine("**************************************************");
            Console.WriteLine("");
            Console.WriteLine("**************************************************");
            Console.WriteLine(" tentant is " + tenant);
            Console.WriteLine(" clientId is " + clientId);
            Console.WriteLine(" redirectUri is " + redirectUri);
            Console.WriteLine("**************************************************");

            if (tenant == "" && clientId == "")
            {
                Console.WriteLine("You need to provide your Azure AD tenant name");
                Console.WriteLine("and application clientId in Program.cs before");
                Console.WriteLine("you can run this app");
                return;
            }
            else
            {
                authenticationContext = new AuthenticationContext
                    (string.Format(CultureInfo.InvariantCulture, aadInstance, tenant));
            }

            Login(false);
            Presence(true);
            GetMessage(true);

            while (!commandString.Equals("Exit", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("Enter exit to exit");
                commandString = Console.ReadLine();
                //commandString = "msg";
                switch (commandString.ToUpper())
                {
                    case "EXIT":
                        Console.WriteLine("Bye!"); ;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid command.");
                        break;
                }
            }
        }

        #region Textual UX

        // Gather user credentials form the command line 

        static UserCredential GetUserCredentials()
        {

            if (hardcodedUsername == String.Empty && hardcodedPassword == String.Empty)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Please enter username and password to sign in.");
                Console.WriteLine("We'll append @" + tenant + " to the username if you don't provide a domain");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("User>");
                string user = Console.ReadLine();
                if (!user.Contains("@"))
                {
                    user += "@" + tenant;
                    Console.WriteLine("We'll try to login as " + user);
                }
                Console.WriteLine("Password>");
                string password = ReadPasswordFromConsole();
                Console.WriteLine("");
                return new UserPasswordCredential(user, password);
            }
            return new UserPasswordCredential(hardcodedUsername, hardcodedPassword);

        }

        // Obscure the password being entered
        static string ReadPasswordFromConsole()
        {
            string password = string.Empty;
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);
            return password;
        }
        #endregion

        #region Commands

        // Login to the user's account and create a UCWA app

        async static void Login(bool IsAuto = false)
        {

            // Clear any cached tokens.
            // We do this to ensure logins with different accounts work 
            // during the same launch of the app

            authenticationContext.TokenCache.Clear();
            string loginStyle = "console";

            Console.WriteLine("How do you want to login?");

            Console.WriteLine("console | dialog | code >");
            if (!IsAuto)
                loginStyle = Console.ReadLine();

            AuthenticationResult testCredentials = null;
            UserCredential uc = null;

            switch (loginStyle.ToLower())
            {
                case "console":
                    uc = GetUserCredentials();
                    testCredentials = UcwaSfbo.AzureAdAuth.GetAzureAdToken(authenticationContext, sfboResourceAppId, clientId, redirectUri, uc);
                    break;
                case "dialog":
                    if (redirectUri == String.Empty)
                    {
                        Console.WriteLine("You haven't defined redirectUri which is needed if you want to sign in with a dialog");
                        return;
                    }
                    testCredentials = UcwaSfbo.AzureAdAuth.GetAzureAdToken(authenticationContext, sfboResourceAppId, clientId, redirectUri, uc);
                    break;
                case "code":
                    DeviceCodeResult deviceCodeResult = authenticationContext.AcquireDeviceCodeAsync(sfboResourceAppId, clientId).Result;
                    Console.WriteLine(deviceCodeResult.Message);
                    Console.WriteLine("Or, use Control-C to exit the app");
                    testCredentials = authenticationContext.AcquireTokenByDeviceCodeAsync(deviceCodeResult).Result;
                    break;
                default:
                    Console.Write("Please select a login style and try again");
                    Console.Write("\n");
                    return;
            }

            if (testCredentials == null)
            {
                Console.WriteLine("We encountered an Azure AD error");
                Console.WriteLine("Check your tenant, clientID, and credentials");
                return;
            }
            ucwaApplicationsUri = UcwaAutodiscovery.GetUcwaRootUri(httpClient, authenticationContext, sfboResourceAppId, clientId, redirectUri, uc);

            Console.WriteLine("We'll store the base UCWA app URI for use with UCWA app calls");
            Console.WriteLine("We prefix this to the links returned from the UCWA apps POST");
            Console.WriteLine("Since these links aren't full URIs");
            ucwaApplicationsHost = Utilities.ReduceUriToProtoAndHost(ucwaApplicationsUri);
            ConfigData.ucwaApplicationsHost = ucwaApplicationsHost;
            Console.WriteLine("ucwaApplicationsHost is " + ucwaApplicationsHost);

            Console.WriteLine("Get a token to access the user's UCWA Applications Resources from Azure AD.");
            Console.WriteLine("We can re-use this token for each UCWA app call");
            ucwaAuthenticationResult = AzureAdAuth.GetAzureAdToken(authenticationContext, ucwaApplicationsHost, clientId, redirectUri, uc);

            Console.WriteLine("Now we'll create and/or query UCWA Apps via POST");
            Console.WriteLine("Well create a UCWA apps object to pass to CreateUcwaApps");

            List<string> Modalities = new List<string>();
            Modalities.Add("PhoneAudio");
            Modalities.Add("Messaging");

            UcwaMyAppsObject ucwaMyAppsObject = new UcwaMyAppsObject()
            {
                UserAgent = "myAgent1",
                EndpointId = Guid.NewGuid().ToString(),
                Culture = "en-US"
            };

            Console.WriteLine("Making request to ucwaApplicationsUri " + ucwaApplicationsUri);
            createUcwaAppsResults = CreateUcwaApps(httpClient, ucwaAuthenticationResult, ucwaApplicationsUri, ucwaMyAppsObject);

            return;
        }

        static void Presence(bool IsAuto = false)
        {
            if (ucwaAuthenticationResult == null)
            {
                Console.WriteLine("You haven't logged in yet!");
                return;
            }

            Console.WriteLine("Please enter which presence value you want");
            foreach (var v in UcwaPresence.UcwaPresenceOptions)
            {
                Console.Write(v.ToString() + " ");
            }
            Console.Write("\n");
            Console.WriteLine("Presence>");
            string userPresence;
            if (IsAuto)
                userPresence = "online";
            else
                userPresence = Console.ReadLine();

            if (!UcwaPresence.UcwaPresenceOptions.Contains(userPresence, StringComparer.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("You didn't pick an option from the list");
                Console.WriteLine("Enter presence if you want to try again");
                return;
            }

            Console.WriteLine("MakeMeAvailable has to be called before you can set a user's presence");
            Console.WriteLine("UCWA lets you set a user's presence when calling MakeMeAvailable, so we'll try it");
            Console.WriteLine("We'll create a UcwaMakeMeAvailableObject that sets the user's presence as " + userPresence);
            List<string> MessageFormats = new List<string>();
            MessageFormats.Add("Plain");
            MessageFormats.Add("Html");

            List<string> Modalities = new List<string>();
            // Modalities.Add("PhoneAudio");
            Modalities.Add("Messaging");

            UcwaMakeMeAvailable.UcwaMakeMeAvailableObject ucwaMakeMeAvailableObject = new UcwaMakeMeAvailable.UcwaMakeMeAvailableObject
            {
                phoneNumber = "21286709",
                signInAs = userPresence,
                supportedMessageFormats = MessageFormats,
                supportedModalities = Modalities

            };

            Console.WriteLine("createUcwaAppsResults is a JSON string containing links to all resources");
            Console.WriteLine("We will parse to find MakeMeAvailable");

            var ucwaMakeMeAvailableRootUri = UcwaMakeMeAvailable.GetMakeMeAvailableUri(createUcwaAppsResults, ucwaApplicationsHost);
            if (ucwaMakeMeAvailableRootUri != String.Empty)
            {

                if (UcwaMakeMeAvailable.MakeMeAvailable(httpClient, ucwaAuthenticationResult, ucwaMakeMeAvailableRootUri, ucwaMakeMeAvailableObject))
                {
                    return;
                }
                //else
                //{
                //    Console.WriteLine("Looks like we encountered an error.  Wait before trying this again");
                //    return;
                //}
            }

            Console.WriteLine("Whoops! MakeMeAvailable isn't in createUcwaAppsResults");
            Console.WriteLine("The user is already available, let's simply change their presence to away");

            Console.WriteLine("createUcwaAppsResults is a JSON string containing links to all resources");
            Console.WriteLine("We will parse to find presence");
            var ucwaPresenceRootUri = UcwaPresence.GetPresenceUri(createUcwaAppsResults, ucwaApplicationsHost);

            Console.WriteLine("We'll create a UcwaPresenceObject that sets the user's presence to be the same as we intended in UcwaMakeMeAvailableObject");

            UcwaPresence.UcwaPresenceObject ucwaPresenceObject = new UcwaPresence.UcwaPresenceObject
            {
                availability = ucwaMakeMeAvailableObject.signInAs
            };
            UcwaPresence.SetPresence(httpClient, ucwaAuthenticationResult, ucwaPresenceRootUri, ucwaPresenceObject);
        }

        static void GetMessage(bool IsNextMsg = false)
        {
            if (ucwaAuthenticationResult == null)
            {
                Console.WriteLine("You haven't logged in yet!");
                return;
            }

            UcwaReciveMessage.GetMessage(httpClient, ucwaAuthenticationResult, IsNextMsg);//, "", message);
        }
        #endregion
    }
}
