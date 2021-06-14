using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopTheCircle.NoteEditor
{
    [System.Serializable]
    public class EventNote : Note
    {
        /// <summary>
        /// 실행할 이벤트의 명칭
        /// </summary>
        public string eventName = "";


        public override Note GetInstance()
        {
            return new EventNote()
            {
                bar = this.bar,
                beat = this.beat,
                noteType = this.noteType,
                railNumber = this.railNumber,

                eventName = this.eventName,

                position = this.position,
                time = this.time,
                isMissed = this.isMissed,
                noteObject = this.noteObject,
            };
        }
    }

}