using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SFBBot_UCWA
{
   public class ConfigData
    {
        public static AuthenticationResult ucwaAuthenticationResult;

        public static string ucwaApplication { get; set; }
     
        public static string ucwaEvents = string.Empty;
        public static string ucwaMakeMeAvailable { get; set; }
        public static string ucwaMessagingInvitations = "/communication/messagingInvitations";
      
        public static string ucwaConversation { get; set; }
        public static string ucwaStopMessaging ="/terminate";

        public static string ucwaApplicationsHost { get; set; }
        public static string ucwaMessaging = "";
        internal static string ucwaFilter= "communication/conversations?filter=active";

        public static string ucwaPeopleContact = "/people/contacts";
        
        public static void Log(string v1, string v2)
        {
            Console.WriteLine(v1 + " " + v2);
        }

    }
}
