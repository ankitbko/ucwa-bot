using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcwaSfboConsole3
{
    public partial class EventRoot   
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

    public partial class Event
    {
        public Link link { get; set; }
        public _Embedded _embedded { get; set; }
        public string type { get; set; }
    }

    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
        public string title { get; set; }
    }

    public partial class _Embedded
    {
        public Messaginginvitation messagingInvitation { get; set; }
        public Conversation2 conversation { get; set; }
        public Localparticipant localParticipant { get; set; }
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

    public partial class _Links1
    {
        public Self1 self { get; set; }
        public To to { get; set; }
        public Conversation conversation { get; set; }
        public Accept accept { get; set; }
        public Decline decline { get; set; }
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

    public class Accept
    {
        public string href { get; set; }
    }

    public class Decline
    {
        public string href { get; set; }
    }

    public class Messaging
    {
        public string href { get; set; }
    }

    public partial class Message
    {
        public string href { get; set; }
    }

    public class _Embedded1
    {
        public From from { get; set; }
    }

    public class From
    {
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

    public class Conversation2
    {
        public string state { get; set; }
        public string threadId { get; set; }
        public string subject { get; set; }
        public List<string> activeModalities { get; set; }
        public string importance { get; set; }
        public bool recording { get; set; }
        public _Links3 _links { get; set; }
        public string rel { get; set; }
    }

    public class _Links3
    {
        public Self3 self { get; set; }
        public Applicationsharing applicationSharing { get; set; }
        public Audiovideo audioVideo { get; set; }
        public Datacollaboration dataCollaboration { get; set; }
        public Messaging1 messaging { get; set; }
        public Phoneaudio phoneAudio { get; set; }
    }

    public class Self3
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

    public class Messaging1
    {
        public string href { get; set; }
    }

    public class Phoneaudio
    {
        public string href { get; set; }
    }

    public class Localparticipant
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
        public Self4 self { get; set; }
        public Conversation3 conversation { get; set; }
        public Me me { get; set; }
    }

    public class Self4
    {
        public string href { get; set; }
    }

    public class Conversation3
    {
        public string href { get; set; }
    }

    public class Me
    {
        public string href { get; set; }
    }

}
