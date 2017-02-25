using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcwaSfboConsole2
{   
    public class InMessageRoot
    {
        public _Links _links { get; set; }
        public Sender[] sender { get; set; }
    }

    public class _Links
    {
        public Self self { get; set; }
        public Self next { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }      

    public class Sender
    {
        public string rel { get; set; }
        public string href { get; set; }
        public Event[] events { get; set; }
    }

    public class Event
    {
        public Link link { get; set; }
        public _Embedded _embedded { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public Reason reason { get; set; }
        public In _in { get; set; }
    }

    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
        public string title { get; set; }
    }

    public class _Embedded
    {
        public Messaginginvitation messagingInvitation { get; set; }
        public Conversation2 conversation { get; set; }
        public Localparticipant1 localParticipant { get; set; }
        public Messaging2 messaging { get; set; }
        public Audiovideo1 audioVideo { get; set; }
        public Phoneaudio1 phoneAudio { get; set; }
        public Applicationsharing1 applicationSharing { get; set; }
        public Datacollaboration1 dataCollaboration { get; set; }
        public Message message { get; set; }
    }

    public class Messaginginvitation
    {
        public string direction { get; set; }
        public string importance { get; set; }
        public string threadId { get; set; }
        public string state { get; set; }
        public string operationId { get; set; }
        public string subject { get; set; }
        public _Links1 _links { get; set; }
        public string rel { get; set; }
        public _Embedded1 _embedded { get; set; }
    }

    public class _Links1
    {
        public Self self { get; set; }
        public From from { get; set; }
        public Self to { get; set; }
        public Self cancel { get; set; }
        public Self conversation { get; set; }
        public Self messaging { get; set; }
    }

    public class From
    {
        public string href { get; set; }
        public string title { get; set; }
    }

   

    public class _Embedded1
    {
        public Acceptedbyparticipant[] acceptedByParticipant { get; set; }
    }

    public class Acceptedbyparticipant
    {
        public bool inLobby { get; set; }
        public string sourceNetwork { get; set; }
        public bool anonymous { get; set; }
        public bool local { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public _Links2 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links2
    {
        public Self self { get; set; }
        public Self conversation { get; set; }
        public Self contact { get; set; }
        public Self contactPresence { get; set; }
        public Self contactPhoto { get; set; }
        public Self participantMessaging { get; set; }
    }
    
    public class Conversation2
    {
        public string state { get; set; }
        public string threadId { get; set; }
        public string subject { get; set; }
        public object[] activeModalities { get; set; }
        public string importance { get; set; }
        public bool recording { get; set; }
        public _Links3 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links3
    {
        public Self self { get; set; }
        public Self applicationSharing { get; set; }
        public Self audioVideo { get; set; }
        public Self dataCollaboration { get; set; }
        public Self messaging { get; set; }
        public Self phoneAudio { get; set; }
        public Localparticipant localParticipant { get; set; }
        public Self participants { get; set; }
        public Self addParticipant { get; set; }
    }
    
    public class Localparticipant
    {
        public string href { get; set; }
        public string title { get; set; }
    }

    public class Localparticipant1
    {
        public string sourceNetwork { get; set; }
        public bool anonymous { get; set; }
        public bool local { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public _Links4 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links4
    {
        public Self self { get; set; }
        public Self conversation { get; set; }
        public Self me { get; set; }
    }
  
    public class Messaging2
    {
        public string state { get; set; }
        public string[] negotiatedMessageFormats { get; set; }
        public _Links5 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links5
    {
        public Self self { get; set; }
        public Self conversation { get; set; }
        public Self stopMessaging { get; set; }
        public Self sendMessage { get; set; }
        public Self setIsTyping { get; set; }
        public Self typingParticipants { get; set; }
        public Self addMessaging { get; set; }
    }

  
    public class Audiovideo1
    {
        public string state { get; set; }
        public _Links6 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links6
    {
        public Self self { get; set; }
        public Self conversation { get; set; }
        public Self addAudioVideo { get; set; }
    }

    public class Phoneaudio1
    {
        public string state { get; set; }
        public _Links7 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links7
    {
        public Self self { get; set; }
        public Self conversation { get; set; }
    }

    public class Applicationsharing1
    {
        public string state { get; set; }
        public _Links8 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links8
    {
        public Self self { get; set; }
        public Self conversation { get; set; }
    }


    public class Datacollaboration1
    {
        public string state { get; set; }
        public _Links9 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links9
    {
        public Self self { get; set; }
        public Self conversation { get; set; }
    }

    public class Message
    {
        public string direction { get; set; }
        public DateTime timeStamp { get; set; }
        public _Links10 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links10
    {
        public Self self { get; set; }
        public Self messaging { get; set; }
        public Self contact { get; set; }
        public Participant participant { get; set; }
        public Self plainMessage { get; set; }
    }

    public class Participant
    {
        public string href { get; set; }
        public string title { get; set; }
    }
    
    public class In
    {
        public string rel { get; set; }
        public string href { get; set; }
        public string title { get; set; }
    }

    public class Reason
    {
        public string code { get; set; }
        public string subcode { get; set; }
        public string message { get; set; }
    }
}
