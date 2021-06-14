using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PopTheCircle.Game
{
    public class TESTGAMEUI : MonoBehaviour
    {
        public Text debugText;
        public Text speedText;



        private void Start()
        {
        }

        private void Update()
        {
            UpdateDeveloperHotKeyState();

            int perCount = GameManager.Instance.judgePerfectCount;
            int greCount = GameManager.Instance.judgeNiceCount;
            int misCount = GameManager.Instance.judgeMissCount;
            int totalTick = GameManager.Instance.totalCombo;

            debugText.text =
                "Total ticks : " + totalTick + "\n" +
                "Perfect : " + perCount + "\n" +
                "Great : " + greCount + "\n" +
                "Miss : " + misCount;
            speedText.text = "Speed : " + BeatManager.Instance.CurrentBPM + " x " + BeatManager.Instance.GameSpeed.ToString("F02") +
                " = " + (int)(BeatManager.Instance.CurrentBPM * BeatManager.Instance.GameSpeed);
        }

        private void UpdateDeveloperHotKeyState()
        {
            if (Input.GetKeyDown(KeyCode.RightBracket))
                BeatManager.Instance.GameSpeedNotRelatedBPM += 100.0f;
            if (Input.GetKeyDown(KeyCode.LeftBracket))
                BeatManager.Instance.GameSpeedNotRelatedBPM -= 100.0f;
            if (Input.GetKeyDown(KeyCode.Plus))
                BeatManager.Instance.GameSpeedNotRelatedBPM += 10.0f;
            if (Input.GetKeyDown(KeyCode.Minus))
                BeatManager.Instance.GameSpeedNotRelatedBPM -= 10.0f;

            if (Input.GetKeyDown(KeyCode.Alpha0))
                BeatManager.Instance.GameSpeedNotRelatedBPM = 460.1f;
        }

        public void SetSpeedTo1400()
        {
            BeatManager.Instance.GameSpeedNotRelatedBPM = 460.1f;
        }

        public void RestartSimulate()
        {
            SceneManager.LoadScene("Game");
        }

        public void ReturnToEditor()
        {
            SceneManager.LoadScene("Blank");
        }
    }
}
