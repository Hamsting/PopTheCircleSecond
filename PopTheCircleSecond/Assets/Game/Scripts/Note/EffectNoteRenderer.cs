using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class EffectNoteRenderer : NoteRenderer
    {
        [Header("시작")]
        public Transform startTransform;
        [Header("몸")]
        public Transform bodyTransform;

        /// <summary>
        /// 롱 노트의 시작 노트 스프라이트 렌더러
        /// </summary>
        private SpriteRenderer startSpriteRenderer;
        /// <summary>
        /// 롱 노트의 중간 노트 스프라이트 렌더러
        /// </summary>
        private SpriteRenderer bodySpriteRenderer;

        private EffectNote effectNote;
        private Vector3 bodyScale = Vector3.one;
        private Vector3 endNotePos = Vector3.zero;



        protected override void Awake()
        {
            startSpriteRenderer = startTransform.GetComponent<SpriteRenderer>();
            bodySpriteRenderer = bodyTransform.GetComponent<SpriteRenderer>();
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

            effectNote = (EffectNote)note;

            bodyTransform.gameObject.SetActive(effectNote.IsLongType);

            bodyScale = bodyTransform.localScale;

            Color c = bodySpriteRenderer.color;
            c.a = 1.0f;
            bodySpriteRenderer.color = c;
        }

        protected override void Update()
        {
            base.Update();

            UpdateLongMissedState();
        }

        protected override void UpdatePosition()
        {
            base.UpdatePosition();

            if (effectNote.IsLongType)
            {
                float timeDiff = note.time - BeatManager.Instance.GameTime;
                double headToEndLength = effectNote.endPosition - effectNote.position;
                if (timeDiff <= 0.0f)
                {
                    notePos.y = 0.0f;
                    this.transform.localPosition = notePos;
                    headToEndLength = effectNote.endPosition - BeatManager.Instance.Position;
                }

                float sizeY = -(float)headToEndLength / LongNote.bodyHeight;
                if (sizeY > 0.0f)
                    sizeY = 0.0f;
                bodySpriteRenderer.size = new Vector2(1.3f, sizeY);

                bodyScale.y = BeatManager.Instance.GameSpeed;
                bodyTransform.localScale = bodyScale;
            }
        }

        public override void SetNoteScale(float _scale)
        {
            startTransform.localScale = new Vector3(1.0f, _scale, 1.0f);
        }

        private void UpdateLongMissedState()
        {
            if (!effectNote.IsLongType || !effectNote.firstTicked)
                return;

            Color c = bodySpriteRenderer.color;
            c.a = (effectNote.pressed) ? 1.0f : 0.35f;
            bodySpriteRenderer.color = c;
        }
    }
}