using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ClientTemplate
{
    public static class OpenLogFilesDirectory
    {
        [MenuItem("JimmyTools/LogManager/OpenLogFilesDirectory")]
        public static void OpenBuildDirectory()
        {
            OpenFileBrowser(Path.GetFullPath($"{Application.persistentDataPath}\\UnityEditorLogFiles"));
        }

        private static void OpenFileBrowser(string path)
        {
#if UNITY_EDITOR_WIN
            if (Directory.Exists(path))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = path,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, $"Failed to open build file path : {path}");
            }
#elif UNITY_EDITOR_OSX
            bool openInsidesOfFolder = Directory.Exists(path);

            string arg = (openInsidesOfFolder ? "" : "-R ") + path;
            try
            {
                System.Diagnostics.Process.Start("open", arg);
            }
            catch(Exception e)
            {
                LogManager.LogError(LogManager.LogType.EXCEPTION, $"Failed to open build file path : {e}");
            }
#endif
        }
    }
}
