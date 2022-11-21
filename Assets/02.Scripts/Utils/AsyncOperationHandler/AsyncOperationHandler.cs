using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace ClientTemplate
{
    namespace UtilityFunctions
    {
        public delegate void AsyncOperation();

        public interface IAsyncOperationHandler : IManager
        {
            void SetIsProcessing(bool processing);
            void Process(System.Action OperationFinishCallback, params AsyncOperation[] operations);
        }

        public class AsyncOperationHandler : IAsyncOperationHandler
        {
            private bool isProcessing;
            
            public void Init()
            {
            }

            public void Release()
            {
            }

            public void ReSet()
            {
            }

            public void SetIsProcessing(bool processing)
            {
                isProcessing = processing;
            }

            public async void Process(System.Action OperationFinishCallback, params AsyncOperation[] operations)
            {
                try
                {
                    long elapsedTime = 0;
                    foreach (AsyncOperation operation in operations)
                    {
                        operation.Invoke();
                        while (isProcessing == true)
                        {
                            await Task.Delay(10);
                            elapsedTime += 10;
                            if (elapsedTime >= 10000)
                            {
                                throw new Exception("Async operation handling time out!");
                            }
                        }
                        elapsedTime = 0;
                    }
                    OperationFinishCallback.Invoke();
                }
                catch(Exception e)
                {
                    LogManager.LogError(LogManager.LogType.EXCEPTION, e.ToString());
                }
            }
        }
    }
}
