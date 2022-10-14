using System;
using System.Collections;
using System.Collections.Generic;
using KlayLand.SceneInfo;
using KlayLand.UIInfo;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

namespace KlayLand
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
