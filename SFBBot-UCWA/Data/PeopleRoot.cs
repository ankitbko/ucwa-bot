using System;
using System.Collections.Generic;

namespace UcwaSfboConsole
{  
    public class PeopleRoot
    {
        public _Links _links { get; set; }
        public _Embeddeda _embedded { get; set; }
        public string rel { get; set; }
    }  


    public class _Embeddeda
    {
        public List<Contact> contact { get; set; }
    }

    public class Contact
    {
        public string uri { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string sourceNetwork { get; set; }
        public List<string> emailAddresses { get; set; }
        public _Links11 _links { get; set; }
        public string rel { get; set; }
        public string etag { get; set; }
        public DateTime expires { get; set; }
    }

    public class _Links11
    {
        public Self self { get; set; }
        public Self contactPhoto { get; set; }
        public Self contactPresence { get; set; }
        public Self contactLocation { get; set; }
        public Self contactNote { get; set; }
        public Self contactSupportedModalities { get; set; }
        public Contactprivacyrelationship contactPrivacyRelationship { get; set; }
    }       

    public class Contactprivacyrelationship
    {
        public string href { get; set; }
        public string revision { get; set; }
    }

}