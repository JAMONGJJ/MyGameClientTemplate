using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    namespace UtilityFunctions
    {
        public delegate void ExceptionHandlingAction();

        public interface IExceptionHandler : IManager
        {
            void Process(ExceptionHandlingAction action);
        }

        public class ExceptionHandler : IExceptionHandler
        {
            public void Init()
            {
            }

            public void Release()
            {
            }

            public void ReSet()
            {
            }

            public void Process(ExceptionHandlingAction action)
            {
                try
                {
                    action.Invoke();
                }
                catch(Exception e)
                {
                    LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
                }
            }
        }
    }
}
