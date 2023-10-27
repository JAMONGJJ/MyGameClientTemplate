using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetManager : MonoBehaviour
{
    [MenuItem("DDinDDon/Asset Management/Window")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow(typeof(AssetManageWindow), false, "Asset Manager");
    }


}
