using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class EventNoteRenderer : NoteRenderer
    {
        [InspectorReadOnly]
        public EventNote eventNote;



        public override void Initialize()
        {
            base.Initialize();

            eventNote = (EventNote)note;
            
            float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;
            notePos.y = (railLength.railNumber - 1) * -railTotalHeight;
            this.transform.localPosition = notePos;
        }
    }
}