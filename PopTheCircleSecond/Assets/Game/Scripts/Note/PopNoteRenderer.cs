using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class PopNoteRenderer : NoteRenderer
    {
        private PopNote popNote;
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

            popNote = (PopNote)note;
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}