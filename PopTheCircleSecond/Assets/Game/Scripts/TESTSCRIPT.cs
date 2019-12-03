// #define LongNoteTest
// #define NormalNoteTest
#define NormalNotePatternedTest
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
        BeatManager.Instance.AddNewBPMInfo(0, 0.0f, 160.0f, false);
        BeatManager.Instance.AddNewBPMInfo(8, 0.0f, 80.0f, false);
        BeatManager.Instance.AddNewBPMInfo(12, 0.0f, 160.0f, false);
        BeatManager.Instance.AddNewBPMInfo(20, 0.0f, 40.0f, false);
        BeatManager.Instance.AddNewBPMInfo(22, 0.0f, 160.0f, false);
        BeatManager.Instance.AddNewBPMInfo(30, 0.0f, 160.0f, true);
        BeatManager.Instance.AddNewBPMInfo(32, 0.0f, 160.0f, false);

        // BeatManager.Instance.AddNewBPMInfo(0, 0.0f, 60.0f, true);
        // BeatManager.Instance.AddNewBPMInfo(2, 0.0f, 120.0f, true);
        // BeatManager.Instance.AddNewBPMInfo(6, 0.0f, 240.0f, true);
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

#if (NormalNotePatternedTest)
        float beat4  = 0.05f;
        float beat8  = 0.30f;
        float beat16 = 0.85f;
        float beat24 = 0.93f;
        float beat32 = 1.00f;
        int lastRail = -1;
        int bar = 0;
        float beat = 0.0f;

        for (int i = 0; i < 960; ++i)
        {
            int railRand = Random.Range(0, 4);
            if (railRand == lastRail)
                railRand = (railRand + 1) % 4;
            lastRail = railRand;
            
            if (!BeatManager.Instance.IsPossibleBarBeat(bar, beat))
            {
                float barBeat = BeatManager.ToBarBeat(bar, beat);
                barBeat = BeatManager.Instance.CorrectBarBeat(barBeat);
                bar = (int)barBeat;
                beat = (barBeat - (float)bar) * (float)GlobalDefines.BeatPerBar;
            }

            NoteManager.Instance.AddNote(new NormalNote()
            {
                bar = bar,
                beat = beat,
                railNumber = railRand
            });
            
            float beatrate = 4.0f;
            float rand = Random.Range(0.0f, 100.0f) * 0.01f;
            if (rand <= beat4)
                beatrate = 1.0f;
            else if (rand <= beat8)
                beatrate = 2.0f;
            else if (rand <= beat16)
                beatrate = 4.0f;
            else if (rand <= beat24)
                beatrate = 6.0f;
            else
                beatrate = 8.0f;
            
            beat += (float)GlobalDefines.BeatPerBar / beatrate;
            if (beat >= (float)GlobalDefines.BeatPerBar)
            {
                beat -= (float)GlobalDefines.BeatPerBar;
                bar += 1;
            }
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
