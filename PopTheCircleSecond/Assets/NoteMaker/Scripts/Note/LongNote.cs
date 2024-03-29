﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopTheCircle.NoteEditor
{
    [System.Serializable]
    public class LongNote : Note
    {
        /// <summary>
        /// 노트의 너비 고정 수치
        /// </summary>
        public const float bodyHeight = 1.0f;

        /// <summary>
        /// 노트가 사라지는 박자 번호이다.
        /// </summary>
        public int endBar = 0;

        /// <summary>
        /// 노트가 사라지는 박자의 세부 번호이다.
        /// </summary>
        public float endBeat = 0.0f;

        /// <summary>
        /// 롱노트가 이어진 라인 번호이다.
        /// </summary>
        public int connectedRail = 0;
        /*
        /// <summary>
        /// 노트가 사라지는 시간 값이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public float endTime = 0.0f;
        
        /// <summary>
        /// 노트가 사라지는 공간적 위치이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public double endPosition = 0.0d;

        /// <summary>
        /// 누르고 있는지의 여부이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public bool pressed = false;

        /// <summary>
        /// 롱노트가 한 번이라도 InputPress 되었는 지의 여부이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public bool firstPressed = false;
        
        /// <summary>
        /// 롱노트의 최초 틱이 발생되었는 지에 대한 여부이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public bool firstTicked = false;

        /// <summary>
        /// 마지막으로 눌렀을 때의 박자비트 위치이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public float lastPressedBarBeat = 0.0f;

        /// <summary>
        /// 롱노트의 틱이 시작되는 박자비트 위치이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public float tickStartBarBeat = 0.0f;

        /// <summary>
        /// 롱노트의 틱이 끝나는 박자비트 위치이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public float tickEndBarBeat = 0.0f;
        */



        public override Note GetInstance()
        {
            return new LongNote()
            {
                bar = this.bar,
                beat = this.beat,
                noteType = this.noteType,
                railNumber = this.railNumber,

                endBar = this.endBar,
                endBeat = this.endBeat,
                connectedRail = this.connectedRail,

                position = this.position,
                time = this.time,
                isMissed = this.isMissed,
                noteObject = this.noteObject,
            };
        }
    }

}