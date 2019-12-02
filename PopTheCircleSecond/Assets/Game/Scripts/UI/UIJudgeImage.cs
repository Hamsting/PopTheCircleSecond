using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopTheCircle.Game
{
    public class UIJudgeImage : MonoBehaviour
    {
        public Sprite perfectSprite;
        public Sprite greatSprite;
        public Sprite missSprite;
     


        private void Awake()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_railNumber"></param>
        /// <param name="_judge">0 : Miss, 1 : Perfect, 2 : Great</param>
        public void AppearJudgeImage(int _railNumber, int _judge)
        {
            
            
        }

        private void Update()
        {
        }
    }
}