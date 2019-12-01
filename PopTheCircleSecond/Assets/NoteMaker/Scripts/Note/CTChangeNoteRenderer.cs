using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class CTChangeNoteRenderer : NoteRenderer
    {
        public TextMesh ctText;
        [InspectorReadOnly]
        public CTInfo ctInfo;



        public override void Initialize()
        {
            if (ctInfo == null)
                return;

            float barBeat = BeatManager.ToBarBeat(ctInfo.bar, 0.0f);
            railLength = BeatManager.Instance.GetNoteRailLengthWithBarBeat(barBeat);
            float localBarBeatDiff = barBeat - railLength.startBarBeat;
            float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;

            notePos.x = 0.0f;
            notePos.y = (railLength.railNumber - 1) * -railTotalHeight;
            notePos.z = -0.5f;

            this.transform.localPosition = notePos;

            ctText.text = ctInfo.numerator.ToString() + "/4";
        }
    }
}