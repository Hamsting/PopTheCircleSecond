using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class JPNoteRenderer : NoteRenderer
    {
        public TextMesh jpText;
        [InspectorReadOnly]
        public JPInfo jpInfo;



        public override void Initialize()
        {
            if (jpInfo == null)
                return;

            float barBeat = BeatManager.ToBarBeat(jpInfo.bar, jpInfo.beat);
            railLength = BeatManager.Instance.GetNoteRailLengthWithBarBeat(barBeat);
            float localBarBeatDiff = barBeat - railLength.startBarBeat;
            float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;

            notePos.x = localBarBeatDiff * NoteRail.RailOneBarWidth;
            notePos.y = (railLength.railNumber - 1) * -railTotalHeight;
            notePos.z = -0.5f;

            this.transform.localPosition = notePos;

            jpText.text = ((jpInfo.jumpBarBeat >= 0.0f) ? "+ " : "- ") + Mathf.Abs(jpInfo.jumpBarBeat).ToString("F02");
        }
    }
}