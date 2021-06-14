﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    [System.Serializable]
    public class MineNote : Note
    {
        public override Note GetInstance()
        {
            return new MineNote()
            {
                bar = this.bar,
                beat = this.beat,
                noteType = this.noteType,
                railNumber = this.railNumber,

                position = this.position,
                time = this.time,
                isMissed = this.isMissed,
                noteObject = this.noteObject,
            };
        }
    }
}