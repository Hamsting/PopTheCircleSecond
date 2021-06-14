using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    /// <summary>
    /// JP(Jump Position, 포지션 강제 스크롤링) 정보이다.
    /// </summary>
    [System.Serializable]
    public class JPInfo
    {
        public int bar = 0;
        public float beat = 0.0f;

        public int jumpBar = 0;
        public float jumpBeat = 0.0f;
        public float jumpBarBeat
        {
            get
            {
                return BeatManager.ToBarBeat(jumpBar, jumpBeat);
            }
            set
            {
                jumpBar = (int)value;
                jumpBeat = (value - (float)jumpBar) * GlobalDefines.BeatPerBar;
            }
        }

        public double jumpPositionAmount = 0.0d;
        public bool isJumped = false;
    }
}
