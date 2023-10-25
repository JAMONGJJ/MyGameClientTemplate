using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Herma
{
    [CreateAssetMenu(fileName = "BoppinBuildSettings", menuName = "ScriptableObject/BoppinBuildSettings")]
    public class BoppinBuildSettings : ScriptableObject
    {
        public string BuildLocation;
    }
}
