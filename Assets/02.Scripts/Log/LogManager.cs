using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;

namespace KlayLand
{
    using System.Diagnostics;
    public static class LogManager
    {
        public enum LogType
        {
            DEFAULT = 0,
            EXCEPTION,
            UI_OPEN,
            UI_CLOSE,
            BUTTON_PRESSED,
            SCENE_LOADING_START,
            SCENE_LOADING_FINISH,
            SCENE_LOADING_FAIL,
            STATE_ON_BEGIN,
            STATE_ON_END,
            STATE_RESTART,
            CONTROLLER_INIT,
            CONTROLLER_RELEASE,
            CONTROLLER_RESET,
            TEST,

        }

        private static class LogStrings
        {
            public static Dictionary<LogType, string> LogTable = new Dictionary<LogType, string>()
            {
                { LogType.DEFAULT, "{0}" },
                { LogType.EXCEPTION, "EXCEPTION!\n{0}" },
                { LogType.UI_OPEN, "{0} UI opened!" },
                { LogType.UI_CLOSE, "{0} UI closed!" },
                { LogType.BUTTON_PRESSED, "{0} button pressed!" },
                { LogType.SCENE_LOADING_START, "{0} SCENE LOAD is started!" },
                { LogType.SCENE_LOADING_FINISH, "{0} SCENE LOAD is finished!" },
                { LogType.SCENE_LOADING_FAIL, "{0} SCENE LOAD is failed!" },
                { LogType.STATE_ON_BEGIN, "{0} is ON BEGIN!" },
                { LogType.STATE_ON_END, "{0} is ON END!" },
                { LogType.STATE_RESTART, "{0} is RESTARTED!" },
                { LogType.CONTROLLER_INIT, "{0} INIT!" },
                { LogType.CONTROLLER_RELEASE, "{0} RELEASE!"},
                { LogType.CONTROLLER_RESET, "{0} RESET!"},
                { LogType.TEST, "*************************** TEST LOGS! *************************** {0}"},

            };

            public static void CheckLogTableValidity()
            {
                try
                {
                    if (LogTable.ContainsKey(LogType.TEST))
                    {
                        UnityEngine.Debug.Log("LogManager's LogTable validity check completed!");
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e.ToString());
                }
            }
        }

        private class FileWriter
        {
            private string logFilePath = string.Empty;
            private string logFileName = string.Empty;
            private string FilePath = string.Empty;

            public FileWriter()
            {
                LogStrings.CheckLogTableValidity();
#if UNITY_EDITOR
                logFilePath = $"{Application.persistentDataPath}\\UnityEditorLogFiles";
                System.IO.Directory.CreateDirectory(logFilePath);
                logFileName = $"{DateTime.Now.Year:D4}{DateTime.Now.Month:D2}{DateTime.Now.Day:D2}_Log";
                FilePath = $"{logFilePath}\\{logFileName}.log";
                WriteInitialText();
#elif TEST_BUILD
                logFilePath = $"{Application.persistentDataPath}/UnityEditorLogFiles";
                System.IO.Directory.CreateDirectory(logFilePath);
                logFileName = $"{DateTime.Now.Year:D4}{DateTime.Now.Month:D2}{DateTime.Now.Day:D2}_Log";
                FilePath = $"{logFilePath}/{logFileName}.txt";
                WriteInitialText();
#endif
            }

            internal void WriteText(string message)
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    string outputMessage = $"{DateTime.Now.ToLongTimeString()}.{DateTime.Now.Millisecond:D6}   {message}\n";
                    for (int i = 0; i < outputMessage.Length; i++)
                    {
                        fs.WriteByte((byte)outputMessage[i]);
                    }
                }
            }

            private void WriteInitialText()
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    string outputMessage = "\n\n-------------------------------<SYSTEM INITIATED>-------------------------------\n";
                    for (int i = 0; i < outputMessage.Length; i++)
                    {
                        fs.WriteByte((byte)outputMessage[i]);
                    }
                }
            }
        }

        private class LogInfo
        {
            private string LogFormat = "<{0}> {1}";
            public string LogMessage { get; private set; }
            private LogType LogType { get; set; }

            /// <summary>
            /// _message의 포맷은 _type에 따라 달라짐.
            /// LogType과 대응되는 로그 메시지가 LogStrings.LogTable에 저장되어있음. 
            /// </summary>
            /// <param name="messages"></param>
            /// <param name="type"></param>
            public void SetLog(object[] messages, LogType type)
            {
                LogType = type;
                SetLogMessage(messages);
            }

            private void SetLogMessage(object[] messages)
            {
                string result = "";
                try
                {
                    if (TryGetLogFormat(out result))
                    {
                        LogMessage = string.Format(result, ConvertIntoList(messages));
                        LogMessage = string.Format(LogFormat, StateMachine.CurrentState, LogMessage);
                    }
                    else
                    {
                        LogError(LogType.DEFAULT, "Log format not found!");
                    }
                }
                catch (Exception e)
                {
                    Log(LogType.EXCEPTION, e.ToString());
                }
            }

            private bool TryGetLogFormat(out string format)
            {
                if (LogStrings.LogTable.TryGetValue(LogType, out format))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private T[] ConvertIntoList<T>(T[] list)
            {
                T[] objectlist = new T[list.Length];
                list.CopyTo(objectlist, 0);
                return objectlist;
            }
        }

        private static LogInfo logInfo = new LogInfo();
        private static FileWriter logFileWriter = new FileWriter();

        /// <summary>
        /// 추후 로그를 찍으면 안 되는 상황이 생길 경우를 핸들링하기 위해 만들어놓은 메소드
        /// </summary>
        /// <returns></returns>
        private static bool CanPrintLog()
        {
            return true;
        }

        #region Print Log Message
        [Conditional("UNITY_EDITOR")]
        [Conditional("TEST_BUILD")]
        public static void Log(LogType type, params object[] messages)
        {
            logInfo.SetLog(messages, type);
            Print();
            PrintEditorLog();
        }
        
        [Conditional("UNITY_EDITOR")]
        [Conditional("TEST_BUILD")]
        public static void LogWarning(LogType type, params object[] messages)
        {
            logInfo.SetLog(messages, type);
            Print();
            PrintEditorLogWarning();
        }
        
        [Conditional("UNITY_EDITOR")]
        [Conditional("TEST_BUILD")]
        public static void LogError(LogType type, params object[] messages)
        {
            logInfo.SetLog(messages, type);
            Print();
            PrintEditorLogError();
        }

        private static void Print()
        {
            if (!CanPrintLog())
            {
                return;
            }
            logFileWriter.WriteText(logInfo.LogMessage);    // export as txt file!
        }

        [Conditional("EDITOR_LOG_ENABLED")]
        private static void PrintEditorLog()
        {
            UnityEngine.Debug.Log(logInfo.LogMessage);         // for editor console log!
        }

        [Conditional("EDITOR_LOG_ENABLED")]
        private static void PrintEditorLogWarning()
        {
            UnityEngine.Debug.LogWarning(logInfo.LogMessage);         // for editor console log!
        }

        [Conditional("EDITOR_LOG_ENABLED")]
        private static void PrintEditorLogError()
        {
            UnityEngine.Debug.LogError(logInfo.LogMessage);         // for editor console log!
        }
        #endregion
    }
}