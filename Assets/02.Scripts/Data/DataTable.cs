using System.Collections;
using System.Collections.Generic;
using KlayLand.StringInfo;
using UnityEngine;

namespace KlayLand
{
    public static class Data
    {
        public static DataTable Table { get; } = new DataTable();
    }

    public class DataTable
    {
        private ICommonStringsInfoContainer _commonStringsInfoContainer;

        public void SetStringsInfoContainer(ICommonStringsInfoContainer container)
        {
            _commonStringsInfoContainer = container;
        }
        
        public string GetStringInfo(StringType type)
        {
            return _commonStringsInfoContainer.GetString(type);
        }
    }
}
