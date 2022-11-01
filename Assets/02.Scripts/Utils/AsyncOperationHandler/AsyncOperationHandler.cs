using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace ClientTemplate
{
    namespace UtilityFunctions
    {
        public delegate bool AsyncOperation();

        public interface IAsyncOperationHandler : IManager
        {
            void Process(System.Action OperationFinishCallback, params AsyncOperation[] operations);
        }

        public class AsyncOperationHandler : IAsyncOperationHandler
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

            public async void Process(System.Action OperationFinishCallback, params AsyncOperation[] operations)
            {
                UIManager.Instance.OpenWaitingWindow();
                try
                {
                    bool operationCompleted = false;
                    long elapsedTime = 0;
                    foreach (AsyncOperation operation in operations)
                    {
                        operationCompleted = operation.Invoke();
                        while (operationCompleted == false)
                        {
                            await Task.Delay(10);
                            elapsedTime += 10;
                            if (elapsedTime >= 5000)
                            {
                                throw new Exception("Async operation handling time out!");
                            }
                        }
                        operationCompleted = false;
                        elapsedTime = 0;
                    }
                }
                catch(Exception e)
                {
                    LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
                }
                OperationFinishCallback.Invoke();
                UIManager.Instance.CloseWaitingWindow();
            }
        }
    }
}
