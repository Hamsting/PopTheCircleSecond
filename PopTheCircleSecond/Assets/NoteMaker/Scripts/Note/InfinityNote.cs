using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopTheCircle.NoteEditor
{
    [System.Serializable]
    public class InfinityNote : Note
    {
        /// <summary>
        /// 노트가 사라지는 박자 번호이다.
        /// </summary>
        public int endBar = 0;

        /// <summary>
        /// 노트가 사라지는 박자의 세부 번호이다.
        /// </summary>
        public float endBeat = 0.0f;

        /// <summary>
        /// 해당 노트를 얼마나 난타해야하는 지의 횟수이다.
        /// </summary>
        public int maxHitCount = 1;

        /// <summary>
        /// 노트가 사라지는 시간 값이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public float endTime = 0.0f;

        /// <summary>
        /// 현재 노트를 난타할 수 있는 지의 여부이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public bool canPress = false;

        /// <summary>
        /// 현재 노트를 얼마나 난타했는 지의 횟수이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public int currentHitCount = 0;
    }

}