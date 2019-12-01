using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class CameraNoteRenderer : NoteRenderer
    {
        private CameraNote cameraNote;



        public override void Initialize()
        {
            base.Initialize();

            cameraNote = (CameraNote)note;

            float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;
            notePos.y = (railLength.railNumber - 1) * -railTotalHeight - NoteRail.RailHeight * 0.333333f;
            this.transform.localPosition = notePos;
        }
    }
}