using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopTheCircle.NoteEditor
{
    [System.Serializable]
    public class DragNote : Note
    {
        /// <summary>
        /// 노트 처리를 위한 드래그 방향
        /// 0 : 왼쪽, 1 : 오른쪽
        /// </summary>
        public int direction = 0;
    }

}