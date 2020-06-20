using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class NoteRenderer : MonoBehaviour
    {
        [InspectorReadOnly]
        public Note note;

        protected Vector3 notePos = Vector3.zero;
        protected NoteRailLength railLength = null;



        public virtual void Initialize()
        {
            if (note == null)
                return;

            float barBeat = BeatManager.ToBarBeat(note.bar, note.beat);
            railLength = BeatManager.Instance.GetNoteRailLengthWithBarBeat(barBeat);
            float localBarBeatDiff = barBeat - railLength.startBarBeat;
            float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;

            notePos.x = localBarBeatDiff * NoteRail.RailOneBarWidth;
            // notePos.y = (railLength.railNumber - 1) * -railTotalHeight;
            //if (note.railNumber == 0)
            //    notePos.y += NoteRail.RailHeight * 0.333333f;
            notePos.y = (railLength.railNumber - 1) * -railTotalHeight + NoteRailManager.Instance.RailNumberToLineNoteYPos(note.railNumber);
            notePos.z = -0.5f;

            this.transform.localPosition = notePos;
        }
    }
}