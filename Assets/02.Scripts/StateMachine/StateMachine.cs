using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    using StateInfo;
    public static class StateMachine
    {
        public static IState CurrentState { get; private set; }

        public static void NextState(IState nextState)
        {
            ProcessState(nextState);
        }

        private static void ProcessState(IState nextState)
        {
            if (CurrentState != null)
            {
                if (CurrentState.CanTransitState(nextState.id) == true)
                {
                    LogManager.Log(LogManager.LogType.STATE_ON_END, CurrentState.name);
                    CurrentState.OnEnd();
                }
                else
                {
                    LogManager.LogError(LogManager.LogType.DEFAULT, "Cannot transit state!");
                    return;
                }
            }

            CurrentState = nextState;
            LogManager.Log(LogManager.LogType.STATE_ON_BEGIN, CurrentState.name);
            CurrentState.OnBegin();
        }

        public static void RestartCurrentState()
        {
            LogManager.Log(LogManager.LogType.STATE_RESTART, CurrentState.name);
            CurrentState.OnBegin();
        }
    }
}
