using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopTheCircle.Game
{
    public class UIJudgeImage : MonoBehaviour
    {
        public Sprite crazySprite;
        public Sprite amazingSprite;
        public Sprite perfectSprite;
        public Sprite niceSprite;
        public Sprite missSprite;

        public Animator leftAnimator;
        public Animator rightAnimator;

        public Image leftImage;
        public Image rightImage;

        public Transform leftAppearPosition;
        public Transform rightAppearPosition;

        private RectTransform leftRectTransform;
        private RectTransform rightRectTransform;
        private Vector2 screenRatio = Vector2.one;



        private void Awake()
        {
            leftRectTransform = leftAnimator.GetComponent<RectTransform>();
            rightRectTransform = rightAnimator.GetComponent<RectTransform>();
            leftAnimator.Play("Appear", -1, 1.0f);
            rightAnimator.Play("Appear", -1, 1.0f);

            screenRatio = new Vector2(Screen.width / 1920.0f, Screen.height / 1080.0f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_railNumber"></param>
        /// <param name="_judge">0 : Miss, 1 : Perfect, 2 : Great</param>
        public void AppearJudgeImage(int _railNumber, int _judge)
        {
            Sprite spr = missSprite;
            if (_judge == 1)
            {
                if (GameManager.Instance.currentCombo >= 100)
                    spr = crazySprite;
                else if (GameManager.Instance.currentCombo >= 50)
                    spr = amazingSprite;
                else
                    spr = perfectSprite;
            }
            else if (_judge == 2)
                spr = niceSprite;

            if (_railNumber == 0)
            {
                leftImage.sprite = spr;
                leftAnimator.Play("Appear", -1, 0.0f);
            }
            else
            {
                rightImage.sprite = spr;
                rightAnimator.Play("Appear", -1, 0.0f);
            }
            
        }

        private void Update()
        {
            leftRectTransform.anchoredPosition  = Camera.main.WorldToScreenPoint(leftAppearPosition.position) / screenRatio.y;
            rightRectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(rightAppearPosition.position) / screenRatio.y;
        }
    }
}