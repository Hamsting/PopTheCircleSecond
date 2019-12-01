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
        [Header("마무리")]
        public Transform endTransform;

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
            endSpriteRenderer = endTransform.GetComponent<SpriteRenderer>();
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
            
            bodyScale = bodyTransform.localScale;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void UpdatePosition()
        {
            base.UpdatePosition();

            double headToEndLength = longNote.endPosition - longNote.position;
            if (note.time - BeatManager.Instance.GameTime <= 0.0f)
                headToEndLength = longNote.endPosition - BeatManager.Instance.Position;

            bodyScale.x = 
                (float)headToEndLength 
                / LongNote.bodyWidth 
                * BeatManager.Instance.GameSpeed 
                / NoteManager.Instance.NoteScale;
            if (bodyScale.x < 0.0f)
                bodyScale.x = 0.0f;
            bodyTransform.localScale = bodyScale;
            
            endNotePos.x = (float)headToEndLength * BeatManager.Instance.GameSpeed / NoteManager.Instance.NoteScale;
            if (endNotePos.x < 0.0f)
                endNotePos.x = 0.0f;
            endTransform.localPosition = endNotePos;
        }
        
        protected override IEnumerator OnNoteMissed()
        {
            float timer = 0.0f;
            float duration = 0.60f;
            float moveLength = 1.35f;
            Color missedColor = new Color();

            while (timer <= duration)
            {
                timer += Time.deltaTime;
                float per = timer / duration;
                missedColor = new Color(1.0f, 1.0f, 1.0f, 0.6f - (0.6f * per));

                if (startSpriteRenderer != null)
                    startSpriteRenderer.color = missedColor;
                if (bodySpriteRenderer != null)
                    bodySpriteRenderer.color = missedColor;
                if (endSpriteRenderer != null)
                    endSpriteRenderer.color = missedColor;

                notePos.x = -moveLength * per;
                this.transform.localPosition = notePos;

                yield return null;
            }

            if (startSpriteRenderer != null)
                startSpriteRenderer.color = Color.white;
            if (bodySpriteRenderer != null)
                bodySpriteRenderer.color = Color.white;
            if (endSpriteRenderer != null)
                endSpriteRenderer.color = Color.white;
            DestroyNote();
        }
    }
}
