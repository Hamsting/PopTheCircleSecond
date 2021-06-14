using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopTheCircle.Game
{
    public class UIJudgeImageAndCombo : MonoBehaviour
    {
        private const float DissapearStartTime = 0.75f;
        private const float TotalAppearDuration = 2.00f;
        private const float TimingAppearDuration = 0.75f;
        private static readonly Color TimingEarlyColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        private static readonly Color TimingLateColor  = new Color(0.0f, 0.0f, 1.0f, 1.0f);

        public Sprite perfectSprite;
        public Sprite greatSprite;
        public Sprite missSprite;

        public Image judgeImage;
        public Animator judgeAnim;
        public Text comboText;
        public Text timingText;

        private float disappearTimer = 0.0f;
        private float timingDisappearTimer = 0.0f;



        private void Awake()
        {
            disappearTimer = 0.0f;
            timingDisappearTimer = 0.0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_railNumber"></param>
        /// <param name="_judge">0 : Miss, 1 : Perfect, 2 : Great</param>
        public void Appear(int _railNumber, int _judge, int _timingDir)
        {
            disappearTimer = TotalAppearDuration;

            if (_judge == 1)
                judgeImage.sprite = perfectSprite;
            else if (_judge == 2)
                judgeImage.sprite = greatSprite;
            else
                judgeImage.sprite = missSprite;

            judgeAnim.Play("JudgeImage", -1, 0.0f);
            comboText.text = GameManager.Instance.currentCombo.ToString();

            UpdateTimingText(_timingDir);
        }

        private void LateUpdate()
        {
            disappearTimer = Mathf.Clamp(disappearTimer - Time.deltaTime, 0.0f, TotalAppearDuration);

            Color color = Color.white;
            if (disappearTimer <= DissapearStartTime)
                color.a = disappearTimer / DissapearStartTime;

            judgeImage.color = color;
            comboText.color = (GameManager.Instance.currentCombo > 0) ? color : Color.clear;

            timingDisappearTimer = Mathf.Clamp(timingDisappearTimer - Time.deltaTime, 0.0f, TimingAppearDuration);
            if (timingDisappearTimer <= 0.0f && !string.IsNullOrEmpty(timingText.text))
            {
                timingText.text = "";
                timingText.color = Color.clear;
            }
        }

        private void UpdateTimingText(int _timingDir)
        {
            if (_timingDir != 0)
            {
                timingDisappearTimer = TimingAppearDuration;
                timingText.text = (_timingDir > 0) ? "EARLY" : "LATE";
                timingText.color = (_timingDir > 0) ? TimingEarlyColor : TimingLateColor;
            }
        }
    }
}