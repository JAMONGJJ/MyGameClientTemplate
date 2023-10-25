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

        public enum LanguageStringType
        {
            None,
            
            
        }

        public interface ICommonStringsInfoContainer
        {
            string GetString(StringType type);
        }

        public class CommonStringsInfoContainer : ICommonStringsInfoContainer
        {
            private Dictionary<StringType, string> CommonStringInfoContainer;

            public CommonStringsInfoContainer(List<CommonStringData> strings)
            {
                CommonStringInfoContainer = new Dictionary<StringType, string>();

                foreach (CommonStringData commonString in strings)
                {
                    CommonStringInfoContainer.TryAdd(commonString.stringType, commonString.description);
                }
            }

            public string GetString(StringType type) 
            {
                return CommonStringInfoContainer[type];
            }
        }

        /// <summary>
        /// 애셋 번들의 string data table에서 값을 가져와 사용하는 문자열들(변할 수 있음)
        /// </summary>
        public class LanguagePacksInfoContainer
        {
            private Dictionary<SystemLanguage, Dictionary<LanguageStringType, string>> LanguageStringInfoMap;

            public LanguagePacksInfoContainer(List<LanguageStringDataContainer> strings)
            {
                LanguageStringInfoMap = new Dictionary<SystemLanguage, Dictionary<LanguageStringType, string>>();

                foreach (LanguageStringDataContainer languageStringDataContainer in strings)
                {
                    if (LanguageStringInfoMap.ContainsKey(languageStringDataContainer.type) == false)
                    {
                        Dictionary<LanguageStringType, string> languageStrings = new Dictionary<LanguageStringType, string>();
                        foreach (LanguageStringData languageString in languageStringDataContainer.DataList)
                        {
                            languageStrings.Add(languageString.type, languageString.description);
                        }

                        LanguageStringInfoMap.Add(languageStringDataContainer.type, languageStrings);
                    }
                    else
                    {
                        foreach (LanguageStringData languageString in languageStringDataContainer.DataList)
                        {
                            if (LanguageStringInfoMap[languageStringDataContainer.type].ContainsKey(languageString.type) == false)
                            {
                                LanguageStringInfoMap[languageStringDataContainer.type]
                                    .Add(languageString.type, languageString.description);
                            }
                        }
                    }
                }
            }

            public string GetString(SystemLanguage languageType, LanguageStringType type) 
            {
                if (LanguageStringInfoMap.ContainsKey(languageType) == false)
                {
                    return string.Empty;
                }

                if (LanguageStringInfoMap[languageType].ContainsKey(type) == false)
                {
                    return string.Empty;
                }
                
                return LanguageStringInfoMap[languageType][type];
            }
        }
    }
}
