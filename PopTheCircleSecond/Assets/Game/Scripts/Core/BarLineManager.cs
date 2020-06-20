using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class BarLineManager : MonoBehaviour
    {
        [SerializeField, InspectorReadOnly]
        private List<int> bars;



        private void Awake()
        {
        }

        private void Start()
        {
            bars = new List<int>();

            if (NoteManager.Instance.MaxBarNumber == -1)
                return;

            int curBar = 0;
            int curCT = 4;
            int ctInfoIndex = 0;
            int bpmInfoIndex = 0;
            for (; curBar <= NoteManager.Instance.MaxBarNumber;)
            {
                bool needIncreaseBar = true;
                bool needContinue = false;

                if (BeatManager.Instance.CTInfos.Count > 0)
                {
                    for (int i = ctInfoIndex; i < BeatManager.Instance.CTInfos.Count; ++i)
                    {
                        CTInfo ct = BeatManager.Instance.CTInfos[i];
                        if (curBar >= ct.bar)
                        {
                            if (curBar == ct.bar)
                                needContinue = true;

                            curCT = ct.numerator;
                            curBar = ct.bar;
                            needIncreaseBar = false;
                            ++ctInfoIndex;
                            break;
                        }
                    }
                }

                if (BeatManager.Instance.BPMInfos.Count > 0)
                {
                    for (int i = bpmInfoIndex; i < BeatManager.Instance.BPMInfos.Count; ++i)
                    {
                        BPMInfo bpm = BeatManager.Instance.BPMInfos[i];
                        int actualBar = (bpm.beat > 0.0f) ? bpm.bar + 1 : bpm.bar;
                        if (curBar >= actualBar)
                        {
                            if (curBar == actualBar)
                                needContinue = true;

                            curBar = actualBar;
                            needIncreaseBar = false;
                            ++bpmInfoIndex;
                            break;
                        }
                    }
                }
                if (needContinue)
                    continue;
                if (needIncreaseBar)
                    curBar += curCT;

                bars.Add(curBar);
            }
        }

        private void Update()
        {
            int railEndBar = (int)BeatManager.Instance.RailEndBarBeat;
            while (bars.Count > 0)
            {
                if (bars[0] <= railEndBar)
                {
                    double position = BeatManager.Instance.BarBeatToPosition(bars[0], 0.0f);
                    for (int i = 4; i <= 6; ++i)
                    {
                        GameObject barLineObj = ObjectPoolManager.Instance.Get("BarLine", true);
                        barLineObj.transform.parent = NoteManager.Instance.rails[i];

                        BarLine barLine = barLineObj.GetComponent<BarLine>();
                        barLine.bar = bars[0];
                        barLine.railNumber = i;
                        barLine.position = position;
                        barLine.Initialize();
                    }
                    bars.RemoveAt(0);
                }
                else
                    break;
            }
        }
    }
}