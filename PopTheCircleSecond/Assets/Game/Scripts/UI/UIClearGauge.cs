using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopTheCircle.Game
{
    public class UIClearGauge : MonoBehaviour
    {
        public Slider gaugeSlider;
        public Text percentText;

        private GameManager gm;


        private void Awake()
        {
            gm = GameObject.FindObjectOfType<GameManager>();
        }

        private void Update()
        {
            if (gm == null)
            {
                gm = GameObject.FindObjectOfType<GameManager>();
            }
            else
            {
                gaugeSlider.value = gm.ClearGauge / 100.0f;
                percentText.text = ((int)gm.ClearGauge).ToString();
            }
        }
    }
}