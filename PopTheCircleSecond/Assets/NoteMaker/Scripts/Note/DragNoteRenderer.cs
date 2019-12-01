using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class DragNoteRenderer : NoteRenderer
    {
        public Transform arrow;

        private DragNote dragNote;


        
        public override void Initialize()
        {
            base.Initialize();

            dragNote = (DragNote)note;

            // DragNote의 왼쪽 라인 배치에서, 현재는 실제 드래그 방향과 반전된 화살표 방향을 보여주고 있음.
            // 화살표 방향을 실제 플레이 시의 드래그 방향으로 보길 원할 경우,
            // 아래의 '+ ((dragNote.railNumber == 0) ? 180.0f : 0.0f)' 부분 제거. (실제 게임 플레이엔 지장 없음)
            float arrowAngle = ((dragNote.direction == 0) ? 0.0f : 180.0f) + ((dragNote.railNumber == 0) ? 180.0f : 0.0f);
            arrow.eulerAngles = new Vector3(0.0f, 0.0f, arrowAngle);
        }
    }
}