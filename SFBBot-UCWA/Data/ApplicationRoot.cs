using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcwaSfboConsole
{
    public class ApplicationRoot
    {
        public string culture { get; set; }
        public string userAgent { get; set; }
        public string type { get; set; }
        public string endpointId { get; set; }
        public string instanceId { get; set; }
        public string id { get; set; }
        public _Links _links { get; set; }
        public _Embedded _embedded { get; set; }
        public string rel { get; set; }
        public string etag { get; set; }
        public DateTime expires { get; set; }
    }

    public class _Links
    {
        public Self self { get; set; }
        public Self policies { get; set; }
        public Self batch { get; set; }
        public Self events { get; set; }
        public Self next { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }
  

    public class _Embedded
    {
        public Me me { get; set; }
        public People people { get; set; }
        public Onlinemeetings onlineMeetings { get; set; }
        public Communication communication { get; set; }
    }

    public class Me
    {
        public string uri { get; set; }
        public string name { get; set; }
        public List<string> emailAddresses { get; set; }
        public _Links1 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links1
    {
        public Self self { get; set; }
        public Makemeavailable makeMeAvailable { get; set; }
        public Self photo { get; set; }
    }
 

    public class Makemeavailable
    {
        public string href { get; set; }
        public string revision { get; set; }
    }   

    public class People
    {
        public _Links2 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links2
    {
        public Self self { get; set; }
        public Self presenceSubscriptions { get; set; }
        public Self subscribedContacts { get; set; }
        public Self presenceSubscriptionMemberships { get; set; }
        public Mygroups myGroups { get; set; }
        public Mygroupmemberships myGroupMemberships { get; set; }
        public Mycontacts myContacts { get; set; }
        public Myprivacyrelationships myPrivacyRelationships { get; set; }
        public Mycontactsandgroupssubscription myContactsAndGroupsSubscription { get; set; }
        public Search search { get; set; }
    }
      

    public class Mygroups
    {
        public string href { get; set; }
        public string revision { get; set; }
    }

    public class Mygroupmemberships
    {
        public string href { get; set; }
        public string revision { get; set; }
    }

    public class Mycontacts
    {
        public string href { get; set; }
    }

    public class Myprivacyrelationships
    {
        public string href { get; set; }
    }

    public class Mycontactsandgroupssubscription
    {
        public string href { get; set; }
    }

    public class Search
    {
        public string href { get; set; }
        public string revision { get; set; }
    }

    public class Onlinemeetings
    {
        public _Links3 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links3
    {
        public Self3 self { get; set; }
        public Myonlinemeetings myOnlineMeetings { get; set; }
        public Onlinemeetingdefaultvalues onlineMeetingDefaultValues { get; set; }
        public Onlinemeetingeligiblevalues onlineMeetingEligibleValues { get; set; }
        public Onlinemeetinginvitationcustomization onlineMeetingInvitationCustomization { get; set; }
        public Onlinemeetingpolicies onlineMeetingPolicies { get; set; }
        public Phonedialininformation phoneDialInInformation { get; set; }
    }

    public class Self3
    {
        public string href { get; set; }
    }

    public class Myonlinemeetings
    {
        public string href { get; set; }
    }

    public class Onlinemeetingdefaultvalues
    {
        public string href { get; set; }
    }

    public class Onlinemeetingeligiblevalues
    {
        public string href { get; set; }
    }

    public class Onlinemeetinginvitationcustomization
    {
        public string href { get; set; }
    }

    public class Onlinemeetingpolicies
    {
        public string href { get; set; }
    }

    public class Phonedialininformation
    {
        public string href { get; set; }
    }

    public class Communication
    {
        public string videoBasedScreenSharing { get; set; }
        public string b8590b4cdf764bd39490ae2805ee149b { get; set; }
        public List<object> supportedModalities { get; set; }
        public List<string> supportedMessageFormats { get; set; }
        public string audioPreference { get; set; }
        public string conversationHistory { get; set; }
        public bool publishEndpointLocation { get; set; }
        public _Links4 _links { get; set; }
        public string rel { get; set; }
        public string etag { get; set; }
    }

    public class _Links4
    {
        public Self4 self { get; set; }
        public Mediarelayaccesstoken mediaRelayAccessToken { get; set; }
        public Mediapolicies mediaPolicies { get; set; }
        public Conversations conversations { get; set; }
        public Startmessaging startMessaging { get; set; }
        public Startaudiovideo startAudioVideo { get; set; }
        public Startonlinemeeting startOnlineMeeting { get; set; }
        public Joinonlinemeeting joinOnlineMeeting { get; set; }
        public Misseditems missedItems { get; set; }
    }

    public class Self4
    {
        public string href { get; set; }
    }

    public class Mediarelayaccesstoken
    {
        public string href { get; set; }
    }

    public class Mediapolicies
    {
        public string href { get; set; }
    }

    public class Conversations
    {
        public string href { get; set; }
    }

    public class Startmessaging
    {
        public string href { get; set; }
        public string revision { get; set; }
    }

    public class Startaudiovideo
    {
        public string href { get; set; }
        public string revision { get; set; }
    }

    public class Startonlinemeeting
    {
        public string href { get; set; }
    }

    public class Joinonlinemeeting
    {
        public string href { get; set; }
    }

    public class Misseditems
    {
        public string href { get; set; }
    }

}
