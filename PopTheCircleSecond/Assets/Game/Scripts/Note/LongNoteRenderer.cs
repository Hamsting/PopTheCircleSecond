using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{

    public class LongNoteRenderer : NoteRenderer
    {
        private const float ConnectorDefaultYScale = 0.45f;

        [Header("시작")]
        public Transform startTransform;
        [Header("몸")]
        public Transform bodyTransform;
        [Header("커넥터")]
        public Transform conTransform;

        /// <summary>
        /// 롱 노트의 시작 스프라이트 렌더러
        /// </summary>
        private SpriteRenderer startSpriteRenderer;
        /// <summary>
        /// 롱 노트의 중간 스프라이트 렌더러
        /// </summary>
        private SpriteRenderer bodySpriteRenderer;
        /// <summary>
        /// 롱 노트의 커넥터 스프라이트 렌더러
        /// </summary>
        private SpriteRenderer conSpriteRenderer;

        private LongNote longNote;
        private Vector3 bodyScale = Vector3.one;
        private Vector3 conPos = Vector3.zero;
        private Vector3 conScale = Vector3.one;




        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Awake()
        {
            startSpriteRenderer = startTransform.GetComponent<SpriteRenderer>();
            bodySpriteRenderer = bodyTransform.GetComponent<SpriteRenderer>();
            conSpriteRenderer = conTransform.GetComponent<SpriteRenderer>();
            base.Awake();
        }

        public override void InitializeNote()
        {
            base.InitializeNote();
            if (note == null)
                return;

            longNote = (LongNote)note;
            
            if (longNote.railNumber != longNote.connectedRail && longNote.connectedRail != -1)
            {
                int railDiff = longNote.connectedRail - longNote.railNumber;
                conTransform.gameObject.SetActive(true);

                conScale = new Vector3(
                    1.0f + (float)Mathf.Abs(railDiff) * 1.08f,
                    ConnectorDefaultYScale * BeatManager.Instance.GameSpeed,
                    1.0f);
                conTransform.localScale = conScale;
                conPos = new Vector3((float)railDiff * 1.08f * 0.5f, 0.0f, 0.0f);
            }
            else
                conTransform.gameObject.SetActive(false);

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
            
            float timeDiff = note.time - BeatManager.Instance.GameTime;
            double headToEndLength = longNote.endPosition - longNote.position;
            if (timeDiff <= 0.0f)
            {
                notePos.y = 0.0f;
                this.transform.localPosition = notePos;
                headToEndLength = longNote.endPosition - BeatManager.Instance.Position;
            }

            bodyScale.y =
                (float)headToEndLength
                / LongNote.bodyHeight
                * BeatManager.Instance.GameSpeed;
            if (bodyScale.y < 0.0f)
                bodyScale.y = 0.0f;
            bodyTransform.localScale = bodyScale;

            conPos.y = (float)headToEndLength * BeatManager.Instance.GameSpeed; //  / NoteManager.Instance.NoteScale;
            if (conPos.y < 0.0f)
                conPos.y = 0.0f;
            conTransform.localPosition = conPos;

            conScale.y = ConnectorDefaultYScale * BeatManager.Instance.GameSpeed;
            conTransform.localScale = conScale;
        }

        public override void SetNoteScale(float _scale)
        {
            startTransform.localScale = new Vector3(1.0f, _scale, 1.0f);
        }

        private void UpdateLongMissedState()
        {
            if (!longNote.firstTicked)
                return;

            Color c = bodySpriteRenderer.color;
            c.a = (longNote.pressed) ? 1.0f : 0.35f;
            bodySpriteRenderer.color = c;
        }
    }
}
