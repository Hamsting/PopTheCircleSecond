using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopTheCircle.Game
{
    public class TESTGAMEUI : MonoBehaviour
    {
        public Text scoreText;
        public Text debugText;
        public Text speedText;

        private USERSETTINGCONTROLLER usc;



        private void Start()
        {
            usc = GameObject.FindObjectOfType<USERSETTINGCONTROLLER>();
        }

        private void Update()
        {
            UpdateDeveloperHotKeyState();

            int perCount = GameManager.Instance.judgePerfectCount;
            int greCount = GameManager.Instance.judgeNiceCount;
            int misCount = GameManager.Instance.judgeMissCount;
            int totalTick = GameManager.Instance.totalCombo;
            float score = 1000000.0f * ((float)(perCount * 2 + greCount) / (float)(totalTick * 2));

            scoreText.text = ((int)score).ToString("D07");
            debugText.text =
                "Total ticks : " + totalTick + "\n" +
                "Perfect : " + perCount + "\n" +
                "Great : " + greCount + "\n" +
                "Miss : " + misCount;
            speedText.text = "Speed : " + BeatManager.Instance.CurrentBPM + " x " + usc.gameSpeed.ToString("F02") + " = " + (int)(BeatManager.Instance.CurrentBPM * usc.gameSpeed);
        }
        
        private void UpdateDeveloperHotKeyState()
        {
            if (Input.GetKeyDown(KeyCode.Keypad9))
                usc.gameSpeed += 1.0f;
            if (Input.GetKeyDown(KeyCode.Keypad3))
                usc.gameSpeed -= 1.0f;
            if (Input.GetKeyDown(KeyCode.Keypad8))
                usc.gameSpeed += 0.1f;
            if (Input.GetKeyDown(KeyCode.Keypad2))
                usc.gameSpeed -= 0.1f;

            if (Input.GetKeyDown(KeyCode.Keypad6))
                usc.gameSpeed = 1400.1f / BeatManager.Instance.StandardBPM;
        }
    }
}
