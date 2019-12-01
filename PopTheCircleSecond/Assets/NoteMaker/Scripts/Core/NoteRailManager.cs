﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    public class NoteRailManager : Singleton<NoteRailManager>
    {
        public Transform railPivot;
        public Transform railRoot;
        public GameObject noteRailPrefab;
        public Sprite[] barGrids;
        public Transform positionBar;
        public float railSpacing = 1.2f;
        public float visibleRailStartBarBeat = 0.0f;
        public float visibleRailEndBarBeat = 4.0f;
        public float CurrentScroll
        {
            get
            {
                return currentScroll;
            }
            set
            {
                currentScroll = value;
                if (currentScroll < 0.0f)
                    currentScroll = 0.0f;
                UpdateRail();
            }
        }
        public float CameraHeight
        {
            get
            {
                return cameraHeight;
            }
            set
            {
                cameraHeight = Mathf.Clamp(value, 3.0f * 2.0f, 15.0f * 2.0f);
                UpdateCameraSize();
            }
        }

        private float cameraHeight = 5.0f;
        [SerializeField, InspectorReadOnly]
        private float currentScroll = 0.0f;
        [SerializeField, InspectorReadOnly]
        private int railMinNumber = -1;
        [SerializeField, InspectorReadOnly]
        private int railMaxNumber = -1;
        private List<NoteRail> noteRails;
        private int barRailIndex = 0;



        private void Start()
        {
            noteRails = new List<NoteRail>();
            cameraHeight = Camera.main.orthographicSize * 2.0f;
            UpdateRail();
            UpdateCameraSize();
        }

        private void Update()
        {
            float curBarBeat = BeatManager.ToBarBeat(BeatManager.Instance.Bar, BeatManager.Instance.Beat);
            NoteRailLength rl = BeatManager.Instance.GetNoteRailLengthWithBarBeat(curBarBeat);

            Vector3 pos = new Vector3(0.0f, 0.0f, -5.0f);
            int railIndex = rl.railNumber - 1;
            float barBeat = curBarBeat - rl.startBarBeat;
            pos.x = (barBeat / 4.0f) * 16.0f;
            pos.y = railIndex * -(NoteRail.RailHeight + railSpacing);
            positionBar.transform.localPosition = pos;
        }

        private void UpdateCameraSize()
        {
            Camera.main.orthographicSize = cameraHeight * 0.5f;
            railPivot.localPosition = new Vector3(
                -(cameraHeight * 0.5f) / Screen.height * Screen.width + 1.38f,
                cameraHeight * 0.5f - (4.0f + 2.5f * (cameraHeight / 10.0f - 1.0f)),
                0.0f);
        }

        private void UpdateRail()
        {
            float lastRailMinNumber = railMinNumber;
            float lastRailMaxNumber = railMaxNumber;
            float railHeightWithSpacing = NoteRail.RailHeight + railSpacing;
            float topRailPos = (currentScroll % railHeightWithSpacing) % NoteRail.RailHeight;

            railMinNumber = Mathf.Clamp((int)(currentScroll / railHeightWithSpacing), 1, 32767);
            railMaxNumber = Mathf.Clamp(railMinNumber + (int)((cameraHeight + topRailPos) / railHeightWithSpacing) + 1, 1, 32767);

            if (lastRailMinNumber != railMinNumber || lastRailMaxNumber != railMaxNumber)
                UpdateRailSpawn();

            railRoot.localPosition = new Vector3(0.0f, currentScroll, 0.0f);
            foreach (var r in noteRails)
            {
                r.transform.localPosition = new Vector3(0.0f, (r.RailNumber - 1) * -railHeightWithSpacing, 0.0f);
            }
        }

        private void UpdateRailSpawn()
        {
            foreach (var r in noteRails)
            {
                ObjectPoolManager.Instance.Free(r.gameObject);
            }
            noteRails.Clear();

            for (int i = railMinNumber; i <= railMaxNumber; ++i)
            {
                GameObject railObj = ObjectPoolManager.Instance.Get(noteRailPrefab.name, true);
                railObj.transform.parent = railRoot;

                /*
                float barCount = (float)(BeatManager.Instance.ctInfos[BeatManager.Instance.ctInfos.Count - 1].numerator);
                int startBar = BeatManager.Instance.ctInfos[BeatManager.Instance.ctInfos.Count - 1].numerator * i;
                if (i <= BeatManager.Instance.railLengths.Count)
                {
                    NoteRailLength rl = BeatManager.Instance.railLengths[i - 1];
                    barCount = rl.barBeatLength;
                    startBar = (int)rl.startBarBeat;
                }
                */
                NoteRailLength rl = BeatManager.Instance.GetNoteRailLengthWithRailNumber(i);
                float barCount = rl.barBeatLength;
                int startBar = (int)rl.startBarBeat;

                NoteRail rail = railObj.GetComponent<NoteRail>();
                rail.RailNumber = i;
                rail.StartBar = startBar;
                rail.BarCount = barCount;
                rail.UpdateBarGridSprite(barGrids[barRailIndex]);
                noteRails.Add(rail);

                if (i == railMinNumber)
                    visibleRailStartBarBeat = rl.startBarBeat;
                else if (i == railMaxNumber)
                    visibleRailEndBarBeat = rl.nextStartBarBeat;
            }

            NoteManager.Instance.UpdateNoteSpawn();
        }

        public void UpdateBarGrid(int _index)
        {
            barRailIndex = _index;
            foreach (var r in noteRails)
            {
                r.UpdateBarGridSprite(barGrids[barRailIndex]);
            }
        }

        public void UpdateRailSpawnImmediately()
        {
            float lastRailMinNumber = railMinNumber;
            float lastRailMaxNumber = railMaxNumber;
            float railHeightWithSpacing = NoteRail.RailHeight + railSpacing;
            float topRailPos = (currentScroll % railHeightWithSpacing) % NoteRail.RailHeight;

            railMinNumber = Mathf.Clamp((int)(currentScroll / railHeightWithSpacing), 1, 32767);
            railMaxNumber = Mathf.Clamp(railMinNumber + (int)((cameraHeight + topRailPos) / railHeightWithSpacing) + 1, 1, 32767);

            UpdateRailSpawn();

            railRoot.localPosition = new Vector3(0.0f, currentScroll, 0.0f);
            foreach (var r in noteRails)
            {
                r.transform.localPosition = new Vector3(0.0f, (r.RailNumber - 1) * -railHeightWithSpacing, 0.0f);
            }
        }
    }
}