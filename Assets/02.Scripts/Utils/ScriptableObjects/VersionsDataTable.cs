using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    [CreateAssetMenu(fileName = "VersionsDataTable", menuName = "ScriptableObject/VersionDataTable")]
    public class VersionsDataTable : ScriptableObject
    {
        public string version;
        public string playStoreLink;
        public string appStoreLink;
    }
}
