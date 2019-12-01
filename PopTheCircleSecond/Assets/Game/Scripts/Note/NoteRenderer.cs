using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 노트 렌더링의 기초 클래스이다.
    /// </summary>
    public class NoteRenderer : MonoBehaviour
    {
        [InspectorReadOnly]
        public Note note;
        
        protected Vector3 notePos = Vector3.zero;



        protected virtual void Awake()
        {
            // InitializeNote();
        }

        protected virtual void OnEnable()
        {
            // InitializeNote();
        }

        protected virtual void Update()
        {
            if (note == null || note.isMissed)
                return;

            UpdatePosition();
        }

        public virtual void InitializeNote()
        {
            if (note == null)
            {
                Debug.LogError("NoteRenderer::노트 데이터가 없습니다.");
                DestroyNote();
                return;
            }
        }
        
        protected virtual void UpdatePosition()
        {
            double positionDiff = note.position - BeatManager.Instance.Position;
            notePos.x = (float)positionDiff * BeatManager.Instance.GameSpeed;
            notePos.z = (float)(positionDiff / GlobalDefines.RailLength) * 0.01f;

            float timeDiff = note.time - BeatManager.Instance.GameTime;
            if (timeDiff <= 0.0f)
                notePos.x = 0.0f;

            this.transform.localPosition = notePos;
        }

        protected virtual IEnumerator OnNoteMissed()
        {
            DestroyNote();
            yield break;
        }

        public virtual void DestroyNote()
        {
            if (note != null)
            {
                NoteManager.Instance.spawnedNotes.Remove(note);
                note = null;
            }

            this.StopCoroutine("OnNoteMissed");
            
            ObjectPoolManager.Instance.Free(this.gameObject);
        }
    }
}