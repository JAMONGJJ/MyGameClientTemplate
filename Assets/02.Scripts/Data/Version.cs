using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace ClientTemplate
{
    namespace VersionInfo
    {

        [XmlRoot("DataTable")]
        public class Version
        {
            [XmlElement("Version")]
            public string version;
            [XmlElement("PlayStoreLink")]
            public string playStoreLink;
            [XmlElement("AppStoreConnectLink")]
            public string appStoreConnectLink;
        }
    }
}