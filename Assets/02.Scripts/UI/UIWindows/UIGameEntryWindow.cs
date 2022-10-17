using System;
using System.Collections;
using System.Collections.Generic;
using ClientTemplate.SceneInfo;
using ClientTemplate.UIInfo;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

namespace ClientTemplate
{
    public class UIGameEntryWindow : MonoBehaviour
    {
        public TMP_Text EnteringText;
        
        private void Start()
        {
            StartCoroutine(Ticking());
        }

        IEnumerator Ticking()
        {
            while (true)
            {
                EnteringText.text = "Entering game....";
                yield return new WaitForSeconds(0.2f);
                EnteringText.text = "Entering game.";
                yield return new WaitForSeconds(0.2f);
                EnteringText.text = "Entering game..";
                yield return new WaitForSeconds(0.2f);
                EnteringText.text = "Entering game...";
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
