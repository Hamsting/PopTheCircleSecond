// #define LongNoteTest
#define NormalNoteTest
// #define DragNoteTest
// #define InfinityNoteTest
// #define DoubleNoteTest

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopTheCircle.Game;

public class TESTSCRIPT : MonoBehaviour
{



    private void Start()
    {
        BeatManager.Instance.AddNewBPMInfo(0, 0.0f, 120.0f, false);

        // BeatManager.Instance.AddNewBPMInfo(0, 0.0f, 60.0f, false);
        // BeatManager.Instance.AddNewBPMInfo(2, 0.0f, 120.0f, false);
        // BeatManager.Instance.AddNewBPMInfo(6, 0.0f, 240.0f, false);
        // BeatManager.Instance.AddNewBPMInfo(14, 0.0f, 60.0f, false);
        // BeatManager.Instance.AddNewBPMInfo(16, 0.0f, -120.0f, false);
        // BeatManager.Instance.AddNewBPMInfo(17, 0.0f, 120.0f, false);
        // BeatManager.Instance.AddNewBPMInfo(18, 0.0f, -120.0f, false);
        // BeatManager.Instance.AddNewBPMInfo(19, 0.0f, 120.0f, false);
        // BeatManager.Instance.AddNewBPMInfo(20, 0.0f, 161.0f, true);
        // BeatManager.Instance.AddNewBPMInfo(25, 0.0f, 161.0f, false);

#if (NormalNoteTest)
        for (int i = 0; i < 360; ++i)
        {
            int bar = i / 2;
            float beat = (float)(i % 2) / 2.0f * (float)GlobalDefines.BeatPerBar;
            if (!BeatManager.Instance.IsPossibleBarBeat(bar, beat))
                continue;

            NoteManager.Instance.AddNote(new NormalNote()
            {
                bar = bar,
                beat = beat,
                railNumber = i % 2
            });
        }
#endif

#if (LongNoteTest)
        for (int i = 1; i < 240; ++i)
        {
            // 4연속 단타 롱노트
            /*
            NoteManager.Instance.AddNote(new LongNote()
            {
                bar = (int)(i * 0.25f),
                beat = GlobalDefines.BeatPerBar / 8 * (i % 4) * 2,

                endBar = (int)(i * 0.25f + 0.0f),
                endBeat = GlobalDefines.BeatPerBar / 8 * ((i % 4) * 2 + 1),

                railNumber = (i / 4) % 2
            });
            */

            // 일반 롱노트
            
            NoteManager.Instance.AddNote(new LongNote()
            {
                bar = (int)(i * 2f),
                beat = 0,

                endBar = (int)(i * 2f + 1f),
                endBeat = GlobalDefines.BeatPerBar / 4 * 3,

                railNumber = i % 2
            });
            
        }

        // 매우 긴 롱노트
        /*
        NoteManager.Instance.AddNote(new LongNote()
        {
            bar = 0,
            beat = 48,

            endBar = 120,
            endBeat = 48,

            railNumber = 0
        });
        */
        /*
        NoteManager.Instance.AddNote(new LongNote()
        {
            bar = 1,
            beat = 48,

            endBar = 3,
            endBeat = 48,

            railNumber = 1
        });
        */
#endif

#if (InfinityNoteTest)
        /*
        for (int i = 0; i < 20; ++i)
        {
            NoteManager.Instance.AddNote(new InfinityNote()
            {
                bar = 1 + i * 6,
                beat = 0,

                endBar = 1 + i * 6 + 4,
                endBeat = 48,

                maxHitCount = 18,
                railNumber = 0
            });
        }
        */

        NoteManager.Instance.AddNote(new InfinityNote()
        {
            bar = 1,
            beat = 0,

            endBar = 8,
            endBeat = 48,

            maxHitCount = 32,
            railNumber = 0
        });

#endif

#if (DoubleNoteTest)
        for (int i = 0; i < 360; ++i)
        {
            int bar = i / 2;
            float beat = (float)(i % 2) / 2.0f * (float)GlobalDefines.BeatPerBar;
            if (!BeatManager.Instance.IsPossibleBarBeat(bar, beat))
                continue;

            NoteManager.Instance.AddNote(new DoubleNote()
            {
                bar = bar,
                beat = beat,
                railNumber = i % 2
            });
        }
#endif
#if (DragNoteTest)
        for (int i = 0; i < 360; ++i)
        {
            int bar = i / 2;
            float beat = (float)(i % 2) / 2.0f * (float)GlobalDefines.BeatPerBar;
            if (!BeatManager.Instance.IsPossibleBarBeat(bar, beat))
                continue;

            NoteManager.Instance.AddNote(new DragNote()
            {
                bar = bar,
                beat = beat,
                railNumber = i % 2,
                direction = Random.Range(0, 2)
            });
        }
#endif
    }

    private void Update()
    {

    }
}
