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
    }

}