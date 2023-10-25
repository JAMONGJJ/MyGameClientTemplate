using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClientTemplate
{
    public class UIDataLoadWindow : MonoBehaviour
    {
        public enum SizeMeasure
        {
            Kb = 0,
            Mb,
            Gb,
            Tb,
            
        }

        [SerializeField] private TMP_Text descText;
        [SerializeField] private Button downloadButton;
        [SerializeField] private Slider downloadSlider;
        
        public void SetDescText(long downloadAmount)
        {
            string calculatedAmount = CalculateDownloadAmount(downloadAmount);
            descText.text = $"You need to download {calculatedAmount}!";
        }

        public void SetSliderValue(float value)
        {
            downloadSlider.value = value;
        }

        public void SetActiveDownloadButton(bool active)
        {
            downloadButton.gameObject.SetActive(active);
        }

        public void SetActiveDownloadSlider(bool active)
        {
            downloadSlider.gameObject.SetActive(active);
        }

        public void SetDownloadButton(System.Action action)
        {
            downloadButton.onClick.AddListener(() =>
                {
                    SetActiveDownloadButton(false);
                    SetActiveDownloadSlider(true);
                    action.Invoke();
                });
        }

        private string CalculateDownloadAmount(long amount)
        {
            SizeMeasure count = 0;
            float result = amount / 1024.0f;
            while (result > 1024)
            {
                result /= 1024;
                count++;
            } 
            return $"{result.ToString("0.0")}{count}";
        }
    }
}
