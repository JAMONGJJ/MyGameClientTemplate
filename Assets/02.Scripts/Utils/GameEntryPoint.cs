using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClientTemplate
{
    public class GameEntryPoint : MonoBehaviour
    {
        /// <summary>
        /// Game Entry Point!!!
        /// </summary>
        private void Start()
        {
            StateMachine.NextState(new EntryState());
        }
    }
}
