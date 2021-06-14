using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    /// <summary>
    /// 노트 데이터의 기초 클래스이다.
    /// </summary>
    [System.Serializable]
    public class Note
    {
        /// <summary>
        /// 해당 노트의 타입 Enum이다.
        /// </summary>
        public NoteType noteType = NoteType.Normal;
        /// <summary>
        /// 노트가 출현할 박자(Bar) 번호이다.
        /// </summary>
        public int bar = 0;
        /// <summary>
        /// 노트가 출현할 박자의 세부 박자(beat) 번호이다.
        /// </summary>
        public float beat = 0.0f;
        /// <summary>
        /// 노트가 출현할 라인 번호이다.
        /// </summary>
        public int railNumber = 0;

        /// <summary>
        /// 노트가 판정될 시간값(초 단위)이다. 해당 값은 인게임에서 계산하여 대입한다.
        /// </summary>
        public float time = 0.0f;

        /// <summary>
        /// 노트가 표시될 공간적 위치이다. 해당 값은 인게임에서 계산하여 대입한다.
        /// </summary>
        public double position = 0.0d;
        
        /// <summary>
        /// 해당 노트를 놓쳤는지에 대한 여부이다. 해당 값은 인게임에서 계산하여 대입한다.
        /// </summary>
        public bool isMissed = false;

        /// <summary>
        /// 게임 내의 노트 오브젝트이다. 해당 값은 인게임에서 대입한다.
        /// </summary>
        public GameObject noteObject;
               
        
        
        public virtual bool ContainsInBarBeat(float _startBarBeat, float _endBarBeat)
        {
            float barBeat = BeatManager.ToBarBeat(bar, beat);
            if (barBeat < _startBarBeat || barBeat > _endBarBeat)
                return false;
            return true;
        }

        public virtual Note GetInstance()
        {
            return new Note()
            {
                bar = this.bar,
                beat = this.beat,
                noteType = this.noteType,
                railNumber = this.railNumber,
                
                position = this.position,
                time = this.time,
                isMissed = this.isMissed,
                noteObject = this.noteObject,
            };
        }
    }

    public enum NoteType
    {
        Normal = 0,
        Pop = 1,
        Long = 2,
        Space = 3,
        Mine = 4,
        Effect = 5,

        Camera = 6,
        BPMChange = 7,
        CTChange = 8,
        Tick = 9,
    }
}
