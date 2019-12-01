using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopTheCircle.Game
{
    /// <summary>
    /// 일반 노트를 렌더링한다.
    /// </summary>
    public class InfinityNoteRenderer : NoteRenderer
    {
        public const float BeginAnimDuration = 0.5f;
        public const float MissedAnimDuration = 0.5f;
        public const float ClearAnimDuration = 0.75f;

        private const float CenterFillDefaultScale = 0.25f;

        public Transform centerFill;

        private InfinityNote infinityNote;
        private Animator anim;



        protected override void Awake()
        {
            anim = this.GetComponentInChildren<Animator>();
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(OnNoteBegin());
        }

        public override void InitializeNote()
        {
            base.InitializeNote();
            if (note == null)
                return;

            infinityNote = (InfinityNote)note;
        }

        protected override void Update()
        {
            base.Update();

        }

        private void LateUpdate()
        {
            if (centerFill != null)
            {
                float hitRatio = Mathf.Clamp01((float)infinityNote.currentHitCount / (float)infinityNote.maxHitCount);
                float centerScale = CenterFillDefaultScale + (1.0f - CenterFillDefaultScale) * hitRatio;
                centerFill.localScale = new Vector3(centerScale, centerScale, centerScale);
            }
        }

        protected override void UpdatePosition()
        {
            this.transform.localPosition = Vector3.zero;
        }

        public void SetNoteClear()
        {
            StartCoroutine(OnNoteClear());
        }

        protected override IEnumerator OnNoteMissed()
        {
            if (anim != null)
            {
                anim.Play("Missed", -1, 0.0f);
                yield return new WaitForSeconds(MissedAnimDuration);
            }
            DestroyNote();
        }

        private IEnumerator OnNoteBegin()
        {
            if (anim != null)
            {
                anim.Play("Begin", -1, 0.0f);
                yield return new WaitForSeconds(BeginAnimDuration);
            }
        }

        private IEnumerator OnNoteClear()
        {
            if (anim != null)
            {
                anim.Play("Clear", -1, 0.0f);
                yield return new WaitForSeconds(ClearAnimDuration);
            }
            DestroyNote();
        }
    }
}