using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PopTheCircle.NoteEditor
{
    public class SimulateController : Singleton<SimulateController>
    {
        public int simulateStartBar = 0;
        public float simulateStartBeat = 0.0f;

        public string lastNoteDataJsonPath = "";
        public int lastPositionBar = 0;
        public float lastPositionBeat = 0.0f;



        protected override void Awake()
        {
            base.Awake();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene _scene, LoadSceneMode _sceneMode)
        {
            PopTheCircle.Game.UserSettings.LoadUserSettings();

            if (_scene.name.Equals("Game"))
                Invoke("StartSimulatePlay", 0.05f);
        }

        private void StartSimulatePlay()
        {
            float startTime = PopTheCircle.Game.BeatManager.Instance.BarBeatToTime(simulateStartBar, simulateStartBeat) -
                              PopTheCircle.Game.GameManager.StartDelayTime;

            PopTheCircle.Game.MusicManager.Instance.MusicPosition = startTime;
            PopTheCircle.Game.BeatManager.Instance.GotoTime(startTime);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.F))
                    SceneManager.LoadScene("Blank");
                if (Input.GetKeyDown(KeyCode.R))
                    SceneManager.LoadScene("Game");
            }
        }
    }
}