﻿using System.Collections;
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
    }
}