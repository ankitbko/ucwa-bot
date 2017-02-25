using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcwaSfboConsole3
{
    public partial class EventRoot
    {
    }

    public partial class Event
    {
        public string status { get; set; }
    }

    public partial class _Embedded
    {
        public Message message { get; set; }
    }

    public partial class Message
    {
        public string direction { get; set; }
        public DateTime timeStamp { get; set; }
        public _Links1 _links { get; set; }
        public string rel { get; set; }
    }

    public partial class _Links1
    {
        public Contact contact { get; set; }
        public Participant participant { get; set; }
        public Plainmessage plainMessage { get; set; }
        public HtmlMessage htmlMessage { get; set; }
    }


    public class Participant
    {
        public string href { get; set; }
        public string title { get; set; }
    }
    public class Plainmessage
    {
        public string href { get; set; }
    }

    public class HtmlMessage
    {
        public string href { get; set; }
    }
}
