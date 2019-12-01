using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopTheCircle.Game;

public class TESTAUTOPLAY : MonoBehaviour
{
    public bool isAutoPlayEnabled = true;

    private NoteManager nm;
    private JudgeManager jm;
    private BeatManager bm;



    private void Update()
    {
        if (nm == null)
            nm = NoteManager.Instance;
        if (jm == null)
            jm = JudgeManager.Instance;
        if (bm == null)
            bm = BeatManager.Instance;

        if (!isAutoPlayEnabled || nm == null || jm == null || bm == null)
        {
            if (InputManager.Instance != null && !InputManager.Instance.enabled)
                InputManager.Instance.enabled = true;
            return;
        }

        if (InputManager.Instance != null && InputManager.Instance.enabled)
            InputManager.Instance.enabled = false;
        
        var notes = nm.spawnedNotes;
        for (int i = 0; i < notes.Count; ++i)
        {
            Note n = notes[i];
            float timeDiff = n.time - bm.GameTime;
            if (timeDiff > 0.0f)
                continue;

            if (n.GetType() == typeof(InfinityNote))
            {
                InfinityNote fn = (InfinityNote)n;
                float barBeat = BeatManager.ToBarBeat(fn.bar, fn.beat);
                float durationBarBeat = BeatManager.ToBarBeat(fn.endBar, fn.endBeat) - barBeat;
                float oneHitTerm = 0.25f;
                if ((fn.endTime - fn.time) / (float)fn.maxHitCount < 0.125f)
                    oneHitTerm = durationBarBeat / (float)fn.maxHitCount;
                float nextHitBarBeat = bm.CorrectBarBeat(barBeat + oneHitTerm * fn.currentHitCount);

                if (nextHitBarBeat <= BeatManager.ToBarBeat(bm.Bar, bm.Beat))
                {
                    ++fn.currentHitCount;
                    if (fn.currentHitCount <= fn.maxHitCount)
                        jm.UpdateComboAndJudgeImage(fn.currentHitCount % 2, 1);
                    else
                        jm.UpdateJudgeImage(fn.currentHitCount % 2, 1);
                }
            }
            else if (n.GetType() == typeof(LongNote))
            {
                LongNote ln = (LongNote)n;
                ln.firstPressed = true;
                ln.pressed = true;
                ln.lastPressedBarBeat = BeatManager.ToBarBeat(bm.Bar, bm.Beat);
            }
            else
            {
                jm.UpdateComboAndJudgeImage(n.railNumber, 1);
                nm.DespawnNote(n, false);
                --i;
            }
        }
    }
}
