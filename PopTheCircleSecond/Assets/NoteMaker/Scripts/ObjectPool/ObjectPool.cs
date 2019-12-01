using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class ObjectPool
    {
        /// <summary>
        /// 오브젝트 
        /// </summary>
        public GameObject source;

        /// <summary>
        /// 최대  생성 갯수
        /// </summary>
        public int maxAmount;

        /// <summary>
        /// 폴더 오브젝트
        /// </summary>
        public GameObject folder;

        /// <summary>
        /// 대기중인 리스트
        /// </summary>
        public List<GameObject> unusedList = new List<GameObject>();

    }
}