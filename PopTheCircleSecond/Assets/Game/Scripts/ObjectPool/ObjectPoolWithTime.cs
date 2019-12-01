using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 시간을 이용한 오브젝트 풀링 이벤트 컴포넌트
    /// </summary>
    public class ObjectPoolWithTime : ObjectPoolFreeEventer
    {
        /// <summary>
        /// 오브젝트 잔여 시간
        /// </summary>
        public float lifeTime = 2f;

        void OnEnable()
        {
            StartCoroutine(DisableTimer());
        }

        IEnumerator DisableTimer()
        {
            yield return new WaitForSeconds(lifeTime);
            Free();
        }

    }
}