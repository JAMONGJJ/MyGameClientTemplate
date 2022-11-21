using System.Collections;
using System.Collections.Generic;
using ClientTemplate.StringInfo;
using ClientTemplate.VersionInfo;
using UnityEngine;

namespace ClientTemplate
{
    public static class Data
    {
        public static DataTable Table { get; } = new DataTable();
    }

    public class DataTable
    {
        private Version Version;
        private ICommonStringsInfoContainer CommonStringsInfoContainer;

        public void SetVersion(Version version)
        {
            Version = version;
        }

        public void SetStringsInfoContainer(ICommonStringsInfoContainer container)
        {
            CommonStringsInfoContainer = container;
        }

        #region Current App Version

        public Version GetVersionInfo()
        {
            return Version;
        }

        #endregion
        
        public string GetStringInfo(StringType type)
        {
            return CommonStringsInfoContainer.GetString(type);
        }
    }
}
