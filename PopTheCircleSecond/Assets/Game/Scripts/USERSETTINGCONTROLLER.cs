using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class USERSETTINGCONTROLLER : MonoBehaviour
    {
        [Range(0.5f, 10.0f)]
        public float gameSpeed = 3.0f;
        [Range(0.1f, 4.0f)]
        public float noteScale = 2.5f;



        private void Update()
        {
            BeatManager.Instance.GameSpeed = gameSpeed;
            NoteManager.Instance.NoteScale = noteScale;
        }
    }
}