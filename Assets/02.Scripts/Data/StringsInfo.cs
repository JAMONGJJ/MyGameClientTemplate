using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace ClientTemplate
{
    
    namespace StringInfo
    {
        public enum StringType
        {
            None,
            Test,
            
        }

        [XmlRoot("String")]
        public class CommonString
        {
            [XmlElement("Type")] public StringType type;
            [XmlElement("Description")] public string description;
        }
        
        [XmlRoot("DataTable")]
        public class CommonStrings
        {
            [XmlArray("CommonStrings"), XmlArrayItem("String")]
            public List<CommonString> stringsList;
        }

        public interface ICommonStringsInfoContainer
        {
            string GetString(StringType type);
        }

        public class CommonStringsInfoContainer : ICommonStringsInfoContainer
        {
            private Dictionary<StringType, string> _commonStringInfoContainer;

            public CommonStringsInfoContainer(CommonStrings strings)
            {
                _commonStringInfoContainer = new Dictionary<StringType, string>();

                foreach (CommonString commonString in strings.stringsList)
                {
                    if (_commonStringInfoContainer.ContainsKey(commonString.type) == false)
                    {
                        _commonStringInfoContainer.Add(commonString.type, commonString.description);
                    }
                }
            }

            public string GetString(StringType type)
            {
                return _commonStringInfoContainer[type];
            }
        }
    }
}
