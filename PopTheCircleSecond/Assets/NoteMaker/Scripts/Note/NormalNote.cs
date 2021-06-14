using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    [System.Serializable]
    /// <summary>
    /// 일반 노트의 데이터 클래스이다.
    /// </summary>
    public class NormalNote : Note
    {
        public override Note GetInstance()
        {
            return new NormalNote()
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
}