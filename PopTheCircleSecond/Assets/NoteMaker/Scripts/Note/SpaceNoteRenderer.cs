using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class SpaceNoteRenderer : NoteRenderer
    {
        public List<GameObject> bodies;

        private SpaceNote spaceNote;



        public override void Initialize()
        {
            base.Initialize();

            spaceNote = (SpaceNote)note;

            while (bodies.Count > 1)
            {
                Destroy(bodies[1]);
                bodies.RemoveAt(1);
            }

            if (!spaceNote.IsLongType)
            {
                bodies[0].SetActive(false);
                return;
            }
            else
            {
                bodies[0].SetActive(true);

                float railTotalHeight = NoteRail.RailHeight + NoteRailManager.Instance.railSpacing;

                float startBarBeat = BeatManager.ToBarBeat(spaceNote.bar, spaceNote.beat);
                float endBarBeat = BeatManager.ToBarBeat(spaceNote.endBar, spaceNote.endBeat);
                NoteRailLength endRailLength = BeatManager.Instance.GetNoteRailLengthWithBarBeat(endBarBeat);
                float localStartBarBeatDiff = startBarBeat - railLength.startBarBeat;
                float localEndBarBeatDiff = endBarBeat - endRailLength.startBarBeat;
                int railNumDiff = endRailLength.railNumber - railLength.railNumber;


                for (int i = 0; i <= railNumDiff; ++i)
                {
                    float barBeatLength = 0.0f;
                    Vector3 bodyPos = new Vector3(0.0f, 0.0f, 0.5f);
                    if (i == 0)
                    {
                        barBeatLength = Mathf.Clamp(endBarBeat - startBarBeat, 0.0f, railLength.barBeatLength - localStartBarBeatDiff);
                        bodies[i].transform.localScale = new Vector3(barBeatLength * NoteRail.RailOneBarWidth, 1.0f, 1.0f);
                        bodies[i].transform.localPosition = bodyPos;
                    }
                    else
                    {
                        GameObject dupedBody = Instantiate<GameObject>(bodies[0], this.transform);
                        bodies.Add(dupedBody);

                        if (i == railNumDiff)
                            barBeatLength = localEndBarBeatDiff;
                        else
                            barBeatLength = BeatManager.Instance.GetNoteRailLengthWithRailNumber(railLength.railNumber + i).barBeatLength;
                        bodies[i].transform.localScale = new Vector3(barBeatLength * NoteRail.RailOneBarWidth, 1.0f, 1.0f);

                        bodyPos.x = -localStartBarBeatDiff * NoteRail.RailOneBarWidth;
                        bodyPos.y = (float)i * -railTotalHeight;
                        bodies[i].transform.localPosition = bodyPos;
                    }
                }

                notePos.z = 0.0f;
                this.transform.localPosition = notePos;
            }
        }
    }
}