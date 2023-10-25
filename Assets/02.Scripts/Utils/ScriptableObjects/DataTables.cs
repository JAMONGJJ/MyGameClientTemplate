using System;
using System.Collections;
using System.Collections.Generic;
using ClientTemplate.StringInfo;
using UnityEngine;

namespace ClientTemplate
{
    [Serializable]
    public class CommonStringData
    {
        public StringType stringType;
        public string description;
    }
    
    [Serializable]
    public class LanguageStringDataContainer
    {
        public SystemLanguage type;
        public List<LanguageStringData> DataList;
    }

    [Serializable]
    public class LanguageStringData
    {
        public LanguageStringType type;
        public string description;
    }

    [CreateAssetMenu(fileName = "DataTables", menuName = "ScriptableObject/DataTables")]
    public class DataTables : ScriptableObject
    {
        public List<CommonStringData> CommonStrings;
        
        public List<LanguageStringDataContainer> LanguagePacks;
    }
}