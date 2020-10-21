using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class BPMChangeNoteRenderer : NoteRenderer
    {
        public TextMesh bpmText;
        [InspectorReadOnly]
        public BPMInfo bpmInfo;



        public override void Initialize()
        {
            if (bpmInfo == null)
                return;

            float barBeat = BeatManager.ToBarBeat(bpmInfo.bar, bpmInfo.beat);
            railLength = BeatManager.Instance.GetNoteRailLengthWithBarBeat(barBeat);
            float localBarBeatDiff = barBeat - railLength.startBarBeat;
            float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;

            notePos.x = 0.0f;
            notePos.y = (railLength.railNumber - 1) * -railTotalHeight;
            notePos.z = -0.5f;

            this.transform.localPosition = notePos;

            string resStr = bpmInfo.bpm.ToString("0.###");
            if (bpmInfo.stopEffect)
                resStr += "(STOP)";
            bpmText.text = resStr;
        }
    }
}