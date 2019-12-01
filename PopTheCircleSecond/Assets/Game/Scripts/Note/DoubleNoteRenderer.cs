using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class DoubleNoteRenderer : NoteRenderer
    {
        private DoubleNote doubleNote;
        private SpriteRenderer topSpriteRenderer;
        private SpriteRenderer bottomSpriteRenderer;



        protected override void Awake()
        {
            SpriteRenderer[] rens = this.GetComponentsInChildren<SpriteRenderer>();
            if (rens.Length >= 2)
            {
                topSpriteRenderer = rens[0];
                bottomSpriteRenderer = rens[1];
            }
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override void InitializeNote()
        {
            base.InitializeNote();
            if (note == null)
                return;

            doubleNote = (DoubleNote)note;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override IEnumerator OnNoteMissed()
        {
            float timer = 0.0f;
            float duration = 0.60f;
            float moveLength = 1.35f;
            float xPosOffset = notePos.x;

            while (timer <= duration)
            {
                timer += Time.deltaTime;
                float per = timer / duration;

                Color c = new Color(1.0f, 1.0f, 1.0f, 0.6f - (0.6f * per));
                if (topSpriteRenderer != null)
                    topSpriteRenderer.color = c;
                if (bottomSpriteRenderer != null)
                    bottomSpriteRenderer.color = c;

                notePos.x = xPosOffset - moveLength * per;
                this.transform.localPosition = notePos;

                yield return null;
            }

            if (topSpriteRenderer != null)
                topSpriteRenderer.color = Color.white;
            if (bottomSpriteRenderer != null)
                bottomSpriteRenderer.color = Color.white;
            DestroyNote();
        }
    }
}