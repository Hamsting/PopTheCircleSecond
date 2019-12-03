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
            if (timeDiff > 0.017f)
                continue;

            if (n.GetType() == typeof(LongNote))
            {
                LongNote ln = (LongNote)n;
                ln.firstPressed = true;
                ln.pressed = true;
                ln.lastPressedBarBeat = BeatManager.ToBarBeat(bm.Bar, bm.Beat);
            }
            else
            {
                jm.UpdateJudgeAndCombo(n.railNumber, 1);
                nm.DespawnNote(n, false);
                --i;
            }
        }
    }
}
