using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 일반 노트를 렌더링한다.
    /// </summary>
    public class NormalNoteRenderer : NoteRenderer
    {
        private NormalNote normalNote;
        private SpriteRenderer noteSpriteRenderer;



        protected override void Awake()
        {
            noteSpriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
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

            normalNote = (NormalNote)note;
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

                if (noteSpriteRenderer != null)
                    noteSpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.6f - (0.6f * per));

                notePos.x = xPosOffset - moveLength * per;
                this.transform.localPosition = notePos;

                yield return null;
            }

            if (noteSpriteRenderer != null)
                noteSpriteRenderer.color = Color.white;
            DestroyNote();
        }
    }
}