using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    public class GlobalData : Singleton<GlobalData>
    {
        [InspectorReadOnly]
        public AudioClip musicClip;
        [InspectorReadOnly]
        public JSONObject noteDataJson;


        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(this.gameObject);
        }
    }
}