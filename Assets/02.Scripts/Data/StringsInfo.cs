using System.Collections;
using System.Collections.Generic;
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

        public interface ICommonStringsInfoContainer
        {
            string GetString(StringType type);
        }

        public class CommonStringsInfoContainer : ICommonStringsInfoContainer
        {
            private Dictionary<StringType, string> _commonStringInfoContainer;

            public CommonStringsInfoContainer(List<CommonStringData> strings)
            {
                _commonStringInfoContainer = new Dictionary<StringType, string>();

                foreach (CommonStringData commonString in strings)
                {
                    if (_commonStringInfoContainer.ContainsKey(commonString.stringType) == false)
                    {
                        _commonStringInfoContainer.Add(commonString.stringType, commonString.description);
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
