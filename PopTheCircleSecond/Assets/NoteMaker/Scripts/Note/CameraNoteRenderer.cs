using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class CameraNoteRenderer : NoteRenderer
    {
        private CameraNote cameraNote;



        public override void Initialize()
        {
            base.Initialize();

            cameraNote = (CameraNote)note;
        }
    }
}