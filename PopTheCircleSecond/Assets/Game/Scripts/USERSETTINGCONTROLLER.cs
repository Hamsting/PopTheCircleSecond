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
        
        private float lastGameSpeed = 3.0f;
        private float lastNoteScale = 2.5f;



        private void Awake()
        {
            UserSettings.gameSpeed = gameSpeed;    
            UserSettings.noteScale = noteScale;
            lastGameSpeed = gameSpeed;
            lastNoteScale = noteScale;
        }

        private void Start()
        {
            BeatManager.Instance.GameSpeed = gameSpeed;
            NoteManager.Instance.NoteScale = noteScale;
        }

        private void Update()
        {
            if (gameSpeed != lastGameSpeed)
            {
                BeatManager.Instance.GameSpeed = gameSpeed;
                lastGameSpeed = gameSpeed;
            }
            if (noteScale != lastNoteScale)
            {
                NoteManager.Instance.NoteScale = noteScale;
                lastNoteScale = noteScale;
            }
        }
    }
}