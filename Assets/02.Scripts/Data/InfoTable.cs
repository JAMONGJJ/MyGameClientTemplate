using System.Collections;
using System.Collections.Generic;
using ClientTemplate.StringInfo;
using UnityEngine;

namespace ClientTemplate
{
    public static class Info
    {
        public static InfoTable Table { get; } = new InfoTable();
    }

    public class InfoTable
    {
        private LanguagePacksInfoContainer LanguagePacksInfoContainer;
        private ICommonStringsInfoContainer CommonStringsInfoContainer;
        private UserDataManager UserData { get; } = new UserDataManager();

        public void SetInfoTable(DataTables dataTables)
        {
            CommonStringsInfoContainer = new CommonStringsInfoContainer(dataTables.CommonStrings);
            LanguagePacksInfoContainer = new LanguagePacksInfoContainer(dataTables.LanguagePacks);
        }
        
        public string GetStringInfo(StringType type)
        {
            return CommonStringsInfoContainer.GetString(type);
        }
        
        public string GetStringInfo(LanguageStringType type)
        {
            return LanguagePacksInfoContainer.GetString(Core.System.Settings.LanguageType, type);
        }
    }
}
