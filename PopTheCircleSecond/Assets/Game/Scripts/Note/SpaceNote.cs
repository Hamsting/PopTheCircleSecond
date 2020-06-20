using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    [System.Serializable]
    public class SpaceNote : LongNote
    {
        /// <summary>
        /// 해당 노트가 롱노트 형태인지의 여부이다.
        /// </summary>
        public bool IsLongType
        {
            get
            {
                if (endBar == 0 && endBeat <= 0.0f)
                    return false;
                else
                    return true;
            }
        }
    }
}