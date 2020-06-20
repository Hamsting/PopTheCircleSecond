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


        private void Start()
        {
        }

        private void Update()
        {
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
        }
    }
}
