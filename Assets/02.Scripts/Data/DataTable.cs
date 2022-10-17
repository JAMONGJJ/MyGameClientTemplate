using System.Collections;
using System.Collections.Generic;
using ClientTemplate.StringInfo;
using UnityEngine;

namespace ClientTemplate
{
    public static class Data
    {
        public static DataTable Table { get; } = new DataTable();
    }

    public class DataTable
    {
        private ICommonStringsInfoContainer _commonStringsInfoContainer;    // 클라이언트 스크립트에서 로그에 찍는 스트링은 포함 안 함.

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
