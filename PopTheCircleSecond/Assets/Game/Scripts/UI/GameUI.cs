using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopTheCircle.Game
{
    public class GameUI : Singleton<GameUI>
    {
        private const float VisualScoreIncreaseDuration = 0.325f;

        public UIJudgeImageAndCombo uIJudgeImageAndCombo;
        public Text scoreText;

        private int lastGameScore = 0;
        private int visualScore = 0;
        private int visualScoreBefore = 0;
        private float visualScoreTimer = 0.0f;



        private void Update()
        {
            UpdateVisualScore();
        }

        private void UpdateVisualScore()
        {
            int score = GameManager.instance.score;

            if (score != lastGameScore)
            {
                visualScoreBefore = visualScore;
                visualScoreTimer = 0.0f;
            }
            lastGameScore = score;

            visualScoreTimer = Mathf.Clamp(visualScoreTimer + Time.deltaTime, 0.0f, 3600.0f);

            visualScore = (int)Mathf.Lerp(visualScoreBefore, score, visualScoreTimer / VisualScoreIncreaseDuration);
            scoreText.text = visualScore.ToString("D07");
        }
    }
}