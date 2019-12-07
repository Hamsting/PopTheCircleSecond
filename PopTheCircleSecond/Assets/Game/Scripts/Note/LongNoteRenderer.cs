using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{

    public class LongNoteRenderer : NoteRenderer
    {
        [Header("시작")]
        public Transform startTransform;
        [Header("몸")]
        public Transform bodyTransform;
        [Header("커넥터")]
        public Transform conTransform;

        /// <summary>
        /// 롱 노트의 시작 노트 스프라이트 렌더러
        /// </summary>
        private SpriteRenderer startSpriteRenderer;
        /// <summary>
        /// 롱 노트의 중간 노트 스프라이트 렌더러
        /// </summary>
        private SpriteRenderer bodySpriteRenderer;
        /// <summary>
        /// 롱 노트의 끝 노트 스프라이트 렌더러
        /// </summary>
        private SpriteRenderer endSpriteRenderer;

        private LongNote longNote;
        private double endPosition;
        private double length = 0.0f;
        private float noteEndTime = 0.0f;
        private Vector3 bodyScale = Vector3.one;
        private Vector3 endNotePos = Vector3.zero;




        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Awake()
        {
            startSpriteRenderer = startTransform.GetComponent<SpriteRenderer>();
            bodySpriteRenderer = bodyTransform.GetComponent<SpriteRenderer>();
            endSpriteRenderer = conTransform.GetComponent<SpriteRenderer>();
            base.Awake();
        }

        public override void InitializeNote()
        {
            base.InitializeNote();
            if (note == null)
                return;

            longNote = (LongNote)note;
            
            endPosition = BeatManager.Instance.BarBeatToPosition(longNote.endBar, longNote.endBeat);
            noteEndTime = BeatManager.Instance.BarBeatToTime(longNote.endBar, longNote.endBeat);
            
            // conTransform.gameObject.SetActive(longNote.railNumber != longNote.connectedRail);
            conTransform.gameObject.SetActive(false);

            bodyScale = bodyTransform.localScale;
        }

        protected override void Update()
        {
            base.Update();
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
            if (bodyScale.x < 0.0f)
                bodyScale.x = 0.0f;
            bodyTransform.localScale = bodyScale;
            
            // endNotePos.x = (float)headToEndLength * BeatManager.Instance.GameSpeed / NoteManager.Instance.NoteScale;
            // if (endNotePos.x < 0.0f)
            //     endNotePos.x = 0.0f;
            // conTransform.localPosition = endNotePos;
        }

        public override void SetNoteScale(float _scale)
        {
            startTransform.localScale = new Vector3(1.0f, _scale, 1.0f);
        }
    }
}
