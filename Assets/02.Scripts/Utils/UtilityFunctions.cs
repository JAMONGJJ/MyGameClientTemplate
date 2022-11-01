using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    namespace UtilityFunctions
    {
        public static class Utility
        {
            public static UtilityFunctions Functions { get; } = new UtilityFunctions();
        }

        public class UtilityFunctions
        {
            public IAsyncOperationHandler Async { get; private set; }
            public IExceptionHandler Exception { get; private set; }

            public void SetAsyncOperationHandler(IAsyncOperationHandler handler)
            {
                Async = handler;
            }

            public void SetExceptionHandler(IExceptionHandler handler)
            {
                Exception = handler;
            }
        }
    }
}
