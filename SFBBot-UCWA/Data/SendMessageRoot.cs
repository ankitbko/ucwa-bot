using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcwaSfboConsole5
{
    class SendMessageRoot   
    {
        public _Links _links { get; set; }
        public List<Sender> sender { get; set; }
    }

    public class _Links
    {
        public Self self { get; set; }
        public Next next { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Next
    {
        public string href { get; set; }
    }

    public class Sender
    {
        public string rel { get; set; }
        public string href { get; set; }
        public List<Event> events { get; set; }
    }

    public class Event
    {
        public Link link { get; set; }
        public string type { get; set; }
        public _Embedded _embedded { get; set; }
        public In _in { get; set; }
        public string status { get; set; }
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
        public Messaging1 messaging { get; set; }
        public Conversation3 conversation { get; set; }
        public Audiovideo1 audioVideo { get; set; }
        public Phoneaudio1 phoneAudio { get; set; }
        public Applicationsharing1 applicationSharing { get; set; }
        public Datacollaboration1 dataCollaboration { get; set; }
        public Participantmessaging participantMessaging { get; set; }
    }

    public class Messaginginvitation
    {
        public string direction { get; set; }
        public string importance { get; set; }
        public string threadId { get; set; }
        public string state { get; set; }
        public string subject { get; set; }
        public _Links1 _links { get; set; }
        public _Embedded1 _embedded { get; set; }
        public string rel { get; set; }
    }

    public class _Links1
    {
        public Self1 self { get; set; }
        public To to { get; set; }
        public Conversation conversation { get; set; }
        public Messaging messaging { get; set; }
        public Message message { get; set; }
    }

    public class Self1
    {
        public string href { get; set; }
    }

    public class To
    {
        public string href { get; set; }
    }

    public class Conversation
    {
        public string href { get; set; }
    }

    public class Messaging
    {
        public string href { get; set; }
    }

    public class Message
    {
        public string href { get; set; }
    }

    public class _Embedded1
    {
        public From from { get; set; }
    }

    public class From
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
        public Self2 self { get; set; }
        public Conversation1 conversation { get; set; }
        public Contact contact { get; set; }
        public Contactpresence contactPresence { get; set; }
        public Contactphoto contactPhoto { get; set; }
    }

    public class Self2
    {
        public string href { get; set; }
    }

    public class Conversation1
    {
        public string href { get; set; }
    }

    public class Contact
    {
        public string href { get; set; }
    }

    public class Contactpresence
    {
        public string href { get; set; }
    }

    public class Contactphoto
    {
        public string href { get; set; }
    }

    public class Messaging1
    {
        public string state { get; set; }
        public List<string> negotiatedMessageFormats { get; set; }
        public _Links3 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links3
    {
        public Self3 self { get; set; }
        public Conversation2 conversation { get; set; }
        public Stopmessaging stopMessaging { get; set; }
        public Sendmessage sendMessage { get; set; }
        public Setistyping setIsTyping { get; set; }
        public Typingparticipants typingParticipants { get; set; }
    }

    public class Self3
    {
        public string href { get; set; }
    }

    public class Conversation2
    {
        public string href { get; set; }
    }

    public class Stopmessaging
    {
        public string href { get; set; }
    }

    public class Sendmessage
    {
        public string href { get; set; }
    }

    public class Setistyping
    {
        public string href { get; set; }
    }

    public class Typingparticipants
    {
        public string href { get; set; }
    }

    public class Conversation3
    {
        public string state { get; set; }
        public string threadId { get; set; }
        public string subject { get; set; }
        public string[] activeModalities { get; set; }
        public string importance { get; set; }
        public bool recording { get; set; }
        public _Links4 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links4
    {
        public Self4 self { get; set; }
        public Applicationsharing applicationSharing { get; set; }
        public Audiovideo audioVideo { get; set; }
        public Datacollaboration dataCollaboration { get; set; }
        public Messaging2 messaging { get; set; }
        public Phoneaudio phoneAudio { get; set; }
        public Localparticipant localParticipant { get; set; }
        public Participants participants { get; set; }
        public Addparticipant addParticipant { get; set; }
    }

    public class Self4
    {
        public string href { get; set; }
    }

    public class Applicationsharing
    {
        public string href { get; set; }
    }

    public class Audiovideo
    {
        public string href { get; set; }
    }

    public class Datacollaboration
    {
        public string href { get; set; }
    }

    public class Messaging2
    {
        public string href { get; set; }
    }

    public class Phoneaudio
    {
        public string href { get; set; }
    }

    public class Localparticipant
    {
        public string href { get; set; }
        public string title { get; set; }
    }

    public class Participants
    {
        public string href { get; set; }
    }

    public class Addparticipant
    {
        public string href { get; set; }
    }

    public class Audiovideo1
    {
        public string state { get; set; }
        public _Links5 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links5
    {
        public Self5 self { get; set; }
        public Conversation4 conversation { get; set; }
        public Addaudiovideo addAudioVideo { get; set; }
    }

    public class Self5
    {
        public string href { get; set; }
    }

    public class Conversation4
    {
        public string href { get; set; }
    }

    public class Addaudiovideo
    {
        public string href { get; set; }
    }

    public class Phoneaudio1
    {
        public string state { get; set; }
        public _Links6 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links6
    {
        public Self6 self { get; set; }
        public Conversation5 conversation { get; set; }
    }

    public class Self6
    {
        public string href { get; set; }
    }

    public class Conversation5
    {
        public string href { get; set; }
    }

    public class Applicationsharing1
    {
        public string state { get; set; }
        public _Links7 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links7
    {
        public Self7 self { get; set; }
        public Conversation6 conversation { get; set; }
    }

    public class Self7
    {
        public string href { get; set; }
    }

    public class Conversation6
    {
        public string href { get; set; }
    }

    public class Datacollaboration1
    {
        public string state { get; set; }
        public _Links8 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links8
    {
        public Self8 self { get; set; }
        public Conversation7 conversation { get; set; }
    }

    public class Self8
    {
        public string href { get; set; }
    }

    public class Conversation7
    {
        public string href { get; set; }
    }

    public class Participantmessaging
    {
        public _Links9 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links9
    {
        public Self9 self { get; set; }
        public Participant participant { get; set; }
    }

    public class Self9
    {
        public string href { get; set; }
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

}
